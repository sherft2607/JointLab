// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// It's an enum that represents a list of possible event names that can be
  /// used to track user interactions and other important occurrences within an
  /// application. These events can be used to track user engagement, conversion
  /// rates, and other important metrics within an app. By logging these events,
  /// developers can gain valuable insights into how users interact with their
  /// app and make data-driven decisions to improve the user experience. The log
  /// event parameters can be found in #LogEventParameter.
  public enum LogEventName : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This event is triggered when a user clicks on an advertisement within the
    /// app.
    [Description("AD_CLICK")]
    AdClick,

    /// This event is triggered when an advertisement is displayed to the user
    /// within the app.
    [Description("AD_IMPRESSION")]
    AdImpression,

    /// This event is triggered when a user completes the registration process
    /// within the app. This could include creating an account, verifying email
    /// address, or completing a profile.
    [Description("VR_COMPLETE_REGISTRATION")]
    VrCompleteRegistration,

    /// This event is triggered when a user completes a tutorial or guided
    /// experience within the app.
    [Description("VR_TUTORIAL_COMPLETION")]
    VrTutorialCompletion,

    /// This event is triggered when a user interacts with a contact form or other
    /// contact-related feature within the app.
    [Description("CONTACT")]
    Contact,

    /// This event is triggered when a user customizes a product or item within the
    /// app. This could include selecting options, choosing colors, or adding
    /// features.
    [Description("CUSTOMIZE_PRODUCT")]
    CustomizeProduct,

    /// This event is triggered when a user makes a donation within the app.
    [Description("DONATE")]
    Donate,

    /// This event is triggered when a user interacts with a feature that helps
    /// them find a physical location, such as a store or event.
    [Description("FIND_LOCATION")]
    FindLocation,

    /// This event is triggered when a user rates an item or experience within the
    /// app. This could include rating a product, service, or experience on a
    /// scale.
    [Description("VR_RATE")]
    VrRate,

    /// This event is triggered when a user schedules an appointment or event
    /// within the app.
    [Description("SCHEDULE")]
    Schedule,

    /// This event is triggered when a user performs a search within the app. This
    /// could include searching for a product, service, or information.
    [Description("VR_SEARCH")]
    VrSearch,

    /// This event is triggered when a user starts a free trial or demo of an app
    /// or game.
    [Description("SMART_TRIAL")]
    SmartTrial,

    /// This event is triggered when a user submits an application or form within
    /// the app.
    [Description("SUBMIT_APPLICATION")]
    SubmitApplication,

    /// This event is triggered when a user subscribes to a service or newsletter
    /// within the app.
    [Description("SUBSCRIBE")]
    Subscribe,

    /// This event is triggered when a user views content within the app, such as a
    /// video or article.
    [Description("VR_CONTENT_VIEW")]
    VrContentView,

    /// This event is triggered when the Oculus Platform SDK is initialized within
    /// the app. This could include setting up the SDK, loading assets, or
    /// initializing game engines.
    [Description("VR_SDK_INITIALIZE")]
    VrSdkInitialize,

    /// This event is triggered when the app is granted background status by the
    /// Oculus Platform SDK.
    [Description("VR_SDK_BACKGROUND_STATUS_AVAILABLE")]
    VrSdkBackgroundStatusAvailable,

    /// This event is triggered when the app is denied background status by the
    /// Oculus Platform SDK.
    [Description("VR_SDK_BACKGROUND_STATUS_DENIED")]
    VrSdkBackgroundStatusDenied,

    /// This event is triggered when the app is granted restricted background
    /// status by the Oculus Platform SDK.
    [Description("VR_SDK_BACKGROUND_STATUS_RESTRICTED")]
    VrSdkBackgroundStatusRestricted,

    /// This event is triggered when a user adds payment information within the
    /// app. This could include entering credit card details, linking a PayPal
    /// account, or setting up recurring payments.
    [Description("VR_ADD_PAYMENT_INFO")]
    VrAddPaymentInfo,

    /// This event is triggered when a user adds an item to their cart within the
    /// app. This could include selecting products and choosing quantities.
    [Description("VR_ADD_TO_CART")]
    VrAddToCart,

    /// This event is triggered when a user adds an item to their wishlist within
    /// the app.
    [Description("VR_ADD_TO_WISHLIST")]
    VrAddToWishlist,

    /// This event is triggered when a user begins the checkout process within the
    /// app. This could include reviewing orders, entering shipping information, or
    /// selecting payment methods.
    [Description("VR_INITIATED_CHECKOUT")]
    VrInitiatedCheckout,

    /// This event is triggered when a user completes a purchase within the app.
    /// This could include finalizing an order, confirming payment, or receiving a
    /// confirmation message.
    [Description("VR_PURCHASE")]
    VrPurchase,

    /// This event is triggered when a user updates their catalog or inventory
    /// within the app.
    [Description("VR_CATALOG_UPDATE")]
    VrCatalogUpdate,

    /// This event is triggered when a purchase fails within the app. This could
    /// include an error message, a failed payment, or a cancelled order.
    [Description("VR_PURCHASE_FAILED")]
    VrPurchaseFailed,

    /// This event is triggered when a purchase is restored within the app. This
    /// could include reactivating a subscription, reinstating a service, or
    /// restoring access to a product.
    [Description("VR_PURCHASE_RESTORED")]
    VrPurchaseRestored,

    /// This event is triggered when a user begins the checkout process for a
    /// subscription within the app.
    [Description("SUBSCRIPTION_INITIATED_CHECKOUT")]
    SubscriptionInitiatedCheckout,

    /// This event is triggered when a subscription fails within the app. This
    /// could include an error message, a failed payment, or a cancelled
    /// subscription.
    [Description("SUBSCRIPTION_FAILED")]
    SubscriptionFailed,

    /// This event is triggered when a subscription is restored within the app.
    /// This could include reactivating a subscription, reinstating a service, or
    /// restoring access to a product.
    [Description("SUBSCRIPTION_RESTORE")]
    SubscriptionRestore,

    /// This event is triggered when a user achieves a level or milestone within
    /// the app.
    [Description("VR_LEVEL_ACHIEVED")]
    VrLevelAchieved,

    /// This event is triggered when a user achieves a level or milestone within
    /// the app.
    [Description("VR_ACHIEVEMENT_UNLOCKED")]
    VrAchievementUnlocked,

    /// This event is triggered when a user spends credits or virtual currency
    /// within the app.
    [Description("VR_SPENT_CREDITS")]
    VrSpentCredits,

    /// This event is triggered when a user obtains a push token within the app.
    [Description("VR_OBTAIN_PUSH_TOKEN")]
    VrObtainPushToken,

    /// This event is triggered when a user opens a push notification within the
    /// app.
    [Description("VR_PUSH_OPENED")]
    VrPushOpened,

    /// This event is triggered when a user activates the app. This could include
    /// launching the app, logging in, or starting a new session.
    [Description("VR_ACTIVATE_APP")]
    VrActivateApp,

    /// This event is triggered when a user deactivates the app. This could include
    /// closing the app, logging out, or ending a session.
    [Description("VR_DEACTIVATE_APP")]
    VrDeactivateApp,

  }

}
