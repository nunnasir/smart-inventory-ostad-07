using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SmartInventory.BLL.Helpers;

/// <summary>
/// Provides email format validation using RFC-compliant rules.
/// </summary>
public static class EmailValidator
{
    // Common regex pattern for email validation (simpler than full RFC 5322, but widely used)
    // Covers: local@domain.tld, allows subdomains and common TLDs
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(100));

    /// <summary>
    /// Validates email format using MailAddress parsing (RFC 5322 compliant).
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>True if the email format is valid; otherwise, false.</returns>
    public static bool IsValidFormat(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var trimmed = email.Trim();

        // RFC 5321: max length is 254 characters
        if (trimmed.Length > 254)
            return false;

        try
        {
            var mailAddress = new MailAddress(trimmed);
            // Reject display names (e.g., "Name" &lt;user@domain.com&gt;) - accept only raw email
            return mailAddress.Address.Equals(trimmed, StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    /// <summary>
    /// Validates email format using regex pattern (faster for high-throughput scenarios).
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>True if the email format matches the pattern; otherwise, false.</returns>
    public static bool IsValidFormatRegex(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > 254)
            return false;

        try
        {
            return EmailRegex.IsMatch(email.Trim());
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    /// <summary>
    /// Validates email and returns a result with an optional error message.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>A tuple of (isValid, errorMessage). errorMessage is null when valid.</returns>
    public static (bool IsValid, string? ErrorMessage) Validate(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return (false, "Email is required.");

        var trimmed = email.Trim();

        if (trimmed.Length > 254)
            return (false, "Email address is too long.");

        try
        {
            var mailAddress = new MailAddress(trimmed);
            if (!mailAddress.Address.Equals(trimmed, StringComparison.OrdinalIgnoreCase))
                return (false, "Invalid email format. Please enter a valid email address.");

            return (true, null);
        }
        catch (FormatException)
        {
            return (false, "Invalid email format. Please enter a valid email address.");
        }
    }
}
