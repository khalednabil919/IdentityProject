using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Helpers
{
    public static class Validations
    {
        public static string EmailOrPhone(string type)
        {
            if (type != null)
            {
                bool isPhone = type.All(char.IsDigit) && type.Length == 10 && type.StartsWith(Res.phoneType);
                if (isPhone)
                    return Res.PhoneNumber;

                return type.Contains('.') && type.Count(x => x == '@') == 1 && type.Count(x => x == '.') + 1 < type.Length ? Res.Email : default;

            }
            return default;
        }

    }
}
