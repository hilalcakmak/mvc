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
    public class SiparisController : Controller
    {
        private readonly AppDbContext _context;
        public SiparisController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet] // yazmasak da default get gelir
        public IActionResult Index()
        {
            var listele = _context.Siparisler
                  .Include(s => s.Musteri) // bak burası önemli
                  .ToList();
            return View(listele);
        }
        public IActionResult Siparislerim()
        {
            // Oturumdaki mail ile müşteriyi bul
            var email = HttpContext.Session.GetString("KullaniciEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            var musteri = _context.Musteriler.FirstOrDefault(m => m.Email == email);
            if (musteri == null)
                return RedirectToAction("Login", "Account");

            // Bu müşterinin tüm siparişleri (ve detayları)
            var siparisler = _context.Siparisler
                .Where(s => s.MusteriNo == musteri.MusteriNo)
                .OrderByDescending(s => s.Tarih)
                .Select(s => new
                {
                    s.SiparisNo,
                    s.Tarih,
                    s.Durum,
                    s.ToplamTutar,
                    Detaylar = _context.SiparisDetaylari
                        .Where(d => d.SiparisNo == s.SiparisNo)
                        .Select(d => new {
                            d.Urun.UrunAdi,
                            d.Adet,
                            d.BirimFiyat,
                            d.Beden
                        }).ToList()
                }).ToList();

            return View(siparisler);
        }


    }
}

