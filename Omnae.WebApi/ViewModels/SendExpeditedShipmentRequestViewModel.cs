using Omnae.Common;
using Omnae.Data.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.ViewModels
{
    public class SendExpeditedShipmentRequestViewModel
    {
        public int OrderId { get; set; }
        public OrderQuery.UserMode Mode { get; set; }
        public EXPEDITED_SHIPMENT_TYPE ExpeditedShipmentState { get; set; }
    }
}