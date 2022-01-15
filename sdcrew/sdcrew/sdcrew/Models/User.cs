using SQLite;

using System;
using System.Collections.Generic;
using System.Text;

namespace sdcrew.Models
{
    public class User
    {
        //CustomerID from http://schemas.satcomdirect.com/ws/2014/10/identity/claims/customerids
        //Account from http://schemas.satcomdirect.com/ws/2014/10/identity/claims/accountids

        [PrimaryKey, AutoIncrement]
        public int UserID { get; set; }

        public string IdentityToken { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessTokenGoodUntilUTC { get; set; }
        public string RefreshToken { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CustomerId { get; set; }
        public string AccountId { get; set; }

        public DateTime LoginTime { get; set; }

        public Uri ImageUri { get; set; }


    }

}

