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
    [Route("api/semesters")]
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _semesterService;

        public SemestersController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] string? fields,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var pagedResult = await _semesterService.GetAllSemestersAsync(search, sort, page, size);

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var filteredData = QueryHelper.FilterFieldsList(pagedResult.Items, fields);
                return Ok(ApiResponse<object>.Ok(filteredData, "Semesters retrieved successfully", pagedResult.Pagination));
            }

            return Ok(ApiResponse<List<SemesterResponse>>.Ok(pagedResult.Items, "Semesters retrieved successfully", pagedResult.Pagination));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? fields)
        {
            var semester = await _semesterService.GetSemesterByIdAsync(id);
            if (semester == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Semester with ID {id} not found"));
            }

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var filteredData = QueryHelper.FilterFields(semester, fields);
                return Ok(ApiResponse<object>.Ok(filteredData, "Semester retrieved successfully"));
            }

            return Ok(ApiResponse<SemesterResponse>.Ok(semester, "Semester retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSemesterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid semester request data", ModelState));
            }

            var createdSemester = await _semesterService.CreateSemesterAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = createdSemester.SemesterId }, 
                ApiResponse<SemesterResponse>.Ok(createdSemester, "Semester created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSemesterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid semester request data", ModelState));
            }

            var updatedSemester = await _semesterService.UpdateSemesterAsync(id, request);
            if (updatedSemester == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Semester with ID {id} not found"));
            }

            return Ok(ApiResponse<SemesterResponse>.Ok(updatedSemester, "Semester updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _semesterService.DeleteSemesterAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail($"Semester with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.Ok(null!, "Semester deleted successfully"));
        }
    }
}
