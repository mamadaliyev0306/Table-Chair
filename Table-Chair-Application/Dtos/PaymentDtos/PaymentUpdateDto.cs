using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos.PaymentDtos
{
    public class PaymentUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        public CurrencyType Currency { get; set; }

        public DateTime? PaidAt { get; set; }

        public PaymentStatus Status { get; set; }

        public string? TransactionId { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string? Notes { get; set; }

        public DateTime? RefundedAt { get; set; }
    }

}
