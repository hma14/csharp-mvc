using JetBrains.Annotations;
using Omnae.Common;
using Omnae.Model.Models;
using Omnae.Model.Models.Aspnet;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omnae.WebApi.DTO
{
    public class PartRevisionDTO
    {
        public int Id { get; set; }
        public int? TaskId { get; set; }
        public int ProductId { get; set; }
        public int OriginProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public States StateId { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}
