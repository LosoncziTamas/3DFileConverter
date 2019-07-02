using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Converter.Data;
using Converter.Documents;
using Converter.Utils;

namespace Converter.Conversion
{
    public class ObjToStlConversionStrategy : IConversionStrategy
    {
        private readonly ObjReader _reader;
        private readonly StlWriter _writer;

        public ObjToStlConversionStrategy()
        {
            _reader = new ObjReader();
            _writer = new StlWriter();
        }
        
        public void ApplyConversion(Stream inputStream, Stream outputStream)
        {
            var source = _reader.ReadFromStream(inputStream);
            

            _writer.WriteToStream(source, outputStream);
        }


    }
}