// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Represents the version information for an application. The information
  /// includes the date of latest release, the size of the latest release and the
  /// application name and version code of currently installed version and the
  /// latest release. You can retrieve it with Application.GetVersion().
  public class ApplicationVersion
  {
    /// The version code number for the version of the application that is
    /// currently installed on the device.
    public readonly int CurrentCode;
    /// The version name string for the version of the application that is
    /// currently installed on the device.
    public readonly string CurrentName;
    /// Version code number of the latest update of the application. This may or
    /// may not be currently installed on the device.
    public readonly int LatestCode;
    /// Version name string of the latest update of the application. This may or
    /// may not be currently installed on the device.
    public readonly string LatestName;
    /// Seconds since epoch when the latest application update was released. You
    /// need to convert this date to a human readable format before displaying it
    /// to the application users.
    public readonly long ReleaseDate;
    /// Size of the latest application update in bytes.
    public readonly string Size;


    public ApplicationVersion(IntPtr o)
    {
      CurrentCode = CAPI.ovr_ApplicationVersion_GetCurrentCode(o);
      CurrentName = CAPI.ovr_ApplicationVersion_GetCurrentName(o);
      LatestCode = CAPI.ovr_ApplicationVersion_GetLatestCode(o);
      LatestName = CAPI.ovr_ApplicationVersion_GetLatestName(o);
      ReleaseDate = CAPI.ovr_ApplicationVersion_GetReleaseDate(o);
      Size = CAPI.ovr_ApplicationVersion_GetSize(o);
    }
  }

}
