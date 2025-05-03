// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// This property indicates the current status of the app on the device. It is
  /// important to note that an app can only query its own status, not the status
  /// of other apps installed on the device. It can be retrieved by
  /// AppDownloadProgressResult#StatusCode.
  public enum AppStatus : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// The user possesses a valid entitlement for the app, indicating they have
    /// the right to install it, although it is not currently installed on the
    /// device.
    [Description("ENTITLED")]
    Entitled,

    /// The app is scheduled for download. The download will start as soon as prior
    /// queued downloads are completed.
    [Description("DOWNLOAD_QUEUED")]
    DownloadQueued,

    /// The app is currently being downloaded to the device. This status is active
    /// during the download process until it is complete.
    [Description("DOWNLOADING")]
    Downloading,

    /// The app is currently being installed on the device. This status remains
    /// until the installation is fully completed.
    [Description("INSTALLING")]
    Installing,

    /// The app is successfully installed on the device and is ready to be used.
    [Description("INSTALLED")]
    Installed,

    /// The app is currently being uninstalled from the device. This status remains
    /// until the uninstallation process is complete.
    [Description("UNINSTALLING")]
    Uninstalling,

    /// The installation of the app is scheduled and will commence once any prior
    /// installations are completed.
    [Description("INSTALL_QUEUED")]
    InstallQueued,

  }

}
