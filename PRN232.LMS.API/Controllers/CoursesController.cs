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
    [Route("api/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
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
            var pagedResult = await _courseService.GetAllCoursesAsync(search, sort, expand, page, size);

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var filteredData = QueryHelper.FilterFieldsList(pagedResult.Items, fields);
                return Ok(ApiResponse<object>.Ok(filteredData, "Courses retrieved successfully", pagedResult.Pagination));
            }

            return Ok(ApiResponse<List<CourseResponse>>.Ok(pagedResult.Items, "Courses retrieved successfully", pagedResult.Pagination));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? expand, [FromQuery] string? fields)
        {
            var course = await _courseService.GetCourseByIdAsync(id, expand);
            if (course == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Course with ID {id} not found"));
            }

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var filteredData = QueryHelper.FilterFields(course, fields);
                return Ok(ApiResponse<object>.Ok(filteredData, "Course retrieved successfully"));
            }

            return Ok(ApiResponse<CourseResponse>.Ok(course, "Course retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid course request data", ModelState));
            }

            var createdCourse = await _courseService.CreateCourseAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = createdCourse.CourseId }, 
                ApiResponse<CourseResponse>.Ok(createdCourse, "Course created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid course request data", ModelState));
            }

            var updatedCourse = await _courseService.UpdateCourseAsync(id, request);
            if (updatedCourse == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Course with ID {id} not found"));
            }

            return Ok(ApiResponse<CourseResponse>.Ok(updatedCourse, "Course updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _courseService.DeleteCourseAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail($"Course with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.Ok(null!, "Course deleted successfully"));
        }

        // Additional teacher requirement: GET /api/courses/{courseId}/enrollments
        [HttpGet("{courseId}/enrollments")]
        public async Task<IActionResult> GetEnrollmentsByCourseId(int courseId, [FromQuery] string? expand, [FromQuery] string? fields)
        {
            var enrollments = await _courseService.GetEnrollmentsByCourseIdAsync(courseId, expand);
            if (enrollments == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Course with ID {courseId} not found"));
            }

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var filteredData = QueryHelper.FilterFieldsList(enrollments, fields);
                return Ok(ApiResponse<object>.Ok(filteredData, $"Enrollments for Course {courseId} retrieved successfully"));
            }

            return Ok(ApiResponse<IEnumerable<EnrollmentResponse>>.Ok(enrollments, $"Enrollments for Course {courseId} retrieved successfully"));
        }
    }
}

