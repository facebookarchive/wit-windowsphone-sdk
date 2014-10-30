using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witai
{
    public class WitPipedStream
    {
        public Stream stream { get; set; }
        private object sync = new object();
        private int readPosition;

        /// <summary>
        /// Gets the state of the stream. True of no data can be written anymore
        /// </summary>
        public bool IsInputCompleted { get; private set; }

        /// <summary>
        /// Gets the length of the stream data
        /// </summary>
        public long Length
        {
            get
            {
                return stream.Length;
            }
        }

        /// <summary>
        /// Initializes new instance of WitPipedStream class
        /// </summary>
        public WitPipedStream()
        {
            stream = new MemoryStream();
        }

        /// <summary>
        /// Writes data array to the stream
        /// </summary>
        /// <param name="data">Data to write</param>
        public void Write(byte[] data)
        {
            if (IsInputCompleted)
            {
                WitLog.Log("Did you closed WitPipedStream accidentally?");

                return;
            }

            lock (sync)
            {
                stream.Seek(0, SeekOrigin.End);

                stream.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Read the data from the stream by chunks
        /// </summary>
        /// <returns>Data array</returns>
        public byte[] Read()
        {
            byte[] buffer = new byte[3200];            
            int count = 0;

            lock (sync)
            {
                stream.Position = readPosition;

                count = stream.Read(buffer, 0, 3200);

                readPosition += count;
            }

            byte[] data = new byte[count];

            Buffer.BlockCopy(buffer, 0, data, 0, count);

            return data;
        }        

        /// <summary>
        /// When you mark input as completed, it enables to upload remaining data to the Wit and close a connection
        /// </summary>
        public void InputCompleted()
        {
            IsInputCompleted = true;
        }
    }
}
