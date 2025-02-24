using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Security.Permissions;
    using Enumerations;
    using Microsoft.EntityFrameworkCore;

    public class Homework
    {
        [Key]
        public int HomeworkId { get; set; }

        [Required]
        [Unicode(false)]
        public string Content { get; set; }

        public ContentTypeEnums ContentType { get; set; }

        public DateTime SubmissionTime { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        public virtual Student Student { get; set; } = null!;

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public virtual Course Course { get; set; } = null!;
    }
}
