using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories;
using PRN232.LMS.Services.Common;
using PRN232.LMS.Services.DTOs;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Implementations
{
    public class SemesterService : ISemesterService
    {
        private readonly IGenericRepository<Semester> _semesterRepository;

        public SemesterService(IGenericRepository<Semester> semesterRepository)
        {
            _semesterRepository = semesterRepository;
        }

        public async Task<PagedResult<SemesterResponse>> GetAllSemestersAsync(string? search, string? sort, int page, int pageSize)
        {
            var query = _semesterRepository.GetQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(s => s.SemesterName.ToLower().Contains(lowerSearch));
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = QueryHelper.ApplySorting(query, sort);
            }
            else
            {
                query = query.OrderBy(s => s.SemesterId);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var mappedItems = items.Select(s => new SemesterResponse
            {
                SemesterId = s.SemesterId,
                SemesterName = s.SemesterName,
                StartDate = s.StartDate,
                EndDate = s.EndDate
            }).ToList();

            return new PagedResult<SemesterResponse>
            {
                Items = mappedItems,
                Pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                }
            };
        }

        public async Task<SemesterResponse?> GetSemesterByIdAsync(int semesterId)
        {
            var semester = await _semesterRepository.GetByIdAsync(semesterId);
            if (semester == null) return null;

            return new SemesterResponse
            {
                SemesterId = semester.SemesterId,
                SemesterName = semester.SemesterName,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate
            };
        }

        public async Task<SemesterResponse> CreateSemesterAsync(CreateSemesterRequest request)
        {
            var semester = new Semester
            {
                SemesterName = request.SemesterName,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            await _semesterRepository.AddAsync(semester);
            await _semesterRepository.SaveChangesAsync();

            return new SemesterResponse
            {
                SemesterId = semester.SemesterId,
                SemesterName = semester.SemesterName,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate
            };
        }

        public async Task<SemesterResponse?> UpdateSemesterAsync(int semesterId, UpdateSemesterRequest request)
        {
            var semester = await _semesterRepository.GetByIdAsync(semesterId);
            if (semester == null) return null;

            semester.SemesterName = request.SemesterName;
            semester.StartDate = request.StartDate;
            semester.EndDate = request.EndDate;

            _semesterRepository.Update(semester);
            await _semesterRepository.SaveChangesAsync();

            return new SemesterResponse
            {
                SemesterId = semester.SemesterId,
                SemesterName = semester.SemesterName,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate
            };
        }

        public async Task<bool> DeleteSemesterAsync(int semesterId)
        {
            var semester = await _semesterRepository.GetByIdAsync(semesterId);
            if (semester == null) return false;

            _semesterRepository.Delete(semester);
            await _semesterRepository.SaveChangesAsync();
            return true;
        }
    }
}
