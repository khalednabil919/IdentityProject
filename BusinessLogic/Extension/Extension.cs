using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Extension
{
    public static class Extension
    {
        public static List<string> GenerateClaimsForRole(this RoleManager<IdentityRole> roleManager, string module)
        {
            return new List<string>()
            {
                $"Permission.{module}.Get",
                $"Permission.{module}.Create",
                $"Permission.{module}.Update",
                $"Permission.{module}.Delete"
            };
        }
    }
}

