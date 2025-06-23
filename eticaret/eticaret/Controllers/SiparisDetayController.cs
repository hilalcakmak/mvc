using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eticaret.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eticaret.Controllers
{
    public class SiparisDetayController : Controller
    {
        private readonly AppDbContext _context;
        public SiparisDetayController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var detaylar = _context.SiparisDetaylari
                .Include(sd => sd.Siparis)   // Sipariş ile ilişkili
                .Include(sd => sd.Urun)      // Ürün ile ilişkili
                .ToList();
            return View(detaylar);
        }
    }
}

