using System;
using System.ComponentModel.DataAnnotations;

namespace eticaret.Models
{
    public class Kategori
    {
        [Key]
        public int KategoriNo { get; set; }

        [Required]
        [StringLength(30)]
        public string KategoriAdi { get; set; }
    }
}
