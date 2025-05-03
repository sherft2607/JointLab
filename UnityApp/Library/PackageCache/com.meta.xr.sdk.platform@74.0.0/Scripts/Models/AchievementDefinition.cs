// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// An AchievementDefinition defines an achievement; this includes its name and
  /// how it is unlocked. For an individual user's progress in unlocking an
  /// achievement, see AchievementProgress. It can be retrieved using
  /// Achievements.GetAllDefinitions().
  public class AchievementDefinition
  {
    /// This is the type of achievement. There are three types of achievement:
    /// AchievementType.Simple - unlocked by completion of a single event or
    /// objective, AchievementType.Bitfield - unlocked when a number of bits in a
    /// bitfield are set, and AchievementType.Count - unlocked when a counter
    /// reaches a defined target.
    public readonly AchievementType Type;
    /// A string of the api name of the achievement. It can be used to get the
    /// achievement progress by name by the function
    /// Achievements.GetProgressByName().
    public readonly string Name;
    /// It is required for bitfield achievements: AchievementType.Bitfield. This
    /// represents the size of the bitfield for this achievement.
    public readonly uint BitfieldLength;
    public readonly ulong Target;


    public AchievementDefinition(IntPtr o)
    {
      Type = CAPI.ovr_AchievementDefinition_GetType(o);
      Name = CAPI.ovr_AchievementDefinition_GetName(o);
      BitfieldLength = CAPI.ovr_AchievementDefinition_GetBitfieldLength(o);
      Target = CAPI.ovr_AchievementDefinition_GetTarget(o);
    }
  }

  /// Represents a paginated list of Models.AchievementDefinition elements. It
  /// allows you to easily access and manipulate the elements in the paginated
  /// list, such as the size of the list and if there is a next page of elements
  /// available.
  public class AchievementDefinitionList : DeserializableList<AchievementDefinition> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public AchievementDefinitionList(IntPtr a) {
      var count = (int)CAPI.ovr_AchievementDefinitionArray_GetSize(a);
      _Data = new List<AchievementDefinition>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new AchievementDefinition(CAPI.ovr_AchievementDefinitionArray_GetElement(a, (UIntPtr)i)));
      }

      _NextUrl = CAPI.ovr_AchievementDefinitionArray_GetNextUrl(a);
    }

  }
}
