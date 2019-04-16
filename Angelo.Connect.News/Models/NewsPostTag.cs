using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.News.Models
{
    public class NewsPostTag
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string TagId { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public NewsPost Post { get; set; }
    }
}
