﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Entity.Models
{
    [Table("AboutInfo",Schema ="Models")]
    public class AboutInfo
    {
        public int Id { get; set; }
        [Required] public string? Content { get; set; }
        public string? ImageUrl { get; set; }
    }
}
