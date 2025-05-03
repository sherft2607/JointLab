// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// This object is retrieved from the Users.GetUserProof() request and will
  /// contain a nonce that is used to verify the identity of the User. Read more
  /// about user verification in our [User Verification
  /// guide](https://developer.oculus.com/documentation/unity/ps-
  /// ownership/#integrate-user-verification)
  ///
  /// NOTE: The nonce is only good for one check and then it is invalidated.
  public class UserProof
  {
    /// A string that is returned from the client that is used to verify the
    /// identity of the User. The nonce can be used with the meta account to the
    /// `https://graph.oculus.com/user_nonce_validate` endpoint to verify identity.
    public readonly string Value;


    public UserProof(IntPtr o)
    {
      Value = CAPI.ovr_UserProof_GetNonce(o);
    }
  }

}
