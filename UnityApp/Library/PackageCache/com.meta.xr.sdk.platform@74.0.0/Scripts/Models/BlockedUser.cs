// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// It contains an array of users who have been blocked by the logged in user.
  /// You can't follow, be followed, invited, or searched by a blocked user. It
  /// can be retrieved using Users.GetBlockedUsers().
  public class BlockedUser
  {
    /// It represents the user ID that has been blocked by the logged in user. It
    /// is a type of ID and can be retrieved using User#ID.
    public readonly UInt64 Id;


    public BlockedUser(IntPtr o)
    {
      Id = CAPI.ovr_BlockedUser_GetId(o);
    }
  }

  /// Represents a paginated list of Models.BlockedUser elements. It allows you
  /// to easily access and manipulate the elements in the paginated list, such as
  /// the size of the list and if there is a next page of elements available.
  public class BlockedUserList : DeserializableList<BlockedUser> {
    /// Instantiates a C# wrapper class that wraps a native list by pointer. Used internally by Platform SDK to wrap the list.
    public BlockedUserList(IntPtr a) {
      var count = (int)CAPI.ovr_BlockedUserArray_GetSize(a);
      _Data = new List<BlockedUser>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new BlockedUser(CAPI.ovr_BlockedUserArray_GetElement(a, (UIntPtr)i)));
      }

      _NextUrl = CAPI.ovr_BlockedUserArray_GetNextUrl(a);
    }

  }
}
