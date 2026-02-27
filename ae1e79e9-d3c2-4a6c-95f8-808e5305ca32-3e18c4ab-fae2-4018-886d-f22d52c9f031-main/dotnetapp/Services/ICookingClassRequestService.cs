using System.Collections.Generic;
using System.Threading.Tasks;
using dotnetapp.Dto;
using dotnetapp.Models;

namespace dotnetapp.Services
{
    public interface ICookingClassRequestService
    {
       
        Task<IEnumerable<CookingClassRequestDto>> GetAllCookingClassRequests();

       
        Task<IEnumerable<CookingClassRequest>> GetCookingClassRequestsByUserId(int userId);

        
        Task<bool> AddCookingClassRequest(CookingClassRequest request);

        
        Task<bool> UpdateCookingClassRequest(int requestId, CookingClassRequest request);

       
        Task<bool> DeleteCookingClassRequest(int requestId);
    }
}