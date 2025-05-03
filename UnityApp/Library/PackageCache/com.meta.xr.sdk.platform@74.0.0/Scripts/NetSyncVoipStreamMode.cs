// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// A network synchronization system allows multiple clients to connect and
  /// communicate with each other in real-time. Once a Models.NetSyncConnection
  /// is established, user can choose the stream mode for the connection. The
  /// NetSyncOptions.SetVoipStreamDefault() will be used when a new VoIP(Voice
  /// over Internet Protocol) user connects.
  public enum NetSyncVoipStreamMode : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This `enum` member represents the ambisonic steam mode the VoIP stream
    /// uses. It is the default value of NetSyncOptions.SetVoipStreamDefault().
    /// Since it allows for the creation of immersive, surround sound experiences
    /// that simulate real-world audio environments, it typically used in virtual
    /// reality (VR) and augmented reality (AR) applications.
    [Description("AMBISONIC")]
    Ambisonic,

    /// This `enum` member represents the mono steam mode the VoIP stream uses. The
    /// advantages mono stream mode has over ambisonic steam mode is the audio
    /// encoding and decoding require less computational resources and thus audio
    /// streams require less bandwidth. So it is typically used in applications
    /// with limited network resources.
    [Description("MONO")]
    Mono,

  }

}
