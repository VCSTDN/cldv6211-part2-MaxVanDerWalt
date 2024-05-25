using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Models
{
    public class MyWorkModel
    {
        [Key]
        public int WorkID { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public decimal Price { get; set; }
        public bool InStock { get; set; }
        public string? ImagePath { get; set; }

        [ForeignKey("CategoryModel")]
        public int CategoryID { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        // Navigation properties
        public virtual CategoryModel? Product { get; set; }

    }
}
