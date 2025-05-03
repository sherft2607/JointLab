// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// It represents the possible status of a Models.NetSyncConnection which
  /// allows multiple clients to connect and communicate with each other in real-
  /// time. The connection provides a way to manage and facilitate real-time
  /// communication and data synchronization between multiple clients in a
  /// networked environment.
  public enum NetSyncConnectionStatus : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This member indicates that the connection of the network sync has been
    /// started and the process is ongoing.
    [Description("CONNECTING")]
    Connecting,

    /// This member indicates that the current status of the network sync
    /// connection is not connected.
    [Description("DISCONNECTED")]
    Disconnected,

    /// This member indicates that the current status of the network sync
    /// connection is connected.
    [Description("CONNECTED")]
    Connected,

  }

}
