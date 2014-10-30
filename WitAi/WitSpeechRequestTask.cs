using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace WitAi
{
    internal class WitSpeechRequestTask
    {
        WitPipedStream witPipedStream;

        private const string Version = "20141026";

        private string accessToken;

        private string type;
        private string encoding;
        private int bits;
        private int rate;
        private ByteOrder order;

        /// <summary>
        /// Initializes new instance of WitSpeechRequestTask class
        /// </summary>
        /// <param name="accessToken">Access Token</param>
        /// <param name="witPipedStream">Audio stream</param>
        /// <param name="type">Type</param>
        /// <param name="encoding">Encoding</param>
        /// <param name="bits">Bits per sample</param>
        /// <param name="rate">Samples per second</param>
        /// <param name="order">Bytes order</param>
        public WitSpeechRequestTask(string accessToken, WitPipedStream witPipedStream, string type, string encoding, int bits, int rate, ByteOrder order)
        {
            this.accessToken = accessToken;
            this.witPipedStream = witPipedStream;
            this.type = type;
            this.encoding = encoding;
            this.bits = bits;
            this.rate = rate;
            this.order = order;
        }

        /// <summary>
        /// Uploads data with a chunks
        /// </summary>
        /// <returns>Unprocessed result from a server</returns>
        public async Task<string> UploadAsync()
        {
            return null;
            return await Task.Run<string>(async () =>
            {
                try
                {
                    var socket = new StreamSocket();

                    var writer = new DataWriter(socket.OutputStream);
                    var reader = new DataReader(socket.InputStream);
                    reader.InputStreamOptions = InputStreamOptions.Partial;

                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine(String.Format("POST /speech?v={0} HTTP/1.1", Version));
                    stringBuilder.AppendLine("Host: api.wit.ai");
                    stringBuilder.AppendLine("Accept: applicaiton/json");
                    stringBuilder.AppendLine(String.Format("Authorization: Bearer {0}", accessToken));
                    stringBuilder.AppendLine(String.Format("Content-Type: {0};encoding={1};bits={2};rate={3};endian={4}", type, encoding, bits, rate, order == ByteOrder.LITTLE_ENDIAN ? "little" : "big"));
                    stringBuilder.AppendLine("Transfer-Encoding: chunked");
                    stringBuilder.AppendLine("Connection: Keep-Alive");
                    string headers = stringBuilder.ToString();
                    byte[] headersPayload = Encoding.UTF8.GetBytes(headers);

                    await socket.ConnectAsync(new HostName("wit.ai"), "443", SocketProtectionLevel.SslAllowNullEncryption);

                    writer.WriteBytes(headersPayload);

                    byte[] data;

                    WitLog.Log("Sending speech...");

                    while (true)
                    {
                        data = witPipedStream.Read();

                        if (data.Length > 0)
                        {
                            byte[] chunkHeader = Encoding.UTF8.GetBytes("\r\n" + data.Length.ToString("X") + "\r\n");
                            writer.WriteBytes(chunkHeader);

                            writer.WriteBytes(data);
                            await writer.StoreAsync();
                            await writer.FlushAsync();
                        }
                        else if (!witPipedStream.IsInputCompleted)
                        {
                            await Task.Delay(50);
                        }
                        else
                        {
                            break;
                        }
                    }

                    byte[] lastChunkHeader = Encoding.UTF8.GetBytes("\r\n0\r\n\r\n");
                    writer.WriteBytes(lastChunkHeader);

                    await writer.StoreAsync();
                    await writer.FlushAsync();

                    var count = await reader.LoadAsync(UInt16.MaxValue);
                    string response = reader.ReadString(count);

                    int bodyStartIndex = response.IndexOf("\r\n\r\n");

                    if (bodyStartIndex != -1)
                    {
                        string body = response.Substring(bodyStartIndex + 4).Trim();

                        WitLog.Log(body);

                        return body;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            });
        }
    }
}
