using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Helpers
{
    public class APIResult
    {
        public bool state { get; set; }
        public string? message { get; set; }
        public dynamic? entity { get; set; }
    }
}
