using System.ComponentModel.DataAnnotations;

namespace FishMarket.Api.Dtos;

public class UserInfo
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}