using Domain.Contracts;
using Domain.Entities.ClinicEntities;
using System;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class RatingSpecification : Specifications<Rating>
    {
        // Get rating by ID
        public RatingSpecification(int id)
            : base(r => r.Id == id)
        {
            AddInclude(r => r.Clinic);
        }
        
        // Get ratings by clinic ID
        public RatingSpecification(int clinicId, bool isClinicId)
            : base(r => r.ClinicId == clinicId)
        {
            AddInclude(r => r.Clinic);
            setOrderByDescending(r => r.CreatedAt);
        }
        
        // Get ratings by user ID
        public RatingSpecification(string userId)
            : base(r => r.UserId == userId)
        {
            AddInclude(r => r.Clinic);
            setOrderByDescending(r => r.CreatedAt);
        }
        
        // Check if user has already rated a clinic
        public RatingSpecification(string userId, int clinicId)
            : base(r => r.UserId == userId && r.ClinicId == clinicId)
        {
        }
        
        // Get ratings with custom criteria
        public RatingSpecification(Expression<Func<Rating, bool>> criteria)
            : base(criteria)
        {
        }
        
        // Get top ratings
        public static RatingSpecification GetTopRatings(int limit)
        {
            var spec = new RatingSpecification(r => true);
            spec.setOrderByDescending(r => r.RatingValue);
            spec.ApplyPagination(1, limit);
            spec.AddInclude(r => r.Clinic);
            return spec;
        }
        
        // Get latest ratings
        public static RatingSpecification GetLatestRatings(int limit)
        {
            var spec = new RatingSpecification(r => true);
            spec.setOrderByDescending(r => r.CreatedAt);
            spec.ApplyPagination(1, limit);
            spec.AddInclude(r => r.Clinic);
            return spec;
        }
    }
} 