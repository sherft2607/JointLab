// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  /// It's an enum about a list of possible parameters that can be logged for an
  /// event. See more information about log event in #LogEventName. Each member
  /// of the enum represents a specific parameter that can be logged.
  public enum LogEventParameter : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// This parameter represents the currency used in a virtual reality (VR)
    /// transaction and it's important for tracking and analyzing VR transactions
    /// across different regions and currencies.
    [Description("VR_CURRENCY")]
    VrCurrency,

    /// This parameter represents the method used to register for a VR service or
    /// application. It helps to understand how users are accessing VR services and
    /// applications.
    [Description("VR_REGISTRATION_METHOD")]
    VrRegistrationMethod,

    /// This parameter represents the type of content being accessed or interacted
    /// with in a VR environment. It helps to understand what types of content are
    /// most popular among VR users.
    [Description("VR_CONTENT_TYPE")]
    VrContentType,

    /// This parameter represents the specific content being accessed or interacted
    /// with in a VR environment.
    [Description("VR_CONTENT")]
    VrContent,

    /// This parameter represents the unique identifier for the content being
    /// accessed or interacted with in a VR environment.
    [Description("VR_CONTENT_ID")]
    VrContentId,

    /// This parameter represents the search query entered by the user in a VR
    /// environment. It helps to understand what users are searching for and how
    /// they are interacting with VR search functionality.
    [Description("VR_SEARCH_STRING")]
    VrSearchString,

    /// This parameter represents whether an action was successful or not in a VR
    /// environment.
    [Description("VR_SUCCESS")]
    VrSuccess,

    /// This parameter represents the maximum rating value allowed for a particular
    /// VR content or service.
    [Description("VR_MAX_RATING_VALUE")]
    VrMaxRatingValue,

    /// This parameter represents whether payment information is available for a VR
    /// transaction. It helps to understand whether users have the necessary
    /// payment information to complete a VR transaction.
    [Description("VR_PAYMENT_INFO_AVAILABLE")]
    VrPaymentInfoAvailable,

    /// This parameter represents the number of items involved.
    [Description("VR_NUM_ITEMS")]
    VrNumItems,

    /// This parameter represents the level or stage reached by the user in a VR
    /// game or application.
    [Description("VR_LEVEL")]
    VrLevel,

    /// This parameter represents a brief description of the VR content or service
    /// being accessed or interacted with.
    [Description("VR_DESCRIPTION")]
    VrDescription,

    /// This parameter represents the type of advertisement displayed to the user
    /// in a VR environment.
    [Description("AD_TYPE")]
    AdType,

    /// This parameter represents the unique identifier for a VR order or
    /// transaction. It helps to track and analyze individual VR transactions and
    /// orders.
    [Description("VR_ORDER_ID")]
    VrOrderId,

    /// This parameter represents the name of the event being logged.
    [Description("EVENT_NAME")]
    EventName,

    /// This parameter represents the timestamp when the event was logged. It helps
    /// to understand when specific events occurred and how they relate to other
    /// events in the VR environment.
    [Description("LOG_TIME")]
    LogTime,

    /// This parameter represents whether the event was implicitly logged or
    /// explicitly logged by the user.
    [Description("IMPLICITLY_LOGGED")]
    ImplicitlyLogged,

    /// This parameter represents whether the event occurred while the application
    /// was running in the background or foreground.
    [Description("IN_BACKGROUND")]
    InBackground,

    /// This parameter represents the campaign or promotion associated with a VR
    /// push notification. It helps to understand the context and purpose of VR
    /// push notifications.
    [Description("VR_PUSH_CAMPAIGN")]
    VrPushCampaign,

    /// This parameter represents the action taken by the user on a VR push
    /// notification, such as clicking or dismissing.
    [Description("VR_PUSH_ACTION")]
    VrPushAction,

    /// This parameter represents the type of in-app purchase (IAP) product being
    /// purchased or interacted with in a VR environment.
    [Description("VR_IAP_PRODUCT_TYPE")]
    VrIapProductType,

    /// This parameter represents the title of the VR content being accessed or
    /// interacted with. It helps to provide context and understanding of what the
    /// VR experience entails.
    [Description("VR_CONTENT_TITLE")]
    VrContentTitle,

    /// This parameter represents the unique identifier for a VR transaction. It
    /// helps to track and analyze individual VR transactions and orders.
    [Description("VR_TRANSACTION_ID")]
    VrTransactionId,

    /// This parameter represents the date and time of a VR transaction. It helps
    /// to understand when specific transactions occurred.
    [Description("VR_TRANSACTION_DATE")]
    VrTransactionDate,

    /// This parameter represents the subscription period for an in-app purchase
    /// (IAP) product in a VR environment, such as monthly or annually.
    [Description("VR_IAP_SUBS_PERIOD")]
    VrIapSubsPeriod,

    /// This parameter represents whether the user is starting a free trial for an
    /// IAP product in a VR environment.
    [Description("VR_IAP_IS_START_TRIAL")]
    VrIapIsStartTrial,

    /// This parameter represents whether an IAP product has a free trial available
    /// in a VR environment.
    [Description("VR_IAP_HAS_FREE_TRIAL")]
    VrIapHasFreeTrial,

    /// This parameter represents the length of the free trial period for an IAP
    /// product in a VR environment.
    [Description("VR_IAP_TRIAL_PERIOD")]
    VrIapTrialPeriod,

    /// This parameter represents the price of the IAP product during the free
    /// trial period in a VR environment.
    [Description("VR_IAP_TRIAL_PRICE")]
    VrIapTrialPrice,

    /// This parameter represents the unique identifier for a user session in a VR
    /// environment. It helps to track and analyze individual user sessions and
    /// understand how users are interacting with VR applications and services over
    /// time.
    [Description("SESSION_ID")]
    SessionId,

  }

}
