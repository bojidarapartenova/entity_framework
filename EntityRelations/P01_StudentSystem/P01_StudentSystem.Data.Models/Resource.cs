﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Enumerations;
    using Microsoft.EntityFrameworkCore;

    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Unicode(false)]
        public string Url { get; set; }

        public ResourceTypeEnums ResourceType { get; set; }


        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public virtual Course Course { get; set; } = null!;

    }
}
