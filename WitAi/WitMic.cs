using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Witai
{
    internal class WitMic
    {
        Microphone microphone;
        DispatcherTimer updateTimer;
        byte[] speech;
        WitDetectTalking witDetectTalking;
        bool detectSpeechStop;

        WitPipedStream witPipedStream;

        /// <summary>
        /// Returns microphone state, i.e. is it recording now or not
        /// </summary>
        public bool IsRecording
        {
            get
            {
                return microphone.State == MicrophoneState.Started;
            }
        }

        /// <summary>
        /// Initializes new instance of WitMic
        /// </summary>
        /// <param name="witPipedStream">Stream to write audio to</param>
        /// <param name="detectSpeechStop">Voice activity detection feature</param>
        public WitMic(WitPipedStream witPipedStream, bool detectSpeechStop)
        {
            this.witPipedStream = witPipedStream;
            this.detectSpeechStop = detectSpeechStop;

            microphone = Microphone.Default;

            if (microphone == null)
            {
                WitLog.Log("Did you enabled ID_CAP_MICROPHONE in WMAppManifest.xml?");

                return;
            }

            witDetectTalking = new WitDetectTalking();

            microphone.BufferDuration = TimeSpan.FromMilliseconds(100);

            speech = new byte[microphone.GetSampleSizeInBytes(microphone.BufferDuration)];

            microphone.BufferReady += microphone_BufferReady;

            updateTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            updateTimer.Tick += (s, e) =>
            {
                FrameworkDispatcher.Update();
            };
        }

        /// <summary>
        /// Starts a voice recording
        /// </summary>
        /// <returns>Returns success of the operation</returns>
        public bool StartRecording()
        {
            if (microphone != null && !IsRecording)
            {
                FrameworkDispatcher.Update();

                updateTimer.Start();

                microphone.Start();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when new audio chuck is ready to use
        /// </summary>
        /// <param name="sender">Microphone instance</param>
        /// <param name="e">Event arguments</param>
        private void microphone_BufferReady(object sender, EventArgs e)
        {
            int microphoneDataSize = microphone.GetData(speech);

            int talking = witDetectTalking.Talking(speech);

            witPipedStream.Write(speech);

            if (detectSpeechStop)
            {
                if (talking == 1)
                {
                    WitLog.Log("Start talking detected");
                }
                else if (talking == 0)
                {
                    WitLog.Log("Stop talking detected");

                    StopRecording();
                }
            }
        }

        /// <summary>
        /// Stops a voice recording
        /// </summary>
        public void StopRecording()
        {
            if (microphone != null && IsRecording)
            {
                microphone.Stop();

                microphone.BufferReady -= microphone_BufferReady;

                updateTimer.Stop();

                witPipedStream.InputCompleted();
            }
        }
    }
}
