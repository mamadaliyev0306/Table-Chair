using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos.CreateDtos
{
    public class PaymentCreateDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        public decimal Amount { get; set; }

        public CurrencyType Currency { get; set; } = CurrencyType.UZB;

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [MaxLength(255)]
        public string? TransactionId { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
