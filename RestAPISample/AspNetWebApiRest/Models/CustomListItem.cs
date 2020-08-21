using System.ComponentModel.DataAnnotations;

namespace AspNetWebApiRest.Models
{
    public class CustomListItem : NewCustomListItem
    {
        [Required]
        public int Id { get; set; }        
    }

    public class NewCustomListItem
    {
        public string Text { get; set; }
    }
}