// (c) Meta Platforms, Inc. and affiliates. Confidential and proprietary.

using Oculus.Haptics;
using System;
using UnityEngine;

public class HapticsSdkOVRControllerGuidance : MonoBehaviour
{
    [SerializeField] private HapticsSdkGuidance hapticsSdkGuidance;
    [SerializeField] private HapticsSdkPlaySample hapticsSdkPlaySample;
    void Update()
    {
        HandleHapticsSdkGuidanceStep();
        HandleControllerInput(OVRInput.Controller.LTouch, Controller.Left);
        HandleControllerInput(OVRInput.Controller.RTouch, Controller.Right);
    }

    private void HandleHapticsSdkGuidanceStep()
    {
        switch (HapticsSdkGuidance.CurrentPopUpIndex)
        {
            case 0:
            case 1:
            case 4:
                if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch) ||
                                OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
                {
                    hapticsSdkGuidance.NextStep();
                }
                break;
            case 2:
                if (OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.LTouch) ||
                                OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.RTouch))
                {
                    hapticsSdkGuidance.NextStep();
                }
                break;
            case 3:
                if (OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y != 0.0 ||
                                OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y != 0.0)
                {
                    hapticsSdkGuidance.NextStep();
                }
                break;
            default:
                break;
        }
    }

    // This section provides a series of interactions that showcase the playback and modulation capabilities of the
    // Haptics SDK.
    void HandleControllerInput(OVRInput.Controller controller, Controller hand)
    {
        try
        {
            // Play first clip with default priority using the index trigger
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
            {
                hapticsSdkPlaySample.PlayFirstClip(hand);
            }

            // Play second clip with higher priority using the grab button
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller))
            {
                hapticsSdkPlaySample.PlaySecondClip(hand);
            }

            // Stop first clip when releasing the index trigger
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controller))
            {
                hapticsSdkPlaySample.StopFirstClip(hand);
            }

            // Stop second clip when releasing the grab button
            if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, controller))
            {
                hapticsSdkPlaySample.StopSecondClip(hand);
            }

            // Loop first clip using the B/Y-button
            if (OVRInput.GetDown(OVRInput.Button.Two, controller))
            {
                hapticsSdkPlaySample.SetLoopingOfFirstClip(hand);
            }

            // Modulate the amplitude and frequency of the first clip using the thumbstick
            // - Moving left/right modulates the frequency shift
            // - Moving up/down modulates the amplitude
            if (controller == OVRInput.Controller.LTouch)
            {
                Vector2 thumbstickInput = new Vector2(OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).x,
                    Mathf.Clamp(1.0f + OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y, 0.0f, 2.0f));
                hapticsSdkPlaySample.ModulateAmplitudeAndFrequencyOfFirstClip(hand, thumbstickInput);
            }
            else if (controller == OVRInput.Controller.RTouch)
            {
                Vector2 thumbstickInput = new Vector2(OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).x,
                    Mathf.Clamp(1.0f + OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y, 0.0f, 2.0f));
                hapticsSdkPlaySample.ModulateAmplitudeAndFrequencyOfFirstClip(hand, thumbstickInput);
            }
        }

        // If any exceptions occur, we catch and log them here.
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
