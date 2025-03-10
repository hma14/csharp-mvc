using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Omnae.Extensions
{
    public static class HttpRequestBaseExtensions
    {
        public static Dictionary<string, byte[]> GetPostedFileBytes(this HttpRequestBase request)
        {
            if (request == null) throw new ArgumentNullException("request");

            var dictionary = new Dictionary<string, byte[]>();

            foreach (string fileName in request.Files)
            {
                var file = request.Files[fileName];
                if (file != null && file.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(file.InputStream))
                    {
                        byte[] blobValue = reader.ReadBytes((int)file.InputStream.Length);
                        dictionary.Add(fileName, blobValue);
                    }
                }
            }

            return dictionary;
        }
    }
}