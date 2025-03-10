using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class ProductCustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Logo { get; set; }
        public DateTime? BecameCustomerAt { get; set; }
    }
}