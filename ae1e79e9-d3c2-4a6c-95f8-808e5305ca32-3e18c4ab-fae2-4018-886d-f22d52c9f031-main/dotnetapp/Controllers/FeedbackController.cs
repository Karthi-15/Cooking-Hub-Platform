using dotnetapp.Models;
using dotnetapp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotnetapp.Exceptions;
using Microsoft.AspNetCore.Authorization;


namespace dotnetapp.Controllers
{  
    [ApiController]
    [Route("api/feedback")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly EmailService _emailService;
        public FeedbackController(IFeedbackService feedbackService,EmailService emailService)
        {
            _feedbackService = feedbackService;
            _emailService = emailService;
        }

        
    [HttpPost("contact-user")]
    public async Task<IActionResult> ContactUser([FromBody] Dictionary<string, string> payload)
    {
        if (!payload.ContainsKey("toEmail") || !payload.ContainsKey("username") || !payload.ContainsKey("feedbackText"))
        {
            return BadRequest("Missing required fields.");
        }

        var toEmail = payload["toEmail"];
        var username = payload["username"];
        var feedbackText = payload["feedbackText"];

        _emailService.SendFeedbackEmail(toEmail, username, feedbackText);

        return Ok(new { message = "Email sent successfully" });
    }

        // Get all feedbacks
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetAllFeedbacks()
        {
            try
            {
                var feedbacks = await _feedbackService.GetAllFeedbacks();
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // Get feedback by User ID
        [Authorize(Roles = "User")]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacksByUserId(int userId)
        {
            try
            {
                var feedbacks = await _feedbackService.GetFeedbacksByUserId(userId);
                if (feedbacks == null || !feedbacks.Any())
                    return NotFound("No feedback found for the given user");
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // Add feedback
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<ActionResult> AddFeedback([FromBody] Feedback feedback)
        {
            try
            {
                var result = await _feedbackService.AddFeedback(feedback);
                if (result)
                    return Ok("Feedback added successfully");

                return StatusCode(500, "Failed to add feedback");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // Delete feedback
        [Authorize(Roles="User")]
        [HttpDelete("{feedbackId}")]
        public async Task<ActionResult> DeleteFeedback(int feedbackId)
        {
            try
            {
                var deleted = await _feedbackService.DeleteFeedback(feedbackId);
                if (!deleted)
                    return NotFound("No feedback found with the given ID");

                return Ok("Feedback deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

}

