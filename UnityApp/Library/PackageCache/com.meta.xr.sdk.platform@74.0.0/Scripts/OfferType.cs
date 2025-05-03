// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// An enumeration that defines the type of the Models.TrialOffer. This can be
  /// utilized to identify the specific type of offer, such as a free trial or an
  /// intro offer. By setting this field, you can easily differentiate between
  /// different types of offers and provide a better user experience for your
  /// customers.
  public enum OfferType : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This value indicates that the offer is an intro offer, which is typically a
    /// special promotion or discount offered to new customers.
    [Description("INTRO_OFFER")]
    INTROOFFER,

    /// This value indicates that the offer is a free trial, which allows customers
    /// to try out a product or service without paying for it.
    [Description("FREE_TRIAL")]
    FREETRIAL,

  }

}
