using System.Collections.Generic;
using System.Threading.Tasks;
using dotnetapp.Models;

namespace dotnetapp.Services
{
    public interface ICookingClassService
    {
        Task<IEnumerable<CookingClass>> GetAllCookingClasses();

        Task<CookingClass> GetCookingClassById(int cookingId);

        Task<bool> AddCookingClass(CookingClass cooking);

        Task<bool> UpdateCookingClass(int cookingId, CookingClass cooking);

        Task<bool> DeleteCookingClass(int cookingId);
    }
}