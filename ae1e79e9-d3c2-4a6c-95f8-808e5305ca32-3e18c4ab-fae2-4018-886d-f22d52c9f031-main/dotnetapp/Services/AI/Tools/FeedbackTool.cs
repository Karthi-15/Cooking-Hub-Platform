
using dotnetapp.Data;
using Microsoft.EntityFrameworkCore;

public class FeedbackTool
{
    private readonly ApplicationDbContext _context;

    public FeedbackTool(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<string> GetAllFeedbackTexts()
    {
        return _context.Feedbacks
            .Select(f => f.FeedbackText)  
            .ToList();
    }
}