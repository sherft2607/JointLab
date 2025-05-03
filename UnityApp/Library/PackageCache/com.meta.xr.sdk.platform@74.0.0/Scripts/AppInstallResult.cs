// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// Result of installing an app. In case of an error during install process,
  /// the error message contains the string representation of this result. This
  /// is returned from Application.StartAppDownload(),
  /// Application.CancelAppDownload() and
  /// Application.InstallAppUpdateAndRelaunch() APIs.
  public enum AppInstallResult : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// Install of the app failed due to low storage on the device
    [Description("LOW_STORAGE")]
    LowStorage,

    /// Install of the app failed due to a network error
    [Description("NETWORK_ERROR")]
    NetworkError,

    /// Install of the app failed as another install request for this application
    /// is already being processed by the installer
    [Description("DUPLICATE_REQUEST")]
    DuplicateRequest,

    /// Install of the app failed due to an internal installer error
    [Description("INSTALLER_ERROR")]
    InstallerError,

    /// Install of the app failed because the user canceled the install operation
    [Description("USER_CANCELLED")]
    UserCancelled,

    /// Install of the app failed due to a user authorization error
    [Description("AUTHORIZATION_ERROR")]
    AuthorizationError,

    /// Install of the app succeeded
    [Description("SUCCESS")]
    Success,

  }

}
