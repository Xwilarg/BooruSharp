using System.Text;
using System.Text.Json;

namespace BooruSharp.Booru
{
    internal class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            StringBuilder str = new();
            str.Append(char.ToLower(name[0]));
            foreach (char c in name[1..])
            {
                if (char.IsUpper(c))
                {
                    str.Append("_" + char.ToLower(c));
                }
                else
                {
                    str.Append(c);
                }
            }
            return str.ToString();
        }
    }
}
