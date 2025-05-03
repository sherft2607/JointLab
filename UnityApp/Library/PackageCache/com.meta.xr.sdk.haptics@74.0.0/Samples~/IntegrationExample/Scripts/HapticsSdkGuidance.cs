// (c) Meta Platforms, Inc. and affiliates. Confidential and proprietary.

using UnityEngine;
using UnityEngine.UI;

// This script helps a user to be guided through the integration example.
// It provides a text popUp in VR that provides guidance throughout the features of the sample scene.
public class HapticsSdkGuidance : MonoBehaviour
{
    public static int CurrentPopUpIndex { get; private set; }

    [SerializeField] private Text popUpText;

    private void Awake()
    {
        // 0) Hold Index Trigger --> Play haptic sample 1 once
        CurrentPopUpIndex = 0;
        popUpText.text =
                           "Press and hold the Index Trigger on either"
                           + " controller to play the first haptic clip once.";

    }
    public void NextStep()
    {
        CurrentPopUpIndex++;

        switch (CurrentPopUpIndex)
        {
            // 1) Hold Grip Button --> Play haptic sample 2 once
            case 1:
                popUpText.text =
                                "Press and hold the Grip Button on either"
                                + " controller to play the second haptic clip once.";
                break;
            // 2) Set loop on first clip (index) using the B/Y-button
            case 2:
                popUpText.text =
                                "Press B/Y-button to toggle looping on the first clip."
                                + " Press and hold Index Trigger to test.";
                break;
            // 3) Move thumbsticks to modulate haptic clip
            case 3:
                popUpText.text =
                                "...while holding the Index Trigger, move the thumbstick"
                                + " to modulate the playback on that side.";
                break;
            // 4) Test Priority --> Second clip should interrupt first clip
            case 4:
                popUpText.text =
                                "...while looping the first clip, playing back the"
                                + " higher priority second clip should"
                                + " interrupt the first clip's playback.";
                break;
            // 5) End of guide.
            case 5:
                popUpText.text =
                                "That's all for this integration example!";
                break;
            default:
                Debug.LogWarning("Step at index " + CurrentPopUpIndex + " not defined");
                break;
        }
    }
}
