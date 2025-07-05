using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos
{
    public class DateRangeDto
    {
        private DateTime? _startDate;
        private DateTime? _endDate;

        [FromQuery(Name = "startDate")]
        public DateTime? StartDate
        {
            get => _startDate;
            set => _startDate = value.HasValue ?
                DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
        }

        [FromQuery(Name = "endDate")]
        public DateTime? EndDate
        {
            get => _endDate;
            set => _endDate = value.HasValue ?
                DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
        }
    }

}
