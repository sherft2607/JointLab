// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// An ID for a Models.User which is unique per Developer Center organization.
  /// This ID allows different apps within the same org to be able to identify
  /// the user. You can retrieve this ID by using Users.GetOrgScopedID().
  public class OrgScopedID
  {
    /// The unique id of the Models.User in each organization, allowing different
    /// apps within the same Developer Center organization to have a consistent id
    /// for the same user.
    public readonly UInt64 ID;


    public OrgScopedID(IntPtr o)
    {
      ID = CAPI.ovr_OrgScopedID_GetID(o);
    }
  }

}
