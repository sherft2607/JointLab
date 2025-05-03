// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// Describes the creator of the associated challenge. This field indicates who
  /// created the challenge, whether it was ChallengeCreationType.UserCreated or
  /// ChallengeCreationType.DeveloperCreated. Understanding the creator of the
  /// associated challenge can provide valuable context and help participants
  /// better understand the nature and purpose of the challenge.
  public enum ChallengeCreationType : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// The challenge was created by a User. This means that a regular user of the
    /// app created the challenge, and it may be a community-driven challenge or a
    /// personal challenge created by the user for themselves or others.
    [Description("USER_CREATED")]
    UserCreated,

    /// The challenge was created by the app developer. This means that the
    /// challenge was created by the team behind the app, and it may be an official
    /// challenge or a special event created by the developers to engage with the
    /// community or promote specific features of the app.
    [Description("DEVELOPER_CREATED")]
    DeveloperCreated,

  }

}
