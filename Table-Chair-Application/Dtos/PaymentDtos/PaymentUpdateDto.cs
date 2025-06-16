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
        public PaymentStatus Status { get; set; }

        [MaxLength(255)]
        public string? TransactionId { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
