using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movie.Models
{
    [Owned]
    public class RefreshToken
    {
        //public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public bool IsActive => RevokedOn == null && !IsExpired;



        //[ForeignKey(nameof(appuser))]
        //public string? userid { get; set; }
        //public AppUser? appuser { get; set; }

    }



}
