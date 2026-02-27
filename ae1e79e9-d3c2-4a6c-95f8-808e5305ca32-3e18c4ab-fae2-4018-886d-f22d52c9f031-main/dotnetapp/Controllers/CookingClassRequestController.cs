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
    [Route("api/cooking-class-request")]
    public class CookingClassRequestController : ControllerBase
    {
        private readonly ICookingClassRequestService _cookingClassRequestService;

        public CookingClassRequestController(ICookingClassRequestService cookingClassRequestService)
        {
            _cookingClassRequestService = cookingClassRequestService;
        }

        // Get all cooking class requests
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CookingClassRequest>>> GetAllCookingClassRequests()
        {
            try
            {
                var requests = await _cookingClassRequestService.GetAllCookingClassRequests();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Get cooking class requests by User ID
        [Authorize(Roles = "User")]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CookingClassRequest>>> GetCookingClassRequestsByUserId(int userId)
        {
            try
            {
                var requests = await _cookingClassRequestService.GetCookingClassRequestsByUserId(userId);
                if (requests == null || !requests.Any())
                    return NotFound("No cooking class requests found for the given user");

                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Add a cooking class request
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<ActionResult> AddCookingClassRequest([FromBody] CookingClassRequest request)
        {
            try
            {
                var result = await _cookingClassRequestService.AddCookingClassRequest(request);
                if (result)
                    return Ok("Cooking class request added successfully");

                return StatusCode(500, "Failed to add cooking class request");
            }
            catch (CookingClassException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Update a cooking class request
        [Authorize(Roles = "User , Admin")]
        [HttpPut("{requestId}")]
        public async Task<ActionResult> UpdateCookingClassRequest(int requestId, [FromBody] CookingClassRequest request)
        {
            try
            {
                var updated = await _cookingClassRequestService.UpdateCookingClassRequest(requestId, request);
                if (!updated)
                    return NotFound("No cooking class request found with the given ID");

                return Ok("Cooking class request updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Delete a cooking class request
        [Authorize(Roles = "User")]
        [HttpDelete("{requestId}")]
        public async Task<ActionResult> DeleteCookingClassRequest(int requestId)
        {
            try
            {
                var deleted = await _cookingClassRequestService.DeleteCookingClassRequest(requestId);
                if (!deleted)
                    return NotFound("No cooking class request found with the given ID");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
