using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
   [Required]
    public string UserName { get; set; }
    [StringLength(8, MinimumLength =4)]
    [Required]
    public string Password{ get; set; }
    [Required] public string Gender { get; set; }
    [Required] public string KnownAs { get; set; }
    [Required] public DateOnly? DateOfBirth { get; set; } // optional to make required work for date!
    [Required] public string City { get; set; }
    [Required] public string Country { get; set; }

}
