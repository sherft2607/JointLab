// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Represents an update to an existing achievement. It will be the payload if
  /// there is any updates on achievements, as unlocking an achievement by
  /// Achievements.Unlock(), adding 'count' to the achievement by
  /// Achievements.AddCount(), and unlocking fields of a BITFIELD achievement by
  /// Achievements.AddFields().
  public class AchievementUpdate
  {
    /// This indicates if this update caused the achievement to unlock.
    public readonly bool JustUnlocked;
    /// The unique AchievementDefinition#Name used to reference the updated
    /// achievement, as specified in the developer dashboard.
    public readonly string Name;


    public AchievementUpdate(IntPtr o)
    {
      JustUnlocked = CAPI.ovr_AchievementUpdate_GetJustUnlocked(o);
      Name = CAPI.ovr_AchievementUpdate_GetName(o);
    }
  }

}
