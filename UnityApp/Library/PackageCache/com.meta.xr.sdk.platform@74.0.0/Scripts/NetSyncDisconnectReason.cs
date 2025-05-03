// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// It represents the possible reasons why the status of a
  /// Models.NetSyncConnection, which allows multiple clients to connect and
  /// communicate with each other in real-time, is disconnected. The connection
  /// provides a way to manage and facilitate real-time communication and data
  /// synchronization between multiple clients in a networked environment.
  public enum NetSyncDisconnectReason : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This member indicates that the disconnect of the Models.NetSyncConnection
    /// was initialized from the a user request.
    [Description("LOCAL_TERMINATED")]
    LocalTerminated,

    /// This member indicates that the connection of the Models.NetSyncConnection
    /// was shutdown by the server intentionally.
    [Description("SERVER_TERMINATED")]
    ServerTerminated,

    /// This member indicates that the initial connection request never succeeded.
    [Description("FAILED")]
    Failed,

    /// This member indicates that the Models.NetSyncConnection was shutdown
    /// because of the network timeout.
    [Description("LOST")]
    Lost,

  }

}
