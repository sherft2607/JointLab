// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// An enum that represents the type of media content being shared, which can
  /// be useful for various purposes such as displaying the media content in a
  /// specific way or applying certain filters or effects to it. It can be used
  /// as a type of parameter in Media.ShareToFacebook().
  public enum MediaContentType : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// Indicates that the media content is a photo. This value can be used to
    /// specify that the media content being shared is a photo.
    [Description("PHOTO")]
    Photo,

  }

}
