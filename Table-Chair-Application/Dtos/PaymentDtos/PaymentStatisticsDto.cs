using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.PaymentDtos
{
    public class PaymentStatisticsDto
    {
        public decimal TotalMonthlyAmount { get; set; }
        public decimal TotalDailyAmount { get; set; }
        public int CompletedPayments { get; set; }
        public int FailedPayments { get; set; }
        public int RefundedPayments { get; set; }
        public string? PopularPaymentMethod { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
        public decimal TotalWeeklyAmount { get; internal set; }
        public int PendingPayments { get; internal set; }
    }

}
