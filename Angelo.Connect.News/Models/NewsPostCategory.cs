using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.News.Models
{
    public class NewsPostCategory
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string CategoryId { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public NewsPost Post { get; set; }

        public NewsCategory Category { get; set; }
    }
}
