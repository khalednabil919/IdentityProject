using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.AuthDTO.Request
{
    public class AddClaimToUserRequest
    {
        [Required]
        public string ClaimValue { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
