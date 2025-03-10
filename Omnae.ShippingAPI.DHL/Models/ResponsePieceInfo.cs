using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class ResponsePieceInfo
    {
        public List<ResponsePieceDetails> PieceDetails { get; set; }
    }
}
