using System;

namespace QuizProject.Application.Email;

public class EmailSettings
{
    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool EnableSSL { get; set; }
} 