// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// It's designed to manage cowatching sessions within a shared virtual home
  /// environment. This state primarily focuses on tracking whether a user is
  /// currently participating in a cowatching session. If there is any change in
  /// the cowatching state, it can be retrieved using
  /// Message::MessageType::Notification_Cowatching_InSessionChanged.
  public class CowatchingState
  {
    /// A `boolean` indicates if the current user is in a cowatching session. It
    /// can be retrieved using Cowatching.IsInSession().
    public readonly bool InSession;


    public CowatchingState(IntPtr o)
    {
      InSession = CAPI.ovr_CowatchingState_GetInSession(o);
    }
  }

}
