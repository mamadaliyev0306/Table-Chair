using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.CreateDtos
{
    public class CreateShippingAddressDto
    {
        public string? FullName { get; set; }
        public string AddressLine { get; set; } = default!;

        public string City { get; set; } = default!;

        public string ZipCode { get; set; } = default!;
    }

}
