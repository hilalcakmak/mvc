using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eticaret.Models;

namespace eticaret.Models
{
    public class SiparisDetay
    {
        [Key]
        public int DetayNo { get; set; }

        [ForeignKey("Siparis")]
        public int SiparisNo { get; set; }

        [ForeignKey("Urun")]
        public int UrunNo { get; set; }

        [Required]
        public int Adet { get; set; }

        [Required]
        [StringLength(10)]
        public string Beden { get; set; } // Sipariş sırasında seçilen beden

        [Column(TypeName = "decimal(18,2)")]
        public decimal BirimFiyat { get; set; }

        [Required]
        [StringLength(50)]
        public string TeslimDurumu { get; set; } // Örnek: "Teslim edildi"

        public DateTime? TeslimTarihi { get; set; }

        [StringLength(100)]
        public string? TeslimAlan { get; set; }

        public virtual Siparis Siparis { get; set; }
        public virtual Urun Urun { get; set; }
    }
}
