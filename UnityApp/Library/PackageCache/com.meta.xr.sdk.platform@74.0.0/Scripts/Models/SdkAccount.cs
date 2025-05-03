// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// A single user can have multiple sdk accounts associated with it. SDK
  /// accounts represent the oculus user and x-accounts that are linked to the
  /// particular user. Retrieve the SDK accounts by using,
  /// Users.GetSdkAccounts().
  public class SdkAccount
  {
    /// The specific type of account that this sdk account represents. The type can
    /// be one of the following:
    ///
    /// SdkAccountType.Unknown
    ///
    /// SdkAccountType.Oculus
    ///
    /// SdkAccountType.FacebookGameroom
    public readonly SdkAccountType AccountType;
    /// The ID of the user, User#ID, of the sdk account. This is a unique value for
    /// every Models.User.
    public readonly UInt64 UserId;


    public SdkAccount(IntPtr o)
    {
      AccountType = CAPI.ovr_SdkAccount_GetAccountType(o);
      UserId = CAPI.ovr_SdkAccount_GetUserId(o);
    }
  }

  /// Represents a paginated list of Models.SdkAccount elements. It allows you to
  /// easily access and manipulate the elements in the paginated list, such as
  /// the size of the list and if there is a next page of elements available.
  public class SdkAccountList : DeserializableList<SdkAccount> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public SdkAccountList(IntPtr a) {
      var count = (int)CAPI.ovr_SdkAccountArray_GetSize(a);
      _Data = new List<SdkAccount>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new SdkAccount(CAPI.ovr_SdkAccountArray_GetElement(a, (UIntPtr)i)));
      }

    }

  }
}
