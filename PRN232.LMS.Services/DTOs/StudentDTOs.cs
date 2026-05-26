using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.DTOs
{
    public class StudentResponse
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }

    public class CreateStudentRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "FullName is required and cannot be empty.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "FullName must be between 2 and 100 characters.")]
        public string FullName { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required and cannot be empty.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "DateOfBirth is required.")]
        public DateTime DateOfBirth { get; set; }
    }

    public class UpdateStudentRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "FullName is required and cannot be empty.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "FullName must be between 2 and 100 characters.")]
        public string FullName { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required and cannot be empty.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "DateOfBirth is required.")]
        public DateTime DateOfBirth { get; set; }
    }
}

