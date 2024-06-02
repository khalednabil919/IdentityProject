using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.AuthDTO.Response
{
    public class AccessAndRefreshToken
    {
        public string AccessToken { get; set; }
        public  string RefreshToken { get; set; }
    }
}
