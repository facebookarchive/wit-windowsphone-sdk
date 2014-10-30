using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WitAi
{
    public partial class WitMicButton : UserControl
    {
        private Wit wit;

        private bool isToggled;
        public bool IsToggled
        {
            get
            {
                return isToggled;
            }
            set
            {
                isToggled = value;

                if (isToggled)
                {
                    VisualStateManager.GoToState(this, "Toggled", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal", false);
                }
            }
        }

        public string AccessToken { get; set; }

        public bool DetectSpeechStop { get; set; }

        public delegate void CaptureVoiceIntentStartedEventHandler(object sender, EventArgs e);
        public event CaptureVoiceIntentStartedEventHandler CaptureVoiceIntentStarted;

        public delegate void CaptureVoiceIntentStoppedEventHandler(object sender, EventArgs e);
        public event CaptureVoiceIntentStoppedEventHandler CaptureVoiceIntentStopped;

        public delegate void CaptureVoiceIntentCompletedEventHandler(object sender, WitResponse witResponse);
        public event CaptureVoiceIntentCompletedEventHandler CaptureVoiceIntentCompleted;

        public WitMicButton()
        {
            InitializeComponent();
        }

        private async void MicButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (AccessToken == null)
            {
                WitLog.Log("WitMicButton", "Did you set your AccessToken propeply?");

                return;
            }

            IsToggled = !IsToggled;

            if (IsToggled)
            {
                if (CaptureVoiceIntentStarted != null)
                {
                    CaptureVoiceIntentStarted(this, EventArgs.Empty);
                }

                wit = new Wit(AccessToken, DetectSpeechStop);

                WitResponse witResponse = await wit.CaptureVoiceIntent();

                IsToggled = false;

                if (CaptureVoiceIntentCompleted != null)
                {
                    CaptureVoiceIntentCompleted(this, witResponse);
                }
            }
            else
            {
                if (wit != null)
                {
                    wit.StopCaptureVoiceIntent();
                }

                if (CaptureVoiceIntentStopped != null)
                {
                    CaptureVoiceIntentStopped(this, EventArgs.Empty);
                }
            }
        }

        private void WitButton_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            if (!IsToggled)
            {
                VisualStateManager.GoToState(this, "Pressed", false);
            }
        }

        private void WitButton_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
        }
    }
}
