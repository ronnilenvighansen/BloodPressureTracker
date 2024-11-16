namespace Shared.Utilities
{
    public static class DateHelper
    {
        public static bool IsPastDate(DateTime date)
        {
            return date < DateTime.UtcNow;
        }
    }
}
