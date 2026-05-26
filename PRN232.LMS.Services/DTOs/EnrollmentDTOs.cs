using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.DTOs
{
    public class EnrollmentResponse
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = null!;

        // Optional expansions
        public StudentResponse? Student { get; set; }
        public CourseResponse? Course { get; set; }
    }

    public class CreateEnrollmentRequest
    {
        [Required(ErrorMessage = "StudentId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "StudentId must be greater than 0.")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "CourseId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "CourseId must be greater than 0.")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "EnrollDate is required.")]
        public DateTime EnrollDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Status is required.")]
        [RegularExpression("^(Active|Inactive|Dropped)$", ErrorMessage = "Status must be 'Active', 'Inactive', or 'Dropped'.")]
        public string Status { get; set; } = null!;
    }

    public class UpdateEnrollmentRequest
    {
        [Required(ErrorMessage = "StudentId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "StudentId must be greater than 0.")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "CourseId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "CourseId must be greater than 0.")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "EnrollDate is required.")]
        public DateTime EnrollDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Status is required.")]
        [RegularExpression("^(Active|Inactive|Dropped)$", ErrorMessage = "Status must be 'Active', 'Inactive', or 'Dropped'.")]
        public string Status { get; set; } = null!;
    }
}

