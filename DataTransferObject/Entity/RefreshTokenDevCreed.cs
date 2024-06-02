using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Entity
{
    //[Owned]
    //[Keyless]
    public class RefreshTokenDevCreed
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public DateTime CreatedOn { get; set; }
        public DateTime? IsRevoked { get; set; }
        public bool IsActive => IsRevoked == null && !IsExpired;
        public string UserId { get; set; }
        //[ForeignKey("UserId")]
        //public User User { get; set; }
    }
}
