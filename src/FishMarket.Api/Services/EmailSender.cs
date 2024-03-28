namespace FishMarket.Api.Services;

/// <summary>
/// This is an in-memory Email Sending client and is designed for test purpose only.
/// In a real-world scenario, you should use a service like SendGrid and implement this class using its API.
/// </summary>
public sealed class EmailSender(ILogger<EmailSender> logger) : IEmailSender
{
    private readonly ILogger<EmailSender> _logger = logger;

    /// <summary>
    /// Sends a single email to our internal records.
    /// </summary>
    /// <param name="email">The recipient’s email address.</param>
    /// <param name="subject">The subject of your email.</param>
    /// <param name="plainTextContent">The text/plain content of the email body.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    public Task SendEmailAsync(string email, string subject, string plainTextContent)
    {
        _logger.LogInformation("Sending email to '{email}' with subject '{subject}'", email, subject);

        return Task.CompletedTask;
    }
}
