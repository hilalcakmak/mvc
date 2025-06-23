//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using eticaret.Data;
//using Microsoft.AspNetCore.Mvc;

//// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace eticaret.Controllers
//{
//    public class KategoriMenuViewComponent : ViewComponent
//{
//    private readonly AppDbContext _context;
//    public KategoriMenuViewComponent(AppDbContext context)
//    {
//        _context = context;
//    }

//    public IViewComponentResult Invoke()
//    {
//        var kategoriler = _context.Kategoriler.ToList();
//        return View(kategoriler);   // Model olarak gönderiyoruz
//    }
//}

//}

