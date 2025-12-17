using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models.DTO;

public class CreateMemberRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
