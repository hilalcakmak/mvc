using Microsoft.AspNetCore.Mvc;
using eticaret.Models; 
using eticaret.Data;
namespace eticaret.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciEmail")))
            {
                // Zaten giriş yaptıysa ana sayfaya at
                return RedirectToAction("Index", "Home");
            }
            else if (HttpContext.Session.GetString("Admin") == "true")
            {
                // Zaten admin girişi yapılmışsa ana sayfaya atabilirsin
                return RedirectToAction("Index", "Home");
                // veya varsa admin paneline yönlendir
                // return RedirectToAction("Dashboard", "Admin");
            }
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Musteri musteri)
        {
            if (ModelState.IsValid)
            {
                _context.Musteriler.Add(musteri);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(musteri);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciEmail")))
            {
                // Zaten giriş yaptıysa ana sayfaya at
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string Email, string Parola)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Parola))
            {
                ModelState.AddModelError("", "E-posta ve parola zorunludur.");
                return View();
            }

            // Müşteri tablosunda var mı kontrol
            var musteri = _context.Musteriler.FirstOrDefault(m => m.Email == Email && m.Parola == Parola);
            if (musteri != null)
            {
                // Basit bir session ile giriş gösterebiliriz (ileride Identity ile geliştirilebilir)
                HttpContext.Session.SetString("KullaniciEmail", musteri.Email);
                HttpContext.Session.SetString("KullaniciAd", musteri.Ad);

                // Giriş başarılıysa anasayfaya gönder
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "E-posta veya parola hatalı.");
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult AdminLogin()
        {
            if (HttpContext.Session.GetString("Admin") == "true")
            {
                // Zaten admin girişi yapılmışsa ana sayfaya atabilirsin
                return RedirectToAction("Index", "Home");
                // veya varsa admin paneline yönlendir
                // return RedirectToAction("Dashboard", "Admin");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdminLogin(string Email, string Parola)
        {
            const string adminEmail = "admin@admin.com";
            const string adminPassword = "Admin123"; 

            if (Email == adminEmail && Parola == adminPassword)
            {
                HttpContext.Session.SetString("Admin", "true");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Admin e-posta veya parola hatalı.");
                return View();
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Tüm sessionları temizle
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Profile()
        {
            var email = HttpContext.Session.GetString("KullaniciEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var musteri = _context.Musteriler.FirstOrDefault(m => m.Email == email);
            if (musteri == null)
                return RedirectToAction("Login");

            return View(musteri);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(Musteri model, string? yeniParola, string? yeniParolaTekrar)
        {
            var email = HttpContext.Session.GetString("KullaniciEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var musteri = _context.Musteriler.FirstOrDefault(m => m.Email == email);
            if (musteri == null)
                return RedirectToAction("Login");

            // Parola değiştirilmek istenirse kontrol
            if (!string.IsNullOrEmpty(yeniParola) || !string.IsNullOrEmpty(yeniParolaTekrar))
            {
                if (string.IsNullOrEmpty(yeniParola) || yeniParola.Length < 6 || !yeniParola.Any(char.IsDigit) || !yeniParola.Any(char.IsLower) || !yeniParola.Any(char.IsUpper))
                {
                    ModelState.AddModelError("yeniParola", "Parola en az 6 karakter, büyük harf, küçük harf ve rakam içermeli.");
                    return View(model);
                }
                if (yeniParola != yeniParolaTekrar)
                {
                    ModelState.AddModelError("yeniParolaTekrar", "Parolalar eşleşmiyor.");
                    return View(model);
                }
                musteri.Parola = yeniParola;
            }

            // Diğer bilgileri güncelle
            musteri.Ad = model.Ad;
            musteri.Soyad = model.Soyad;
            musteri.Telefon = model.Telefon;
            musteri.DogumTarihi = model.DogumTarihi;
            musteri.Cinsiyet = model.Cinsiyet;

            _context.SaveChanges();

            ViewBag.Basarili = true;
            return View(musteri);
        }


    }
}

