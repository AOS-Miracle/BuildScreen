using System.Text.RegularExpressions;

namespace BuildScreen.Core.Utilities
{
    public static class Validate
    {
        public static bool IsNumeric(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            Regex numeric = new Regex(@"^[0-9]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return numeric.IsMatch(value);
        }

        public static bool IsDomain(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            Regex url = new Regex(@"^[\w\-_]+((\.[\w\-_]+)+([a-z]))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return url.IsMatch(value);
        }

        public static bool IsIpAddress(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            Regex ipAddress = new Regex(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return ipAddress.IsMatch(value);
        }
    }
}
