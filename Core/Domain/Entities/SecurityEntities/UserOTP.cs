using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SecurityEntities
{
    public class UserOTP
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string OTP { get; set; }

        public DateTime Expire { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }

}
