// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// An enumeration that defines the type of the Models.TrialOffer. This can be
  /// utilized to determine the length of time for which the offer is valid. By
  /// setting this field, you can easily manage the duration of your offers and
  /// provide a better user experience for your customers.
  public enum OfferTerm : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// Represents that the offer term is weekly. This means that the offer will be
    /// valid for a period of one week from the date of purchase.
    [Description("WEEKLY")]
    WEEKLY,

    /// Represents that the offer term is biweekly. This means that the offer will
    /// be valid for a period of two weeks from the date of purchase.
    [Description("BIWEEKLY")]
    BIWEEKLY,

    /// Represents that the offer term is monthly. This means that the offer will
    /// be valid for a period of one month from the date of purchase.
    [Description("MONTHLY")]
    MONTHLY,

    /// Represents that the offer term is quarterly. This means that the offer will
    /// be valid for a period of three months from the date of purchase.
    [Description("QUARTERLY")]
    QUARTERLY,

    /// Represents that the offer term is every 6 months. This means that the offer
    /// will be valid for a period of six months from the date of purchase.
    [Description("SEMIANNUAL")]
    SEMIANNUAL,

    /// Represents that the offer term is annual. This means that the offer will be
    /// valid for a period of one year from the date of purchase.
    [Description("ANNUAL")]
    ANNUAL,

    /// Represents that the offer term is every 2 years. This means that the offer
    /// will be valid for a period of two years from the date of purchase.
    [Description("BIANNUAL")]
    BIANNUAL,

  }

}
