using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Omnae.WebApi.Util
{
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path)
            : base(path)
        { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ?
                headers.ContentDisposition.FileName :
                "NoName";
            // This is here because Chrome submits files in quotation 
            // marks which get treated as part of the filename and get escaped
            return name.Replace("\"\"", string.Empty);
        }
    }
}