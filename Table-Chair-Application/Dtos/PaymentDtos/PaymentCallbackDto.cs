using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos.PaymentDtos
{
    public class PaymentCallbackDto
    {
        [Required]
        public int PaymentId { get; set; }

        [Required]
        public bool Success { get; set; }

        public string? TransactionId { get; set; }

        [MaxLength(500)]
        public string? Message { get; set; }
    }

}
