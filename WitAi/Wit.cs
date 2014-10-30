using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WitAi
{
    public class Wit
    {
        private WitMic witMic;
        private string accessToken;

        /// <summary>
        /// Voice activity detection feature
        /// </summary>
        public bool DetectSpeechStop { get; set; }

        /// <summary>
        /// Enables debug output to Console
        /// </summary>
        public static bool DebugMode
        {
            get
            {
                return WitLog.IsLoggingEnabled;
            }
            set
            {
                WitLog.IsLoggingEnabled = value;
            }
        }

        /// <summary>
        /// Returns microphone state, i.e. is it recording now or not
        /// </summary>
        public bool IsRecording
        {
            get
            {
                return witMic.IsRecording;
            }
        }

        /// <summary>
        /// Initializes new instance of Wit class
        /// </summary>
        /// <param name="accessToken">Client access token. You can grab it from your Wit console, under Settings\Access Token.</param>
        /// <param name="detectSpeechStop">Voice activity detection feature</param>
        public Wit(string accessToken, bool detectSpeechStop = true)
        {
            this.accessToken = accessToken;
            this.DetectSpeechStop = detectSpeechStop;
        }

        /// <summary>
        /// Capture intent and entities from a text string
        /// </summary>
        /// <param name="text">Text string to capture from</param>
        /// <returns>Captured data</returns>
        public async Task<WitResponse> CaptureTextIntent(string text)
        {
            WitMessageRequestTask witMessageRequestTask = new WitMessageRequestTask(accessToken, text);

            string result = await witMessageRequestTask.GetAsync();

            if (result != null)
            {
                WitResponse witResponse = JsonConvert.DeserializeObject<WitResponse>(result);

                return witResponse;
            }

            return null;
        }

        /// <summary>
        /// Capture intent and entities from a microphone
        /// </summary>
        /// <returns>Captured data</returns>
        public async Task<WitResponse> CaptureVoiceIntent()
        {
            WitPipedStream witPipedStream = new WitPipedStream();

            witMic = new WitMic(witPipedStream, DetectSpeechStop);

            if (witMic.StartRecording())
            {
                return await StreamRawAudio(witPipedStream, "audio/raw", "signed-integer", 16, 16000, ByteOrder.LITTLE_ENDIAN);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Stop capture intent from a microphone
        /// </summary>
        public void StopCaptureVoiceIntent()
        {
            witMic.StopRecording();            
        }

        /// <summary>
        /// Streams raw audio data and returns captured intent and entities
        /// </summary>
        /// <param name="witPipedStream">Audio stream</param>
        /// <param name="type">Type</param>
        /// <param name="encoding">Encoding</param>
        /// <param name="bits">Bits per sample</param>
        /// <param name="rate">Samples per second</param>
        /// <param name="order">Bytes order</param>
        /// <returns>Captured data</returns>
        public async Task<WitResponse> StreamRawAudio(WitPipedStream witPipedStream, string type, string encoding, int bits, int rate, ByteOrder order)
        {
            WitSpeechRequestTask witSpeechRequestTask = new WitSpeechRequestTask(accessToken, witPipedStream, type, encoding, bits, rate, order);

            string result = await witSpeechRequestTask.UploadAsync();

            if (result != null)
            {
                WitResponse witResponse = JsonConvert.DeserializeObject<WitResponse>(result);

                return witResponse;
            }

            return null;
        }
    }

    /// <summary>
    /// Bytes order
    /// </summary>
    public enum ByteOrder
    {
        LITTLE_ENDIAN,
        BIG_ENDIAN
    }
}
