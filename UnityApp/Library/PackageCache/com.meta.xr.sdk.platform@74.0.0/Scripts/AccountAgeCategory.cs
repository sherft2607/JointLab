// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// The age category in a Meta account is used to determine eligibility for
  /// certain features and services. This information is important for ensuring
  /// that users are able to access the appropriate content and functionality
  /// based on their age. The values are used in Models.UserAccountAgeCategory
  /// API. See more details
  /// [here](https://developer.oculus.com/documentation/unity/ps-get-age-
  /// category-api).
  public enum AccountAgeCategory : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// Child age group for users between the ages of 10-12 (or applicable age in
    /// user's region).
    [Description("CH")]
    Ch,

    /// Teenage age group for users between the ages of 13-17 (or applicable age in
    /// user's region).
    [Description("TN")]
    Tn,

    /// Adult age group for users ages 18 and up (or applicable age in user's
    /// region).
    [Description("AD")]
    Ad,

  }

}
