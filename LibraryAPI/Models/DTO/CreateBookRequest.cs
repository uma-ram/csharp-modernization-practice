using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models.DTO;

public class CreateBookRequest
{
    [Required(ErrorMessage="Title is required")]
    [StringLength(200, ErrorMessage="Title cannot exceed 200 characters", MinimumLength =1)]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage="Author is required")]
    [StringLength(100, ErrorMessage="Author cannot exceed 100 characters", MinimumLength =1)]
    public string Author { get; set; } = string.Empty;
    [Required(ErrorMessage="ISBN is required")]
    [RegularExpression(@"^\d{10}(\d{3})?$", ErrorMessage="ISBN must be 10 or 13 digits")]
    public string ISBN { get; set; } = string.Empty;
    [Required]
    [Range(1800,2100, ErrorMessage="Published Year must be between 1800 and 2100")]
    public int PublishedYear { get; set; }
}
