﻿

namespace BaseLibrary.DTO;

public class UserSession
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
