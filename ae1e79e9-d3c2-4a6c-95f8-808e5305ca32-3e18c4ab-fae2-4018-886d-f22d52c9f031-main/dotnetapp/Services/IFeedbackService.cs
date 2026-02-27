using System.Collections.Generic;
using System.Threading.Tasks;
using dotnetapp.Dto;
using dotnetapp.Models;

namespace dotnetapp.Services
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackDto>> GetAllFeedbacks();
        Task<IEnumerable<Feedback>> GetFeedbacksByUserId(int userId);
        Task<bool> AddFeedback(Feedback feedback);
        Task<bool> DeleteFeedback(int feedbackId);
    }
}
