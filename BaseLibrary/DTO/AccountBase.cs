﻿
using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.DTO;

public class AccountBase
{
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Required]
    public string Password { get; set; } = string.Empty;
}
