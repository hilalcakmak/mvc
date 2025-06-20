using System;
using System.ComponentModel.DataAnnotations;

namespace eticaret.Models
{
    public class Musteri
    {
        [Key]
        public int MusteriNo { get; set; }

        [Required]
        [StringLength(50)]
        public string Ad { get; set; }

        [Required]
        [StringLength(50)]
        public string Soyad { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
            ErrorMessage = "Parola en az 1 büyük harf, 1 küçük harf ve 1 rakam içermeli ve en az 6 karakter olmalıdır.")]
        public string Parola { get; set; }

        [Phone]
        [Display(Name = "Cep Telefon Numarası")]
        public string? Telefon { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi")]
        public DateTime? DogumTarihi { get; set; }

        [Display(Name = "Cinsiyet")]
        public string? Cinsiyet { get; set; } // "Kadın", "Erkek", "Belirtmek istemiyorum" gibi opsiyonlar için
    }
}
