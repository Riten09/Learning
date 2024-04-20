using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class loginDto
{
[Required]
public string username { get; set; }
[Required]
public string Password { get; set; }
}
