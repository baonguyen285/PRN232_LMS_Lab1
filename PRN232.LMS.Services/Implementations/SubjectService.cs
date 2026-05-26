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
    public class SubjectService : ISubjectService
    {
        private readonly IGenericRepository<Subject> _subjectRepository;

        public SubjectService(IGenericRepository<Subject> subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<PagedResult<SubjectResponse>> GetAllSubjectsAsync(string? search, string? sort, int page, int pageSize)
        {
            var query = _subjectRepository.GetQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(s => s.SubjectName.ToLower().Contains(lowerSearch) || s.SubjectCode.ToLower().Contains(lowerSearch));
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = QueryHelper.ApplySorting(query, sort);
            }
            else
            {
                query = query.OrderBy(s => s.SubjectId);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var mappedItems = items.Select(s => new SubjectResponse
            {
                SubjectId = s.SubjectId,
                SubjectCode = s.SubjectCode,
                SubjectName = s.SubjectName,
                Credit = s.Credit
            }).ToList();

            return new PagedResult<SubjectResponse>
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

        public async Task<SubjectResponse?> GetSubjectByIdAsync(int subjectId)
        {
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null) return null;

            return new SubjectResponse
            {
                SubjectId = subject.SubjectId,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Credit = subject.Credit
            };
        }

        public async Task<SubjectResponse> CreateSubjectAsync(CreateSubjectRequest request)
        {
            var subject = new Subject
            {
                SubjectCode = request.SubjectCode,
                SubjectName = request.SubjectName,
                Credit = request.Credit
            };

            await _subjectRepository.AddAsync(subject);
            await _subjectRepository.SaveChangesAsync();

            return new SubjectResponse
            {
                SubjectId = subject.SubjectId,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Credit = subject.Credit
            };
        }

        public async Task<SubjectResponse?> UpdateSubjectAsync(int subjectId, UpdateSubjectRequest request)
        {
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null) return null;

            subject.SubjectCode = request.SubjectCode;
            subject.SubjectName = request.SubjectName;
            subject.Credit = request.Credit;

            _subjectRepository.Update(subject);
            await _subjectRepository.SaveChangesAsync();

            return new SubjectResponse
            {
                SubjectId = subject.SubjectId,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Credit = subject.Credit
            };
        }

        public async Task<bool> DeleteSubjectAsync(int subjectId)
        {
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null) return false;

            _subjectRepository.Delete(subject);
            await _subjectRepository.SaveChangesAsync();
            return true;
        }
    }
}
