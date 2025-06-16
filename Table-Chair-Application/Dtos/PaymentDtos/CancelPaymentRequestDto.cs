using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.PaymentDtos
{
    public class CancelPaymentRequestDto
    {
        [Required]
        public string Reason { get; set; } = null!;
    }
}
