using System;

namespace Omnae.WebApi.DTO
{
    public class ProductShareDto
    {
        public string Id { get; set; }
        public string SharerCompanyId { get; set; }
        public string ShareeCompanyId { get; set; }
        public string ShareeCompanyName { get; set; }
        public string ShareeCompanyEmail { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
