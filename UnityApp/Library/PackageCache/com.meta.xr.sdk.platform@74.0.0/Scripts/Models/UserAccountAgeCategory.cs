// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// A UserAccountAgeCategory represents the age category of a Meta user. This
  /// object contains information about the user's age group, which can be used
  /// for various purposes such as targeted advertising or content restriction.
  /// The possible values for the age category are defined in the
  /// #AccountAgeCategory.
  public class UserAccountAgeCategory
  {
    /// Age category of the user in Meta account. This field represents the age
    /// group that the user falls into, and the possible values for this field are
    /// defined in the #AccountAgeCategory.
    public readonly AccountAgeCategory AgeCategory;


    public UserAccountAgeCategory(IntPtr o)
    {
      AgeCategory = CAPI.ovr_UserAccountAgeCategory_GetAgeCategory(o);
    }
  }

}
