// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// Determines the type of the achievement. This enum defines the different
  /// types of achievements that can be used in the game. Each type has its own
  /// unique characteristics and requirements for unlocking. See more details
  /// about achievement
  /// [here](https://developer.oculus.com/documentation/unity/ps-achievements/).
  public enum AchievementType : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// Simple achievements are unlocked by a single event or objective completion.
    /// They are often used to reward players for completing specific tasks or
    /// milestones within the game.
    [Description("SIMPLE")]
    Simple,

    /// Bitfield achievements are unlocked when a target number of bits are set
    /// within a bitfield.
    [Description("BITFIELD")]
    Bitfield,

    /// Count achievements are unlocked when a counter reaches a defined target.
    /// The counter is incremented each time the player completes the required
    /// action, and when it reaches the target value, the achievement is unlocked.
    [Description("COUNT")]
    Count,

  }

}
