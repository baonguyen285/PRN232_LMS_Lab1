using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models;
using PRN232.LMS.Services.Common;
using PRN232.LMS.Services.DTOs;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.API.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] string? fields,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var pagedResult = await _studentService.GetAllStudentsAsync(search, sort, page, size);

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var filteredData = QueryHelper.FilterFieldsList(pagedResult.Items, fields);
                return Ok(ApiResponse<object>.Ok(filteredData, "Students retrieved successfully", pagedResult.Pagination));
            }

            return Ok(ApiResponse<List<StudentResponse>>.Ok(pagedResult.Items, "Students retrieved successfully", pagedResult.Pagination));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? fields)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Student with ID {id} not found"));
            }

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var filteredData = QueryHelper.FilterFields(student, fields);
                return Ok(ApiResponse<object>.Ok(filteredData, "Student retrieved successfully"));
            }

            return Ok(ApiResponse<StudentResponse>.Ok(student, "Student retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid student request data", ModelState));
            }

            var createdStudent = await _studentService.CreateStudentAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = createdStudent.StudentId }, 
                ApiResponse<StudentResponse>.Ok(createdStudent, "Student created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid student request data", ModelState));
            }

            var updatedStudent = await _studentService.UpdateStudentAsync(id, request);
            if (updatedStudent == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Student with ID {id} not found"));
            }

            return Ok(ApiResponse<StudentResponse>.Ok(updatedStudent, "Student updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail($"Student with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.Ok(null!, "Student deleted successfully"));
        }
    }
}

