wit-windowsphone-sdk
====================

Windows Phone SDK for Wit


## Quickstart

This section will show you how to get started in minutes using the Wit SDK for Windows Phone

Assumptions
We'll create a project from scratch, but you can easily apply this guide to any existing project.
As we want to focus on the Wit SDK integration, the app will only display the user intent Wit.AI picked up.

## Prerequisites
To follow this tutorial, you will need:

- A [Wit.ai] (https://wit.ai) account
- [Windows Phone SDK](https://dev.windows.com/en-us/develop/download-phone-sdk) (tested on Windows Phone SDK 8.0 and Visual Studio 2012)

## The Windows Phone project

1. Start up Visual Studio<br/>
We'll start from a Windows Phone App
In Visual Studio, go to `File > New > Project...` or press `Ctrl + Shift + N`
Select the `Windows Phone App`

2. Set up a new project<br/>
Type a name for your project and press Ok.<br/>
We will now pull Wit into the project.

## Pulling Wit SDK into your project

1. Grab the binary<br/>
Grab the latest binary from our GitHub repo or build it from source.
In Visual Studio use context menu on `Project - References`, `Add Reference...`, click on `Browse` and select `WitAI.dll` assembly<br/>
2. Now, we need to add the resources (images, etc.) to our project.<br/>
Choose `Add - Existing Item...` on `Assets` folder and select `microphone.png`
3. Update WMAppManifest<br/>
Network and Microphone permissions<br/>
Wit SDK requires `ID_CAP_MICROPHONE` and `ID_CAP_NETWORKING` to be enabled in the `WMAppManifest.xml` file

## Use Wit in your project!
add `using WitAi;` in usings section to any `.cs` file where you will use Wit SDK

Adding the Wit button<br/>
The SDK provides a `WitMicButton` control to simply record the user voice and request the API

We'll add a recording button to the main screen of the app.

add `xmlns:wit="clr-namespace:WitAi;assembly:WitAi"` to xmlns section of `MainPage.xaml`

```
<wit:WitMicButton />
```

Than we need to enter our access token so Wit.ai knows what instance we are querying.
You can grab it from your Wit console, under Settings\Access Token.

```
<wit:WitMicButton AccessToken="<AccessToken>" />
```

`WitMicButton` provides several events related to voice capturing

subscribing to `WitMicButton` events in xaml:
```
<wit:WitMicButton x:Name="WitMicButton" AccessToken="<AccessToken>" CaptureVoiceIntentStarted="WitMicButton_CaptureVoiceIntentStarted" CaptureVoiceIntentStopped="WitMicButton_CaptureVoiceIntentStopped" CaptureVoiceIntentCompleted="WitMicButton_CaptureVoiceIntentCompleted" />
```
subscribing to `WitMicButton` events in code:
```
WitMicButton.CaptureVoiceIntentStarted += WitMicButton_CaptureVoiceIntentStarted;
WitMicButton.CaptureVoiceIntentStopped += WitMicButton_CaptureVoiceIntentStopped;
WitMicButton.CaptureVoiceIntentCompleted += WitMicButton_CaptureVoiceIntentCompleted;
```
And here is event handlers for this events:

```
private void WitMicButton_CaptureVoiceIntentStarted(object sender, EventArgs e)
{
    // voice capture started
}

private void WitMicButton_CaptureVoiceIntentStopped(object sender, EventArgs e)
{
    // voice capture stopped;
}

private void WitMicButton_CaptureVoiceIntentCompleted(object sender, WitResponse witResponse)
{
    // voice capture completed; use witResponse to access captured data
}
```

Note that you can also call Wit programmatically using the - ```Task <WitResponse> CaptureVoiceIntent()``` instance method of the Wit class.

## Acting on Wit response

Wit SDK returns `WitResponse` object, contains all data coming from Wit. Next code shows how to proceed a text message:
```
WitResponse witResponse = await wit.CaptureTextIntent(WitText.Text);

if (witResponse.outcomes != null && witResponse.outcomes.Count > 0)
{
   WitIntent.Text = "Intent = " + witResponse.outcomes[0].intent;
}
else
{
   WitIntent.Text = "Intent not found";
}
```
Note: you should mark your method as async to use await operator inside

## Run your app
That is it! Just run the app in the emulator.
Press the microphone button and say `Wake me up at 7am`.
Provided your instance has an `alarm` intent, you should see this something like this

Now go check your inbox.
The command you just said should be there. Click the wave icon next to the sentence to play the audio file.

Hooray!

If your instance is brand new, it will need a bit of training before yielding satisfying results.
Please refer to the Quickstart tutorial.

You can find the code for this tutorial at [link]
