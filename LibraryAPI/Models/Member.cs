using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
    [JsonIgnore]  //  Don't serialize this collection
    public List<Loan> Loans { get; set; } = new();

}
