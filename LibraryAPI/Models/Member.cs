using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models;

public class Member
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public DateTime JoinedDate { get; set; }

    // Navigation Property
    public List<Loan> Loans { get; set; } = new();

}
