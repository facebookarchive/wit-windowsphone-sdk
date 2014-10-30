using Microsoft.Phone.Controls;
using System;
using Witai;

namespace WitAiSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            Wit.DebugMode = true;
        }

        private async void WitText_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.Focus();

                ProgressBar.IsIndeterminate = true;

                Wit wit = new Wit("CBP3OGVVJI23M5XAH7ARKOMDDSKB3HJV");

                WitResponse witResponse = await wit.CaptureTextIntent(WitText.Text);

                if (witResponse != null && witResponse.outcomes != null && witResponse.outcomes.Count > 0)
                {
                    WitIntent.Text = "Intent = " + witResponse.outcomes[0].intent;
                }
                else
                {
                    WitIntent.Text = "Intent not found";
                }

                ProgressBar.IsIndeterminate = false;
            }
        }

        private void WitMicButton_CaptureVoiceIntentStarted(object sender, EventArgs e)
        {
            
        }

        private void WitMicButton_CaptureVoiceIntentStopped(object sender, EventArgs e)
        {
            ProgressBar.IsIndeterminate = true;
        }

        private void WitMicButton_CaptureVoiceIntentCompleted(object sender, WitResponse witResponse)
        {
            if (witResponse != null && witResponse.outcomes != null && witResponse.outcomes.Count > 0)
            {
                WitIntent.Text = "Intent = " + witResponse.outcomes[0].intent;
            }
            else
            { 
                WitIntent.Text = "Intent not found";
            }

            ProgressBar.IsIndeterminate = false;
        }
    }
}