using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetapp.Dto
{
    public class FeedbackDto
    {
    public int FeedbackId { get; set; }
    public int UserId { get; set; }
    public string? Username { get; set; }
    public string? Email{get;set;}
    public string FeedbackText { get; set; }
    public DateTime Date { get; set; }
    }
}
