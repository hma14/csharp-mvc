using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Model.Models
{
    public class QboTokens
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string RealmId { get; set; }
        
        public string AccessToken { get; set; }
        public DateTime AccessTokenCreatedAt { get; set; }
        public string RefreshToken { get; set; }

        public string TokenEndpoint { get; set; }

        public DateTime RefeshTokenExpireAt { get; set; }

    }
}
