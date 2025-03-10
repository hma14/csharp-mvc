using Omnae.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class PartRvisionDTO
    {
        public int TaskId { get; set; }
        public int OriginProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public States StateId { get; set; }

    }
}