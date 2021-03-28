using FinalProiect.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Incercare2Proiect.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Numele este obligatoriu")]
        [StringLength(100, ErrorMessage = "Numele nu poate avea mai mult de 20 caractere")]
        public string Nume { get; set; }
        [Required(ErrorMessage = "Pretul este obligatoriu")]
        public int Pret { get; set; }
        [StringLength(1000, ErrorMessage = "Descrierea nu poate avea mai mult de 1000 caractere")]
        public string Descriere { get; set; }
        public string ImageName { get; set; }

        public int Suma { get; set; }
        public double Medie { get; set; }
        public float Numar { get; set; }

        public bool Accept { get; set; }

        public int CategoryId { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }


        public virtual Category Category { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public IEnumerable<SelectListItem> Categ { get; set; }


        public int? OrderBy { get; set; }
    }
}