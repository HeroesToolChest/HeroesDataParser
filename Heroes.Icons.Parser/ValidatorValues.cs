using System.Text.RegularExpressions;

namespace Heroes.Icons.Parser
{
    public static class ValidatorValues
    {
        public static int GetValue(string id)
        {
            if (int.TryParse(Regex.Match(id, @"\d+").Value, out int value))
                return value;
            else
                return 0;
        }
    }
}
