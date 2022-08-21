using System.ComponentModel.DataAnnotations;

namespace IdentityService.DataObjects;

public class LoginCredentials{
    public string Email { get; set; }
    public string Password { get; set; }
}