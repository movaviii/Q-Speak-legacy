# Q-Speak

A small Windows Forms text-to-speech app. It can send speech to any Windows audio output, including a virtual cable.

## VB-CABLE setup

Q-Speak is set up to use [VB-Audio Virtual Cable](https://vb-audio.com/Cable/). Install the driver from that page as administrator, then restart Windows.

### Send Q-Speak to the cable

1. Open Q-Speak and open its settings menu.
2. Choose **Select output device**.
3. Select **CABLE Input (VB-Audio Virtual Cable)**.

Audio sent to `CABLE Input` arrives at `CABLE Output`.

### Hear the cable through speakers or headphones

This is the monitoring setup used by Q-Speak:

1. Open **Settings** > **System** > **Sound** > **More sound settings**.
2. Open the **Recording** tab.
3. Right-click **CABLE Output (VB-Audio Virtual Cable)** > **Properties** > **Listen**.
4. Enable **Listen to this device**.
5. Under **Playback through this device**, select your actual speakers or headphones, then choose **Apply**.

Do not select `CABLE Input` as the playback device here; that creates a feedback loop. Apps that should receive Q-Speak as an input (for example, a voice chat app) should use **CABLE Output** as their microphone.
