using System.Collections.Generic;
using System.Web;

namespace Omnae.Util
{
    public static class FileUploadHelper
    {
        public static IList<HttpPostedFileBase> ToFileList(this HttpFileCollectionBase filesCollectionBase)
        {
            var files = new List<HttpPostedFileBase>();
            if (filesCollectionBase == null || filesCollectionBase.Count == 0)
                return files;

            for (int i = 0; i < filesCollectionBase.Count; i++)
            {
                var filebase = filesCollectionBase[i];
                files.Add(filebase);
            }
            return files;
        }
    }
}