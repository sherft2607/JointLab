// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// This is an enum that defines the possible states for the Opus codec's
  /// discontinuous transmission (DTX) feature. It allows you to control whether
  /// the Opus codec uses DTX to conserve battery power and reduce transmission
  /// rate during pauses in the voice chat. It can be used as the type for the
  /// VoipOptions.SetCreateNewConnectionUseDtx()
  public enum VoipDtxState : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This state indicates that the DTX feature is enabled. When enabled, the
    /// Opus codec will only transmit data when a person is speaking, which can
    /// conserve battery power and reduce transmission rate during pauses in the
    /// voice chat.
    [Description("ENABLED")]
    Enabled,

    /// This state indicates that the DTX feature is disabled. When disabled, the
    /// Opus codec will continuously transmit data, even during pauses in the voice
    /// chat.
    [Description("DISABLED")]
    Disabled,

  }

}
