using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetapp.Data;
using dotnetapp.Exceptions;
using dotnetapp.Models;
using Microsoft.EntityFrameworkCore;
using dotnetapp.Dto;

namespace dotnetapp.Services
{
    public class CookingClassRequestService : ICookingClassRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;


        public CookingClassRequestService(ApplicationDbContext context,EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Get all cooking class requests including related CookingClass & User details
       public async Task<IEnumerable<CookingClassRequestDto>> GetAllCookingClassRequests()
{
    return await _context.CookingClassRequests
        .Include(cr => cr.CookingClass)
        .Include(cr => cr.User)
        .Select(cr => new CookingClassRequestDto
        {
            CookingClassRequestId = cr.CookingClassRequestId,
            UserId = cr.UserId,
            Username = cr.User.Username,
            Email = cr.User.Email,
            Mobile = cr.User.MobileNumber,
            CookingClassId = cr.CookingClassId,
            CookingClass = cr.CookingClass != null ? new CookingClassDto
            {
                ClassName = cr.CookingClass.ClassName,
                CuisineType = cr.CookingClass.CuisineType,
                ChefName = cr.CookingClass.ChefName,
                Location = cr.CookingClass.Location,
                DurationInHours = cr.CookingClass.DurationInHours,
                Fee = cr.CookingClass.Fee,
                IngredientsProvided = cr.CookingClass.IngredientsProvided,
                SkillLevel = cr.CookingClass.SkillLevel,
                SpecialRequirements = cr.CookingClass.SpecialRequirements
            } : null,
            RequestDate = cr.RequestDate,
            Status = cr.Status,
            DietaryPreferences = cr.DietaryPreferences,
            CookingGoals = cr.CookingGoals,
            Comments = cr.Comments
        })
        .ToListAsync();
}


        // Get cooking class requests for a specific user
        public async Task<IEnumerable<CookingClassRequest>> GetCookingClassRequestsByUserId(int userId)
        {
            return await _context.CookingClassRequests
                .Include(cr => cr.CookingClass)
                .Where(cr => cr.UserId == userId)
                .ToListAsync();
        }

        // Add a new cooking class request
        public async Task<bool> AddCookingClassRequest(CookingClassRequest request)
        {
            var existingRequest = await _context.CookingClassRequests.FirstOrDefaultAsync(cr => 
                cr.UserId == request.UserId && cr.CookingClassId == request.CookingClassId);

            if (existingRequest != null)
                throw new CookingClassException("User already requested this cooking class");

            _context.CookingClassRequests.Add(request);
            await _context.SaveChangesAsync();
            return true;
        }

        // Update a cooking class request
        public async Task<bool> UpdateCookingClassRequest(int requestId, CookingClassRequest request)
        {
            var existingRequest = await _context.CookingClassRequests.FindAsync(requestId);
            if (existingRequest == null)
                return false;

            existingRequest.Status = request.Status;
            existingRequest.DietaryPreferences = request.DietaryPreferences;
            existingRequest.CookingGoals = request.CookingGoals;
            existingRequest.Comments = request.Comments;

            await _context.SaveChangesAsync();
            
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == request.UserId);
            if (user != null)
            {
            await _emailService.SendDecisionEmail(user.Email, request.Status);
            }

            return true;
        }

        // Delete a cooking class request
        public async Task<bool> DeleteCookingClassRequest(int requestId)
        {
            var request = await _context.CookingClassRequests.FindAsync(requestId);
            if (request == null)
                return false;

            _context.CookingClassRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
