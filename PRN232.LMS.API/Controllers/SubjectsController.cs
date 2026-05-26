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
    [Route("api/subjects")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] string? fields,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var pagedResult = await _subjectService.GetAllSubjectsAsync(search, sort, page, size);

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var filteredData = QueryHelper.FilterFieldsList(pagedResult.Items, fields);
                return Ok(ApiResponse<object>.Ok(filteredData, "Subjects retrieved successfully", pagedResult.Pagination));
            }

            return Ok(ApiResponse<List<SubjectResponse>>.Ok(pagedResult.Items, "Subjects retrieved successfully", pagedResult.Pagination));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? fields)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Subject with ID {id} not found"));
            }

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var filteredData = QueryHelper.FilterFields(subject, fields);
                return Ok(ApiResponse<object>.Ok(filteredData, "Subject retrieved successfully"));
            }

            return Ok(ApiResponse<SubjectResponse>.Ok(subject, "Subject retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSubjectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid subject request data", ModelState));
            }

            var createdSubject = await _subjectService.CreateSubjectAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = createdSubject.SubjectId }, 
                ApiResponse<SubjectResponse>.Ok(createdSubject, "Subject created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSubjectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid subject request data", ModelState));
            }

            var updatedSubject = await _subjectService.UpdateSubjectAsync(id, request);
            if (updatedSubject == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Subject with ID {id} not found"));
            }

            return Ok(ApiResponse<SubjectResponse>.Ok(updatedSubject, "Subject updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _subjectService.DeleteSubjectAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail($"Subject with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.Ok(null!, "Subject deleted successfully"));
        }
    }
}
