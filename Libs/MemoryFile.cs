using System;
using System.IO;
using System.Web;

namespace Libs
{
    public class MemoryFile : HttpPostedFileBase, IDisposable
    {
        private readonly Stream _stream;

        public MemoryFile(string filePath, string contentType)
        {
            this.ContentType = contentType;
            this.FileName = Path.GetFileName(filePath);

            this._stream = File.OpenRead(filePath);
        }

        public override int ContentLength => (int) _stream.Length;

        public override string ContentType { get; }

        public override string FileName { get; }

        public override Stream InputStream => _stream;

        public override void SaveAs(string filename)
        {
            using (var file = File.Open(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                _stream.CopyTo(file);
            }
        }

        public void Dispose() => _stream?.Dispose();
        
    }
}
