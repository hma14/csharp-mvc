using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.WebApi.DTO
{
    public class DocumentDTO
    {
        /// <summary>
        /// Doc Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Product Id
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Order Id
        /// </summary>
        public int? OrderId { get; set; }
        /// <summary>
        /// Task Id
        /// </summary>
        public int? TaskId { get; set; }

        /// <summary>
        /// Document Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Document Version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Document Uri (Azure Blob URL)
        /// </summary>
        public string DocUri { get; set; }

        /// <summary>
        /// Document Type
        /// </summary>
        public int? DocType { get; set; }

        /// <summary>
        /// Is Doc is locked
        /// </summary>
        public bool? IsLocked { get; set; }

        /// <summary>
        /// Doc created Utc datetime
        /// </summary>
        public DateTime CreatedUtc { get; set; }

        /// <summary>
        /// Doc updated by user name
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Doc modified Utc datetime
        /// </summary>
        public DateTime ModifiedUtc { get; set; }

        ///// <summary>
        ///// Current User Type (Customer/Vendor ?)
        ///// </summary>
        public int? UserType { get; set; }

    }
}