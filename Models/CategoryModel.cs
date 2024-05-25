using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Models
{
    public class CategoryModel
    {

        [Key]
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }

        // Link to my work
        public virtual ICollection<MyWorkModel> MyWork { get; set; }
    }
}

