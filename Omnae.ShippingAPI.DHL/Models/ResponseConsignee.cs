﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.ShippingAPI.DHL.Models
{
    public class ResponseConsignee
    {
        public string City { get; set; }
        public string DivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
    }
}
