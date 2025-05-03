// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Represents a linked account that is associated with the Models.User's
  /// account in the system. It provides a way to store and manage information
  /// about linked accounts in the system, allowing users to easily access and
  /// manage their data or services from multiple platforms in one place
  public class LinkedAccount
  {
    /// Access token of the linked account. This token is used to authenticate the
    /// user on the service provider's platform and grant access to their data or
    /// services.
    public readonly string AccessToken;
    /// Service provider with which the linked account is associated. There are
    /// several possible service providers that can be found in #ServiceProvider.
    public readonly ServiceProvider ServiceProvider;
    /// A unique identifier represents the user ID of the linked account. It can be
    /// retrieved using User#ID
    public readonly string UserId;


    public LinkedAccount(IntPtr o)
    {
      AccessToken = CAPI.ovr_LinkedAccount_GetAccessToken(o);
      ServiceProvider = CAPI.ovr_LinkedAccount_GetServiceProvider(o);
      UserId = CAPI.ovr_LinkedAccount_GetUserId(o);
    }
  }

  /// Represents a paginated list of Models.LinkedAccount elements. It allows you
  /// to easily access and manipulate the elements in the paginated list, such as
  /// the size of the list and if there is a next page of elements available.
  public class LinkedAccountList : DeserializableList<LinkedAccount> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public LinkedAccountList(IntPtr a) {
      var count = (int)CAPI.ovr_LinkedAccountArray_GetSize(a);
      _Data = new List<LinkedAccount>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new LinkedAccount(CAPI.ovr_LinkedAccountArray_GetElement(a, (UIntPtr)i)));
      }

    }

  }
}
