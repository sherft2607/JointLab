// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// A network synchronization system allows multiple clients to connect and
  /// communicate with each other in real-time. Once a Models.NetSyncConnection
  /// is established, it uses Voice over Internet Protocol to allow users to make
  /// voice calls using the internet. The mic source will represent the current
  /// source of the mic in the call.
  public enum NetSyncVoipMicSource : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This `enum` member represents no net sync connection has been identified as
    /// the source from which the data was sent to the server
    [Description("NONE")]
    None,

    /// This `enum` member represents an internal net sync connection has been
    /// identified as the source from which the data was sent to the server
    [Description("INTERNAL")]
    Internal,

  }

}
