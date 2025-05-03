// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// This payload contains information about the result of an update request to
  /// the user data store. It includes a success field
  /// UserDataStoreUpdateResponse#Success that indicates whether the update was
  /// successful or not. See more info about Platform Solutions
  /// [here](https://developer.oculus.com/documentation/unity/ps-platform-
  /// intro/).
  public class UserDataStoreUpdateResponse
  {
    /// Indicates whether the update request was successful or not. A value of true
    /// indicates that the update was successful, while a value of false indicates
    /// that the update failed.
    public readonly bool Success;


    public UserDataStoreUpdateResponse(IntPtr o)
    {
      Success = CAPI.ovr_UserDataStoreUpdateResponse_GetSuccess(o);
    }
  }

}
