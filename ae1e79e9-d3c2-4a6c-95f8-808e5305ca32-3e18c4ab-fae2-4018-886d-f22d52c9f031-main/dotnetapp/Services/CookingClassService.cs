using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using dotnetapp.Data;
using dotnetapp.Exceptions;
using dotnetapp.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnetapp.Services
{
    public class CookingClassService : ICookingClassService
    {
        private readonly ApplicationDbContext _context;

        public CookingClassService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Get all cooking classes
        public async Task<IEnumerable<CookingClass>> GetAllCookingClasses()
        {
            return await _context.CookingClasses.ToListAsync();
        }

        // 2. Get a cooking class by ID
        public async Task<CookingClass> GetCookingClassById(int cookingId)
        {
            return await _context.CookingClasses.FindAsync(cookingId);
        }

        // 3. Add a new cooking class
        public async Task<bool> AddCookingClass(CookingClass cooking)
        {
            var existingClass = await _context.CookingClasses.FirstOrDefaultAsync(c => c.ClassName == cooking.ClassName);
            if (existingClass != null)
                throw new CookingClassException("Cooking class with the same name already exists");

            _context.CookingClasses.Add(cooking);
            await _context.SaveChangesAsync();
            return true;
        }

        // 4. Update a cooking class
        public async Task<bool> UpdateCookingClass(int cookingId, CookingClass cooking)
        {
            var existingClass = await _context.CookingClasses.FindAsync(cookingId);
            if (existingClass == null)
                return false;

            var duplicateClass = await _context.CookingClasses.FirstOrDefaultAsync(c => c.ClassName == cooking.ClassName && c.CookingClassId != cookingId);
            if (duplicateClass != null)
                throw new CookingClassException("Cooking class with the same name already exists");

            existingClass.ClassName = cooking.ClassName;
            existingClass.CuisineType = cooking.CuisineType;
            existingClass.ChefName = cooking.ChefName;
            existingClass.Location = cooking.Location;
            existingClass.DurationInHours = cooking.DurationInHours;
            existingClass.Fee = cooking.Fee;
            existingClass.IngredientsProvided = cooking.IngredientsProvided;
            existingClass.SkillLevel = cooking.SkillLevel;
            existingClass.SpecialRequirements = cooking.SpecialRequirements;

            await _context.SaveChangesAsync();
            return true;
        }

        // 5. Delete a cooking class
        public async Task<bool> DeleteCookingClass(int cookingId)
        {
            var cookingClass = await _context.CookingClasses.FindAsync(cookingId);
            if (cookingClass == null)
                return false;

            var referencedRequest = await _context.CookingClassRequests.AnyAsync(cr => cr.CookingClassId == cookingId);
            if (referencedRequest)
                throw new CookingClassException("Cooking class cannot be deleted as it is referenced in a request");

            _context.CookingClasses.Remove(cookingClass);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
