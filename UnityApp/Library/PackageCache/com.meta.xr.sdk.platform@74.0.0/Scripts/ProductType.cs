// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// This `enum` value represents the possible types of a Models.Product, which
  /// is an item purchased in the application. An In-app purchase (IAP) add-on
  /// can be durable, consuable, or a subscription.
  public enum ProductType : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This product is a durable IAP item that can be consumed multiple times. It
    /// can be purchased only once.
    [Description("DURABLE")]
    DURABLE,

    /// This product is an IAP item that can be consumed only once. It can only be
    /// purchased again after it is consumed.
    [Description("CONSUMABLE")]
    CONSUMABLE,

    /// This product represents a subscription. Subscriptions provide a way for
    /// users to purchase your app or its premium content by way of a recurring
    /// payment model.
    [Description("SUBSCRIPTION")]
    SUBSCRIPTION,

  }

}
