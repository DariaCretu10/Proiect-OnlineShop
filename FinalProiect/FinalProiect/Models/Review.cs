using FinalProiect.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Incercare2Proiect.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        [Required]
        public string Continut { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int Nota { get; set; }

        public virtual Product Product { get; set; }


    }
}