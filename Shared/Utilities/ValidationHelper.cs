namespace Shared.Utilities
{
    public static class ValidationHelper
    {
        public static bool IsValidSSN(string ssn)
        {
            // Example: Validate Danish SSN format (10 digits).
            return !string.IsNullOrEmpty(ssn) && ssn.Length == 10 && ssn.All(char.IsDigit);
        }
    }
}
