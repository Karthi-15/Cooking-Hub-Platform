using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetapp.Dto
{
    public class CookingClassRequestDto
    {
    public int CookingClassRequestId { get; set; }
    public int UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public int CookingClassId { get; set; }
    public CookingClassDto? CookingClass { get; set; }
    public string RequestDate { get; set; }
    public string Status { get; set; }
    public string DietaryPreferences { get; set; }
    public string CookingGoals { get; set; }
    public string? Comments { get; set; }
    }
}
