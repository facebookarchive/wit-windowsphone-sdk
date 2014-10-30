using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace WitAi
{
    public class WitMessageRequestTask
    {
        private const string Version = "20141026";

        private string accessToken;
        private string message;

        /// <summary>
        /// Initializes new instance of WitMessageRequestTask class
        /// </summary>
        /// <param name="accessToken">Access Token</param>
        /// <param name="message">Text string to capture from</param>
        public WitMessageRequestTask(string accessToken, string message)
        {
            this.accessToken = accessToken;
            this.message = message;
        }

        /// <summary>
        /// Gets intent and entities from a text string
        /// </summary>
        /// <returns>Unprocessed result from a server</returns>
        public async Task<string> GetAsync()
        {
            return await Task.Run<string>(async () =>
            {
                try
                {
                    var socket = new StreamSocket();

                    var writer = new DataWriter(socket.OutputStream);
                    var reader = new DataReader(socket.InputStream);
                    reader.InputStreamOptions = InputStreamOptions.Partial;

                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine(String.Format("GET /message?q={0} HTTP/1.1", HttpUtility.UrlEncode(message)));
                    stringBuilder.AppendLine("Host: api.wit.ai");
                    stringBuilder.AppendLine(String.Format("Authorization: Bearer {0}", accessToken));
                    stringBuilder.AppendLine("");
                    string headers = stringBuilder.ToString();
                    byte[] headersPayload = Encoding.UTF8.GetBytes(headers);

                    await socket.ConnectAsync(new HostName("wit.ai"), "443", SocketProtectionLevel.SslAllowNullEncryption);

                    writer.WriteBytes(headersPayload);

                    WitLog.Log("Sending text...");

                    await writer.StoreAsync();
                    await writer.FlushAsync();

                    var count = await reader.LoadAsync(UInt16.MaxValue);
                    string response = reader.ReadString(count);

                    int bodyStartIndex = response.IndexOf("\r\n\r\n");

                    if (bodyStartIndex != -1)
                    {
                        string body = response.Substring(bodyStartIndex + 4);

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
