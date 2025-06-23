using Microsoft.AspNetCore.Mvc;
using eticaret.Data;

public class KategoriMenuViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    public KategoriMenuViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        var kategoriler = _context.Kategoriler.ToList();
        return View(kategoriler);
    }
}
