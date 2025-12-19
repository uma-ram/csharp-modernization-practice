using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Author { get; set; } = string.Empty;

    [Required]
    [StringLength(13, MinimumLength = 10)] 
    public string ISBN { get; set; } = string.Empty;

    [Range(1800,2100)]
    public int PublishedYear { get; set; }

    public bool isAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    //Navigation Property

    [JsonIgnore]  //  Don't serialize this collection
    public List<Loan> Loans { get; set; } = new List<Loan>();

}
