using dotnetapp.Data;

public class ClassTool
{
    private readonly ApplicationDbContext _context;

    public ClassTool(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<string> GetAllClassNames()
    {
        return _context.CookingClasses
            .Select(c => c.ClassName)   
            .ToList();
    }
}