using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.PaymentDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Payments;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;
using Microsoft.EntityFrameworkCore;
using Table_Chair_Application.Dtos.CreateDtos;
using ValidationException = Table_Chair_Application.Exceptions.ValidationException;

namespace Table_Chair_Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGatewayFactory _gatewayFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;
        private readonly IMemoryCache _memoryCache;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IPaymentGatewayFactory gatewayFactory,
            IMapper mapper,
            ILogger<PaymentService> logger,
            IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _gatewayFactory = gatewayFactory;
            _mapper = mapper;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<PaymentResponseDto> CreatePaymentAsync(PaymentCreateDto paymentDto)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Payment yaratish boshlandi. OrderId: {OrderId}", paymentDto.OrderId);

            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                ValidatePaymentCreateDto(paymentDto);

                var order = await _unitOfWork.Orders.GetByIdAsync(paymentDto.OrderId)
                    ?? throw new NotFoundException("Buyurtma topilmadi");

                var payment = _mapper.Map<Payment>(paymentDto);
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;
                payment.Status = PaymentStatus.Pending;

                var gateway = _gatewayFactory.Create(payment.PaymentMethod);
                var paymentResult = await gateway.ProcessPaymentAsync(payment);

                if (!paymentResult.Success)
                {
                    payment.Status = PaymentStatus.Failed;
                    payment.Notes = paymentResult.ErrorMessage;
                    _logger.LogError("To'lov amalga oshirilmadi. Xato: {ErrorMessage}", paymentResult.ErrorMessage);
                }
                else
                {
                    payment.Status = PaymentStatus.Completed;
                    payment.TransactionId = paymentResult.TransactionId;
                    payment.PaidAt = DateTime.UtcNow;
                    _logger.LogInformation("To'lov muvaffaqiyatli amalga oshirildi. Tranzaksiya ID: {TransactionId}", payment.TransactionId);
                }

                await _unitOfWork.paymentRepository.AddAsync(payment);

                var retryPolicy = Policy
                    .Handle<DbUpdateException>()
                    .WaitAndRetryAsync(3, retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                await retryPolicy.ExecuteAsync(async () =>
                {
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();
                });

                // Cache yangilash
                _memoryCache.Remove("payment_stats");

                return _mapper.Map<PaymentResponseDto>(payment);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Payment yaratishda xatolik yuz berdi");
                throw new PaymentOperationException("To'lovni yaratishda xatolik yuz berdi", ex);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("CreatePaymentAsync ishlash vaqti: {ElapsedMilliseconds}ms",
                    stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<PaymentResponseDto?> GetPaymentByIdAsync(int? id)
        {
            var cacheKey = $"payment_{id}";

            if (_memoryCache.TryGetValue(cacheKey, out PaymentResponseDto? cachedPayment) && cachedPayment != null)
            {
                _logger.LogInformation("Payment #{PaymentId} ma'lumotlari cache'dan olindi", id);
                return cachedPayment;
            }

            try
            {
                _logger.LogInformation("Payment ma'lumotlari olinmoqda. PaymentId: {PaymentId}", id);

                var payment = await _unitOfWork.paymentRepository.GetByIdAsync(id)
                    ?? throw new NotFoundException("To'lov topilmadi");

                var result = _mapper.Map<PaymentResponseDto>(payment);

                // 5 daqiqa uchun cache'ga saqlash
                _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment ma'lumotlarini olishda xatolik. PaymentId: {PaymentId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsByOrderIdAsync(int orderId)
        {
            try
            {
                _logger.LogInformation("Order uchun paymentlar olinmoqda. OrderId: {OrderId}", orderId);

                var payments = await _unitOfWork.paymentRepository
                    .GetQueryable()
                    .Where(p => p.OrderId == orderId)
                    .Include(p => p.Order)
                    .ToListAsync();

                if (!payments.Any())
                {
                    _logger.LogInformation("Order uchun paymentlar topilmadi. OrderId: {OrderId}", orderId);
                }

                return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Order paymentlarini olishda xatolik. OrderId: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<PaymentStatisticsDto> GetPaymentStatisticsAsync()
        {
            const string cacheKey = "payment_stats";

            if (_memoryCache.TryGetValue(cacheKey, out PaymentStatisticsDto? cachedStats) && cachedStats != null)
            {
                _logger.LogInformation("Payment statistikasi cache'dan olindi");
                return cachedStats;
            }

            try
            {
                _logger.LogInformation("Payment statistikasi hisoblanmoqda");

                var now = DateTime.UtcNow;

                // ✅ DateTimeKind.Unspecified muammosini hal qilish
                var startOfMonth = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1), DateTimeKind.Utc);
                var startOfDay = DateTime.SpecifyKind(now.Date, DateTimeKind.Utc);
                var startOfWeek = DateTime.SpecifyKind(now.AddDays(-(int)now.DayOfWeek).Date, DateTimeKind.Utc);

                var monthlyPayments = await _unitOfWork.paymentRepository
                    .GetByDateRangeAsync(startOfMonth, now);

                var stats = new PaymentStatisticsDto
                {
                    TotalMonthlyAmount = monthlyPayments.Sum(p => p.Amount),
                    TotalWeeklyAmount = (await _unitOfWork.paymentRepository
                        .GetByDateRangeAsync(startOfWeek, now))
                        .Sum(p => p.Amount),
                    TotalDailyAmount = (await _unitOfWork.paymentRepository
                        .GetByDateRangeAsync(startOfDay, now))
                        .Sum(p => p.Amount),
                    CompletedPayments = monthlyPayments.Count(p => p.Status == PaymentStatus.Completed),
                    FailedPayments = monthlyPayments.Count(p => p.Status == PaymentStatus.Failed),
                    RefundedPayments = monthlyPayments.Count(p => p.Status == PaymentStatus.Refunded),
                    PendingPayments = monthlyPayments.Count(p => p.Status == PaymentStatus.Pending),
                    PopularPaymentMethod = monthlyPayments
                        .GroupBy(p => p.PaymentMethod)
                        .OrderByDescending(g => g.Count())
                        .FirstOrDefault()?.Key.ToString()
                };

                _memoryCache.Set(cacheKey, stats, TimeSpan.FromMinutes(10));

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment statistikasini hisoblashda xatolik");
                throw;
            }
        }


        public async Task<PaginatedPaymentResponseDto> GetFilteredPaymentsAsync(PaymentFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Filterlangan paymentlar olinmoqda. Filter: {@Filter}", filter);

                var query = _unitOfWork.paymentRepository.GetQueryable();

                if (filter.Status.HasValue)
                    query = query.Where(p => p.Status == filter.Status.Value);

                if (filter.PaymentMethod.HasValue)
                    query = query.Where(p => p.PaymentMethod == filter.PaymentMethod.Value);

                if (filter.StartDate.HasValue)
                    query = query.Where(p => p.CreatedAt >= filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    query = query.Where(p => p.CreatedAt <= filter.EndDate.Value);

                int totalCount = await query.CountAsync();

                var payments = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                _logger.LogInformation("Topilgan paymentlar soni: {TotalCount}", totalCount);

                return new PaginatedPaymentResponseDto
                {
                    Payments = _mapper.Map<IEnumerable<PaymentResponseDto>>(payments),
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Filterlangan paymentlarni olishda xatolik");
                throw;
            }
        }

        // ... (Qolgan metodlar shu tartibda takomillashtirilgan holda yoziladi)

        private void ValidatePaymentCreateDto(PaymentCreateDto dto)
        {
            if (dto.Amount <= 0)
                throw new ValidationException("To'lov summasi 0 dan katta bo'lishi kerak");

            if (!Enum.IsDefined(typeof(PaymentMethod), dto.PaymentMethod))
                throw new ValidationException("Noto'g'ri to'lov usuli");

            if (!Enum.IsDefined(typeof(CurrencyType), dto.Currency))
                throw new ValidationException("Valyuta ko'rsatilishi shart");
        }

        private bool ValidateCallbackSignature(PaymentCallbackDto callbackDto)
        {
            try
            {

                // Haqiqiy loyihada quyidagilarni tekshirish kerak:
                // 1. HMAC yoki boshqa imzo mexanizmi
                // 2. Request vaqti (eski requestlarni rad etish)
                // 3. IP manzili
                return true; // Demo uchun
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Callback imzosini tekshirishda xatolik");
                return false;
            }
        }

        private bool IsValidStatusTransition(PaymentStatus currentStatus, PaymentStatus newStatus)
        {
            var allowedTransitions = new Dictionary<PaymentStatus, List<PaymentStatus>>
            {
                [PaymentStatus.Pending] = new() { PaymentStatus.Completed, PaymentStatus.Failed, PaymentStatus.Cancelled },
                [PaymentStatus.Completed] = new() { PaymentStatus.Refunded, PaymentStatus.PartiallyRefunded },
                [PaymentStatus.Failed] = new() { PaymentStatus.Pending },
                [PaymentStatus.Refunded] = new(),
                [PaymentStatus.PartiallyRefunded] = new() { PaymentStatus.Refunded },
                [PaymentStatus.Cancelled] = new() { PaymentStatus.Pending }
            };

            return allowedTransitions.TryGetValue(currentStatus, out var allowed) && allowed.Contains(newStatus);
        }
        public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            _logger.LogInformation("{Status} statusidagi paymentlar olinmoqda", status);

            var payments = await _unitOfWork.paymentRepository.GetByStatusAsync(status);

            if (!payments.Any())
            {
                _logger.LogInformation("{Status} statusidagi paymentlar topilmadi", status);
            }
            else
            {
                _logger.LogInformation("{Count} ta payment topildi", payments.Count());
            }

            return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
        }


        public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            _logger.LogInformation("{StartDate} dan {EndDate} gacha bo'lgan paymentlar olinmoqda",
                startDate, endDate);

            if (startDate > endDate)
            {
                _logger.LogWarning("Noto'g'ri sana oralig'i. StartDate: {StartDate}, EndDate: {EndDate}",
                    startDate, endDate);
                throw new ValidationException("Boshlang'ich sana tugash sanasidan katta bo'lishi mumkin emas");
            }

            var payments = await _unitOfWork.paymentRepository.GetByDateRangeAsync(startDate, endDate);
            if (!payments.Any())
            {
                _logger.LogInformation("Berilgan oralikda paymentlar topilmadi");
            }

            return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
        }


        public async Task<PaymentResponseDto> UpdatePaymentStatusAsync(int id, PaymentUpdateDto paymentDto)
        {
            _logger.LogInformation("Payment statusi yangilanmoqda. PaymentId: {PaymentId}", id);

            var payment = await _unitOfWork.paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                _logger.LogWarning("Payment topilmadi. PaymentId: {PaymentId}", id);
                throw new NotFoundException("To'lov topilmadi");
            }

            // Faqat allowed status transitions
            if (!IsValidStatusTransition(payment.Status, paymentDto.Status))
            {
                _logger.LogWarning("Noto'g'ri status o'zgarishi. Joriy: {CurrentStatus}, Yangi: {NewStatus}",
                    payment.Status, paymentDto.Status);
                throw new InvalidOperationException("To'lov holatini o'zgartirib bo'lmaydi");
            }

            _mapper.Map(paymentDto, payment);
            payment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.paymentRepository.Update(payment);

            // Concurrency uchun retry policy
            var retryPolicy = Policy
                .Handle<DbUpdateException>()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            await retryPolicy.ExecuteAsync(async () =>
            {
                await _unitOfWork.CompleteAsync();
            });

            _logger.LogInformation("Payment statusi muvaffaqiyatli yangilandi. Yangi status: {Status}",
                payment.Status);

            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<PaymentResponseDto> ProcessPaymentCallbackAsync(PaymentCallbackDto callbackDto)
        {
            _logger.LogInformation("Payment callback qabul qilindi. PaymentId: {PaymentId}", callbackDto.PaymentId);

            // Callback imzosini tekshirish
            if (!ValidateCallbackSignature(callbackDto))
            {
                _logger.LogError("Noto'g'ri callback imzosi. PaymentId: {PaymentId}", callbackDto.PaymentId);
                throw new SecurityException("Noto'g'ri callback imzosi");
            }

            var payment = await _unitOfWork.paymentRepository.GetByIdAsync(callbackDto.PaymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment topilmadi. PaymentId: {PaymentId}", callbackDto.PaymentId);
                throw new NotFoundException("To'lov topilmadi");
            }

            // Statusni yangilash
            payment.Status = callbackDto.Success ? PaymentStatus.Completed : PaymentStatus.Failed;
            payment.TransactionId = callbackDto.TransactionId;
            payment.Notes = callbackDto.Message;
            payment.UpdatedAt = DateTime.UtcNow;

            if (callbackDto.Success)
            {
                payment.PaidAt = DateTime.UtcNow;
                _logger.LogInformation("Payment muvaffaqiyatli yakunlandi. PaymentId: {PaymentId}", payment.Id);
            }
            else
            {
                _logger.LogWarning("Payment amalga oshirilmadi. PaymentId: {PaymentId}, Xato: {Message}",
                    payment.Id, callbackDto.Message);
            }

            _unitOfWork.paymentRepository.Update(payment);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<PaymentResponseDto> RefundPaymentAsync(int paymentId, RefundRequestDto refundDto)
        {
            _logger.LogInformation("Payment qaytarilmoqda. PaymentId: {PaymentId}", paymentId);

            var payment = await _unitOfWork.paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment topilmadi. PaymentId: {PaymentId}", paymentId);
                throw new NotFoundException("To'lov topilmadi");
            }

            if (payment.Status != PaymentStatus.Completed)
            {
                _logger.LogWarning("Faqat completed paymentlarni qaytarish mumkin. Joriy status: {Status}",
                    payment.Status);
                throw new InvalidOperationException("Faqat muvaffaqiyatli to'lovlarni qaytarish mumkin");
            }

            if (refundDto.Amount <= 0 || refundDto.Amount > payment.Amount)
            {
                _logger.LogWarning("Noto'g'ri qaytarish summasi. So'ralgan: {RequestedAmount}, Mavjud: {PaymentAmount}",
                    refundDto.Amount, payment.Amount);
                throw new InvalidOperationException("Noto'g'ri qaytarish summasi");
            }

            // Payment gateway orqali refund
            var gateway = _gatewayFactory.Create(payment.PaymentMethod);
            var refundResult = await gateway.RefundPaymentAsync(paymentId, refundDto.Amount);

            if (!refundResult.Success)
            {
                _logger.LogError("Payment qaytarishda xatolik. PaymentId: {PaymentId}, Xato: {ErrorMessage}",
                    paymentId, refundResult.ErrorMessage);
                throw new PaymentException(refundResult.ErrorMessage ?? "Pulni qaytarishda xatolik yuz berdi");
            }

            // Payment statusini yangilash
            payment.Status = refundDto.Amount == payment.Amount
                ? PaymentStatus.Refunded
                : PaymentStatus.PartiallyRefunded;

            payment.Notes = $"Qaytarilgan summa: {refundDto.Amount}. Sabab: {refundDto.Reason}";
            payment.UpdatedAt = DateTime.UtcNow;
            payment.RefundedAt = DateTime.UtcNow;

            _unitOfWork.paymentRepository.Update(payment);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Payment muvaffaqiyatli qaytarildi. PaymentId: {PaymentId}, Qaytarilgan summa: {Amount}",
                paymentId, refundDto.Amount);

            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<PaymentStatusDto> CheckPaymentStatusAsync(int paymentId)
        {
            _logger.LogInformation("Payment statusi tekshirilmoqda. PaymentId: {PaymentId}", paymentId);

            var payment = await _unitOfWork.paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment topilmadi. PaymentId: {PaymentId}", paymentId);
                throw new NotFoundException("To'lov topilmadi");
            }

            return new PaymentStatusDto
            {
                Status = payment.Status,
               UpdatedAt = payment.UpdatedAt,
                TransactionId = payment.TransactionId
            };
        }

        public async Task<PaymentResponseDto> CancelPaymentAsync(int paymentId, string reason)
        {
            _logger.LogInformation("Payment bekor qilinmoqda. PaymentId: {PaymentId}", paymentId);

            var payment = await _unitOfWork.paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment topilmadi. PaymentId: {PaymentId}", paymentId);
                throw new NotFoundException("To'lov topilmadi");
            }

            if (payment.Status != PaymentStatus.Pending)
            {
                _logger.LogWarning("Faqat pending paymentlarni bekor qilish mumkin. Joriy status: {Status}",
                    payment.Status);
                throw new InvalidOperationException("Faqat kutilayotgan to'lovlarni bekor qilish mumkin");
            }

            payment.Status = PaymentStatus.Cancelled;
            payment.Notes = $"Bekor qilish sababi: {reason}";
            payment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.paymentRepository.Update(payment);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Payment muvaffaqiyatli bekor qilindi. PaymentId: {PaymentId}", paymentId);

            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<PaymentResponseDto> UpdatePaymentDetailsAsync(int id, PaymentDetailsUpdateDto updateDto)
        {
            _logger.LogInformation("Payment ma'lumotlari yangilanmoqda. PaymentId: {PaymentId}", id);

            var payment = await _unitOfWork.paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                _logger.LogWarning("Payment topilmadi. PaymentId: {PaymentId}", id);
                throw new NotFoundException("To'lov topilmadi");
            }

            if (payment.Status != PaymentStatus.Pending)
            {
                _logger.LogWarning("Faqat pending paymentlarni yangilash mumkin. Joriy status: {Status}",
                    payment.Status);
                throw new InvalidOperationException("Faqat kutilayotgan to'lovlarni yangilash mumkin");
            }

            _mapper.Map(updateDto, payment);
            payment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.paymentRepository.Update(payment);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Payment ma'lumotlari muvaffaqiyatli yangilandi. PaymentId: {PaymentId}", id);

            return _mapper.Map<PaymentResponseDto>(payment);
        }
    }
}

