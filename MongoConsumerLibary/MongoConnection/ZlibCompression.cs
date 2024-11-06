using ICSharpCode.SharpZipLib.Zip.Compression;
using System;
using System.Text;

namespace MongoConsumerLibary.MongoConnection
{
    public class ZlibCompression
    {
        private readonly Deflater _deflater = new Deflater();
        private readonly Inflater _inflater = new Inflater();
        private const int ZLIB_HEADER_SIZE = 3;

        public ZlibCompression()
        {
            _deflater = new Deflater();
            _inflater = new Inflater();
        }

        public string CompressData(string data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            _deflater.Reset();
            _deflater.SetInput(byteData);
            _deflater.Finish();

            byte[] compressedData = new byte[byteData.Length + ZLIB_HEADER_SIZE];
            int compressedSize = _deflater.Deflate(compressedData);

            Array.Resize(ref compressedData, compressedSize);
            string compressedDataString = Convert.ToBase64String(compressedData);
            return compressedDataString;
        }
        
        public string DecompressData(string data)
        {
            const int STRING_STARTING_POINT = 0; 
            byte[] byteData = Convert.FromBase64String(data);
            _inflater.Reset();
            _inflater.SetInput(byteData);

            byte[] outputData = new byte[byteData.Length];
            _inflater.Inflate(outputData);
            string retString = Encoding.UTF8.GetString(outputData);

            while(!_inflater.IsFinished)
            {
                int bytesWritten = _inflater.Inflate(outputData);
                retString += Encoding.UTF8.GetString(outputData, STRING_STARTING_POINT,bytesWritten);
            }
            return retString;
        }
    }
}
