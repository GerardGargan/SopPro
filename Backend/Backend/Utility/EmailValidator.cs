using System.ComponentModel.DataAnnotations;

namespace Backend.Utility
{
    /// <summary>
    /// Validates an email consistent with the dot net email validation attribute.
    /// </summary>
    public static class EmailValidator
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var emailAttribute = new EmailAddressAttribute();
            return emailAttribute.IsValid(email);
        }
    }
}
