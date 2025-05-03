// (c) Meta Platforms, Inc. and affiliates. Confidential and proprietary.

using UnityEngine;
using Oculus.Haptics;
using System;

// This scene is a minimal integration example, meant to run on device (f.e. Meta Quest 2, Meta Quest Pro).
// It showcases how events, like button presses, can be hooked up to haptic feedback; and how we can use other input, like
// a controller's thumbstick movements, to modulate haptic effects.
// We gain access to the Haptics SDK's features through an API by importing Oculus.Haptics (see above).
public class HapticsSdkPlaySample : MonoBehaviour
{
    // The haptic clips are assignable in the Unity editor.
    // For this example, we are using the two demo clips found in Assets/Haptics.
    // Haptic clips can be designed in Haptics Studio (https://developer.oculus.com/experimental/exp-haptics-studio)
    [SerializeField] private HapticClip clip1;
    [SerializeField] private HapticClip clip2;
    private HapticClipPlayer leftClipPlayer1;
    private HapticClipPlayer leftClipPlayer2;
    private HapticClipPlayer rightClipPlayer1;
    private HapticClipPlayer rightClipPlayer2;

    protected virtual void Start()
    {
        // We create two haptic clip players for each hand.
        leftClipPlayer1 = new HapticClipPlayer(clip1);
        leftClipPlayer2 = new HapticClipPlayer(clip2);
        rightClipPlayer1 = new HapticClipPlayer(clip1);
        rightClipPlayer2 = new HapticClipPlayer(clip2);

        // We increase the priority for the second player on both hands.
        leftClipPlayer2.priority = 1;
        rightClipPlayer2.priority = 1;
    }

    public void PlayFirstClip(Controller hand)
    {
        switch (hand)
        {
            case Controller.Right:
                rightClipPlayer1.Play(Controller.Right);
                break;
            case Controller.Left:
                leftClipPlayer1.Play(Controller.Left);
                break;
            default:
                Debug.LogWarning("Input hand not mapped for: " + hand);
                break;
        }
        Debug.Log("Should feel vibration from clipPlayer1 on " + hand + " controller.");
    }
    public void StopFirstClip(Controller hand)
    {
        switch (hand)
        {
            case Controller.Right:
                rightClipPlayer1.Stop();
                break;
            case Controller.Left:
                leftClipPlayer1.Stop();
                break;
            default:
                Debug.LogWarning("Input hand not mapped for: " + hand);
                break;
        }
        Debug.Log("Vibration from clipPlayer1 should stop on hand " + hand + ".");
    }

    public void SetLoopingOfFirstClip(Controller hand)
    {
        switch (hand)
        {
            case Controller.Right:
                rightClipPlayer1.isLooping = !rightClipPlayer1.isLooping;
                Debug.Log(String.Format("Looping should be {0} on " + hand + " controller.", rightClipPlayer1.isLooping));
                break;
            case Controller.Left:
                leftClipPlayer1.isLooping = !leftClipPlayer1.isLooping;
                Debug.Log(String.Format("Looping should be {0} on " + hand + " controller.", leftClipPlayer1.isLooping));
                break;
            default:
                Debug.LogWarning("Input hand not mapped for: " + hand);
                break;
        }
    }

    /// <summary>
    /// Modulates amplitude and frequency of first clip, the x axis manages the frequency, the y axis the amplitude.
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="input"></param>
    public void ModulateAmplitudeAndFrequencyOfFirstClip(Controller hand, Vector2 input)
    {
        switch (hand)
        {
            case Controller.Right:
                rightClipPlayer1.amplitude = input.y;
                rightClipPlayer1.frequencyShift = input.x;
                break;
            case Controller.Left:
                leftClipPlayer1.amplitude = input.y;
                leftClipPlayer1.frequencyShift = input.x;
                break;
            default:
                Debug.LogWarning("Input hand not mapped for: " + hand);
                break;
        }
    }

    public void PlaySecondClip(Controller hand)
    {
        switch (hand)
        {
            case Controller.Right:
                rightClipPlayer2.Play(Controller.Right);
                break;
            case Controller.Left:
                leftClipPlayer2.Play(Controller.Left);
                break;
            default:
                Debug.LogWarning("Input hand not mapped for: " + hand);
                break;
        }
        Debug.Log("Should feel vibration from clipPlayer2 on " + hand + " controller.");
    }

    public void StopSecondClip(Controller hand)
    {
        switch (hand)
        {
            case Controller.Right:
                rightClipPlayer2.Stop();
                break;
            case Controller.Left:
                leftClipPlayer2.Stop();
                break;
            default:
                Debug.LogWarning("Input hand not mapped for: " + hand);
                break;
        }
        Debug.Log("Vibration from clipPlayer2 should stop on hand " + hand + ".");
    }

    protected virtual void OnDestroy()
    {
        leftClipPlayer1?.Dispose();
        leftClipPlayer2?.Dispose();
        rightClipPlayer1?.Dispose();
        rightClipPlayer2?.Dispose();
    }

    // Upon exiting the application (or when playmode is stopped) we release the haptic clip players and uninitialize (dispose) the SDK.
    protected virtual void OnApplicationQuit()
    {
        Haptics.Instance.Dispose();
    }
}
