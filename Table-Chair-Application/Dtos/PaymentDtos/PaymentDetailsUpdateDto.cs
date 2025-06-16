using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.ValidationAttributes;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos.PaymentDtos
{
    /// <summary>
    /// Payment ma'lumotlarini yangilash uchun DTO
    /// </summary>
    public class PaymentDetailsUpdateDto
    {
        /// <summary>
        /// To'lov summasi (faqat Pending holatida yangilanishi mumkin)
        /// </summary>
        [Range(0.01, double.MaxValue, ErrorMessage = "To'lov summasi 0 dan katta bo'lishi kerak")]
        public decimal Amount { get; set; }

        /// <summary>
        /// To'lov valyutasi (ISO currency code: USD, UZS, EUR)
        /// </summary>
        [Required(ErrorMessage = "Valyuta ko'rsatilishi shart")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Valyuta kodi 3 belgidan iborat bo'lishi kerak")]
        public string? Currency { get; set; }

        /// <summary>
        /// To'lov haqida qo'shimcha izoh
        /// </summary>
        [StringLength(500, ErrorMessage = "Izoh 500 belgidan oshmasligi kerak")]
        public string? Notes { get; set; }

        /// <summary>
        /// To'lov usuli (naqd, karta, bank orqali va h.k.)
        /// </summary>
        [Required(ErrorMessage = "To'lov usuli ko'rsatilishi shart")]
        public PaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// To'lovning taxminiy amal qilish muddati
        /// </summary>
        [DataType(DataType.DateTime)]
        [FutureDate(ErrorMessage = "Amal qilish muddati kelajakdagi sana bo'lishi kerak")]
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// To'lovning taxminiy yetkazib berish sanasi
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? EstimatedDeliveryDate { get; set; }

        /// <summary>
        /// To'lovning manba URL manzili (agar mavjud bo'lsa)
        /// </summary>
        [Url(ErrorMessage = "Noto'g'ri URL formati")]
        public string SourceUrl { get; set; } = null!;

        /// <summary>
        /// To'lov bilan bog'liq metadata (JSON formatida)
        /// </summary>
        public string Metadata { get; set; }=null!;
    }
}
