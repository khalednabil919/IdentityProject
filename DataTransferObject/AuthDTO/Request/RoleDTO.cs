using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.AuthDTO.Request
{
    public class RoleDTO
    {
        [Required(ErrorMessage ="Name of Role is Required")]
        public string Name { get; set; }
    }
}
