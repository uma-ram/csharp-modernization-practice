using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public class CreateTodoRequest
{
    [Required(ErrorMessage ="Title is required.")]
    [StringLength(100, MinimumLength =3, ErrorMessage ="Title must be between 3 and 100 characters")]
    public string Title { get; set; } = string.Empty;
}
