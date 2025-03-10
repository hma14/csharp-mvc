using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{
    public static class FileUtil
    {
        public static string MakeValidFileName(string name)
        {
            var invalidFileNameChars = new string(Path.GetInvalidFileNameChars());
            string invalidChars = Regex.Escape(invalidFileNameChars);
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }
	}
}
