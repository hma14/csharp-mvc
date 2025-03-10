using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Omnae.WebApi.Models
{
    /// <summary>
    /// Represent a Page of Results
    /// </summary>
    /// <typeparam name="T">Type of the Result</typeparam>
    [DataContract(IsReference = true)]
    [JsonObject(IsReference = false)]
    [JsonArray(IsReference = false)]
    public class PagedResultSet<T>
    {
        /// <summary>
        /// Metadata for this Page of Results
        /// </summary>
        [Required, NotNull]
        public PageMetadata Metadata { get; set; } = new PageMetadata();

        /// <summary> 
        /// The records this page represents. 
        /// </summary> 
        [Required, NotNull]
        [JsonProperty(IsReference = false)]
        public IEnumerable<T> Results { get; set; } = new List<T>();

        /// <summary>
        /// Page Metadata Information
        /// </summary>
        public class PageMetadata
        {
            /// <summary>
            /// The page number this page represents. 
            /// </summary>
            public int PageNumber { get; set; }

            /// <summary> 
            /// The size of this page. 
            /// </summary> 
            public int PageSize { get; set; }

            /// <summary> 
            /// The total number of pages available. 
            /// </summary> 
            public int TotalNumberOfPages { get; set; }

            /// <summary> 
            /// The total number of records available. 
            /// </summary> 
            public int TotalNumberOfRecords { get; set; }

            /// <summary> 
            /// The URL to the Next pages
            /// </summary> 
            [CanBeNull]
            public string NextPageUrl { get; set; }
        }
    }
}
    
    