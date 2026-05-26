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
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] string? expand,
            [FromQuery] string? fields,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var pagedResult = await _enrollmentService.GetAllEnrollmentsAsync(search, sort, expand, page, size);

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var filteredData = QueryHelper.FilterFieldsList(pagedResult.Items, fields);
                return Ok(ApiResponse<object>.Ok(filteredData, "Enrollments retrieved successfully", pagedResult.Pagination));
            }

            return Ok(ApiResponse<List<EnrollmentResponse>>.Ok(pagedResult.Items, "Enrollments retrieved successfully", pagedResult.Pagination));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? expand, [FromQuery] string? fields)
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id, expand);
            if (enrollment == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Enrollment with ID {id} not found"));
            }

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var filteredData = QueryHelper.FilterFields(enrollment, fields);
                return Ok(ApiResponse<object>.Ok(filteredData, "Enrollment retrieved successfully"));
            }

            return Ok(ApiResponse<EnrollmentResponse>.Ok(enrollment, "Enrollment retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEnrollmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid enrollment request data", ModelState));
            }

            var createdEnrollment = await _enrollmentService.CreateEnrollmentAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = createdEnrollment.EnrollmentId }, 
                ApiResponse<EnrollmentResponse>.Ok(createdEnrollment, "Enrollment created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEnrollmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid enrollment request data", ModelState));
            }

            var updatedEnrollment = await _enrollmentService.UpdateEnrollmentAsync(id, request);
            if (updatedEnrollment == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Enrollment with ID {id} not found"));
            }

            return Ok(ApiResponse<EnrollmentResponse>.Ok(updatedEnrollment, "Enrollment updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _enrollmentService.DeleteEnrollmentAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail($"Enrollment with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.Ok(null!, "Enrollment deleted successfully"));
        }
    }
}

