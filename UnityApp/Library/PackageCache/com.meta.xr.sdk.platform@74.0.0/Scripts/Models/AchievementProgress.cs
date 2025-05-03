// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The unlock progress of a particular achievement can be retrieved using
  /// Achievements.GetAllProgress(). It can be used to display the progress of
  /// each achievement in your game. See the following
  /// [link](https://developer.oculus.com/documentation/unity/ps-achievements/)
  /// for more information.
  public class AchievementProgress
  {
    /// If the type of the achievement is AchievementType.Bitfield it represents
    /// the current bitfield state that the achievement has reached.
    public readonly string Bitfield;
    /// If the type of the achievement is AchievementType.Count, it represents the
    /// current counter state that the achievement has reached.
    public readonly ulong Count;
    /// If the user has already unlocked this achievement.
    public readonly bool IsUnlocked;
    /// The unique string that you use to reference the achievement in your app, as
    /// specified in the developer dashboard. It can be retrieved using
    /// AchievementDefinition#Name.
    public readonly string Name;
    /// If the achievement is unlocked, the time when it was unlocked.
    public readonly DateTime UnlockTime;


    public AchievementProgress(IntPtr o)
    {
      Bitfield = CAPI.ovr_AchievementProgress_GetBitfield(o);
      Count = CAPI.ovr_AchievementProgress_GetCount(o);
      IsUnlocked = CAPI.ovr_AchievementProgress_GetIsUnlocked(o);
      Name = CAPI.ovr_AchievementProgress_GetName(o);
      UnlockTime = CAPI.ovr_AchievementProgress_GetUnlockTime(o);
    }
  }

  /// Represents a paginated list of Models.AchievementProgress elements. It
  /// allows you to easily access and manipulate the elements in the paginated
  /// list, such as the size of the list and if there is a next page of elements
  /// available.
  public class AchievementProgressList : DeserializableList<AchievementProgress> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public AchievementProgressList(IntPtr a) {
      var count = (int)CAPI.ovr_AchievementProgressArray_GetSize(a);
      _Data = new List<AchievementProgress>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new AchievementProgress(CAPI.ovr_AchievementProgressArray_GetElement(a, (UIntPtr)i)));
      }

      _NextUrl = CAPI.ovr_AchievementProgressArray_GetNextUrl(a);
    }

  }
}
