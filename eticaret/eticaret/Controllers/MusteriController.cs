using System.Linq;
using System.Threading.Tasks;
using eticaret.Data;
using eticaret.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eticaret.Controllers
{
    public class MusteriController : Controller
    {
        private readonly AppDbContext _context;

        public MusteriController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string cinsiyetFilter)
        {
            // Tüm cinsiyetleri çek ve SelectListItem listesine dönüştür
            var cinsiyetList = await _context.Musteriler
                .Select(m => m.Cinsiyet)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            ViewBag.CinsiyetList = cinsiyetList.Select(c => new SelectListItem
            {
                Text = c,
                Value = c
            }).ToList();

            ViewBag.CinsiyetFilter = cinsiyetFilter;
            ViewBag.SearchString = searchString;

            var musteriQuery = _context.Musteriler.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                musteriQuery = musteriQuery.Where(m =>
                    m.Ad.ToLower().StartsWith(searchString.ToLower()) ||
                    m.Soyad.ToLower().StartsWith(searchString.ToLower()));
            }

            if (!string.IsNullOrEmpty(cinsiyetFilter))
            {
                musteriQuery = musteriQuery.Where(m => m.Cinsiyet == cinsiyetFilter);
            }

            musteriQuery = musteriQuery.OrderBy(m => m.Ad);

            return View(await musteriQuery.ToListAsync());
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Musteri musteri)
        {
            _context.Add(musteri);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var update = _context.Musteriler.Find(id);
            return View(update);
        }

        [HttpPost]
        public IActionResult Edit(int id, Musteri musteri)
        {
            _context.Update(musteri);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var delete = _context.Musteriler.Find(id);
            _context.Remove(delete);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
