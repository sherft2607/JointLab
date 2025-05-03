// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// It's an enum that represents the status of a microphone during a
  /// livestream. It provides a simple and standardized way to represent the
  /// status of a microphone during a livestream, which can be useful for various
  /// applications and use cases. See
  /// [livestreaming](https://developer.oculus.com/blog/sharing-via-
  /// livestreaming-now-available-for-rift-applications/) documentation for more
  /// details.
  public enum LivestreamingMicrophoneStatus : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// It indicates that the microphone is currently on and transmitting audio.
    /// This value can be used to indicate that the microphone is functioning
    /// properly and that audio is being captured and transmitted during the
    /// livestream.
    [Description("MICROPHONE_ON")]
    MicrophoneOn,

    /// It indicates that the microphone is currently off and not transmitting
    /// audio. This value can be used to indicate that the microphone has been
    /// turned off or muted, either intentionally or unintentionally, during the
    /// livestream.
    [Description("MICROPHONE_OFF")]
    MicrophoneOff,

  }

}
