using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models.DTO;

public class LoanBookRequest
{
    [Required]
    public int BookId { get; set; }

    [Required]
    public int MemberId { get; set; }

    [Range(1, 90, ErrorMessage = "Loan duration must be between 1 and 90 days")]
    public int DurationInDays { get; set; } = 14; // Default 2 weeks
}
