// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// It's a enum that represent the different types of audiences that can be
  /// selected for a livestreaming. It can be used to specify the audience when a
  /// live streaming starts. The livestreaming status change will be notified by
  /// Message::MessageType::Notification_Livestreaming_StatusChange
  public enum LivestreamingAudience : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This value represents a public audience, meaning that anyone can view the
    /// livestream.
    [Description("PUBLIC")]
    Public,

    /// This value represents an audience consisting of the user's friends. Only
    /// people who are friends with the user will be able to view the livestream.
    [Description("FRIENDS")]
    Friends,

    /// This value represents an audience consisting only of the user themselves.
    /// Only the user who created the livestream will be able to view it.
    [Description("ONLY_ME")]
    OnlyMe,

  }

}
