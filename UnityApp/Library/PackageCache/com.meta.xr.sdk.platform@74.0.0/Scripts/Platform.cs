// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]

namespace Oculus.Platform
{
  using UnityEngine;
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Runtime.InteropServices;

  /// The Core class provides methods for initializing and checking the status of the Oculus Platform SDK.
  /// This class is responsible for setting up the necessary components and configurations to enable
  /// communication with the Oculus Platform, allowing developers to access features such as Entitlement checks: Entitlements.IsUserEntitledToApplication().
  /// The Core class also provides methods for checking the initialization status of the platform,
  /// allowing developers to determine whether the platform is ready for use.
  public sealed class Core {
    private static bool IsPlatformInitialized = false;

    /// Returns whether the Oculus Platform SDK has been successfully initialized.
    /// True if the platform is initialized, false otherwise.
    public static bool IsInitialized()
    {
      return IsPlatformInitialized;
    }

    /// If true, the contents of each request response will be printed using Debug.Log. This can allocate a lot of heap memory, so it should only be used for testing and debugging purposes.
    public static bool LogMessages = false;

    /// The error message that will be displayed to the user if the platform is not initialized.
    public static string PlatformUninitializedError = "This function requires an initialized Oculus Platform. Run Oculus.Platform.Core.[Initialize|AsyncInitialize] and try again.";

    internal static void ForceInitialized()
    {
      IsPlatformInitialized = true;
    }

    private static string getAppID(string appId = null) {
      string configAppID = GetAppIDFromConfig();
      if (String.IsNullOrEmpty(appId))
      {
        if (String.IsNullOrEmpty(configAppID))
        {
          throw new UnityException("Update your app id by selecting 'Oculus Platform' -> 'Edit Settings'");
        }
        appId = configAppID;
      }
      else
      {
        if (!String.IsNullOrEmpty(configAppID))
        {
          Debug.LogWarningFormat("The 'Oculus App Id ({0})' field in 'Oculus Platform/Edit Settings' is being overridden by the App Id ({1}) that you passed in to Platform.Core.Initialize.  You should only specify this in one place.  We recommend the menu location.", configAppID, appId);
        }
      }
      return appId;
    }

    /// Asynchronously Initialize Platform SDK. The result will be the type of Models.PlatformInitialize.Result.
    ///
    /// While the platform is in an initializing state, it's not fully functional.
    /// [Requests]: will queue up and run once platform is initialized.
    ///    For example: Users.GetLoggedInUser() can be called immediately after
    ///    asynchronous init and once platform is initialized, this request will run
    /// [Synchronous Methods]: will return the default value;
    ///    For example: Users.GetLoggedInUser() will return 0 until platform is
    ///    fully initialized.
    ///
    /// Note that during initialization, some features may not be available or may
    /// return default values. It's important to check the initialization status
    /// before attempting to use certain features or functions.
    /// Use Message.IsError to check if the result is an error.
    /// Use Message.Type to check the message type.
    /// Use Models.PlatformInitialize.Result to get the result of the
    /// initialization.
    /// \param appId The app ID to use for initialization. If null, the app ID will be retrieved from the Oculus Platform Settings.
    public static Request<Models.PlatformInitialize> AsyncInitialize(string appId = null) {
      appId = getAppID(appId);

      Request<Models.PlatformInitialize> request;
      if (UnityEngine.Application.isEditor && PlatformSettings.UseStandalonePlatform) {
        var platform = new StandalonePlatform();
        request = platform.InitializeInEditor();
      }
      else if (UnityEngine.Application.platform == RuntimePlatform.WindowsEditor ||
               UnityEngine.Application.platform == RuntimePlatform.WindowsPlayer) {
        var platform = new WindowsPlatform();
        request = platform.AsyncInitialize(appId);
      }
      else if (UnityEngine.Application.platform == RuntimePlatform.Android) {
        var platform = new AndroidPlatform();
        request = platform.AsyncInitialize(appId);
      }
      else {
        throw new NotImplementedException("Oculus platform is not implemented on this platform yet.");
      }

      IsPlatformInitialized = (request != null);

      if (!IsPlatformInitialized)
      {
        throw new UnityException("Oculus Platform failed to initialize.");
      }

      if (LogMessages) {
        Debug.LogWarning("Oculus.Platform.Core.LogMessages is set to true. This will cause extra heap allocations, and should not be used outside of testing and debugging.");
      }

      // Create the GameObject that will run the callbacks
      (new GameObject("Oculus.Platform.CallbackRunner")).AddComponent<CallbackRunner>();
      return request;
    }

    /// (BETA) For use on platforms where the Oculus service isn't running, with additional
    /// configuration options to pass in.
    ///
    /// This method allows you to initialize the Platform SDK with custom settings,
    /// such as disabling P2P networking.
    ///
    /// Example usage:
    ///
    ///  var config = new Dictionary<InitConfigOptions, bool>{
    ///    [InitConfigOptions.DisableP2pNetworking] = true
    ///  };
    /// Platform.Core.AsyncInitialize("{access_token}", config);
    ///
    /// Note that this method is still in beta and may be subject to change.
    /// Use at your own risk.
    public static Request<Models.PlatformInitialize> AsyncInitialize(string accessToken, Dictionary<InitConfigOptions, bool> initConfigOptions, string appId = null) {
      appId = getAppID(appId);

      Request<Models.PlatformInitialize> request;
      if (UnityEngine.Application.isEditor ||
        UnityEngine.Application.platform == RuntimePlatform.WindowsEditor ||
        UnityEngine.Application.platform == RuntimePlatform.WindowsPlayer) {

        var platform = new StandalonePlatform();
        request = platform.AsyncInitializeWithAccessTokenAndOptions(appId, accessToken, initConfigOptions);
      }
      else {
        throw new NotImplementedException("Initializing with access token is not implemented on this platform yet.");
      }

      IsPlatformInitialized = (request != null);

      if (!IsPlatformInitialized)
      {
        throw new UnityException("Oculus Standalone Platform failed to initialize. Check if the access token or app id is correct.");
      }

      if (LogMessages) {
        Debug.LogWarning("Oculus.Platform.Core.LogMessages is set to true. This will cause extra heap allocations, and should not be used outside of testing and debugging.");
      }

      // Create the GameObject that will run the callbacks
      (new GameObject("Oculus.Platform.CallbackRunner")).AddComponent<CallbackRunner>();
      return request;
    }

    /// Synchronously Initialize Platform SDK. The result will be the type of Models.PlatformInitialize.Result.
    ///
    /// While the platform is in an initializing state, it's not fully functional.
    /// [Requests]: will queue up and run once platform is initialized.
    ///    For example: Users.GetLoggedInUser() can be called immediately after
    ///    synchronous init and once platform is initialized, this request will run
    /// [Synchronous Methods]: will return the default value;
    ///    For example: Users.GetLoggedInUser() will return 0 until platform is
    ///    fully initialized.
    ///
    /// Note that during initialization, some features may not be available or may
    /// return default values. It's important to check the initialization status
    /// before attempting to use certain features or functions.
    /// Use Message.IsError to check if the result is an error.
    /// Use Message.Type to check the message type.
    /// Use Models.PlatformInitialize.Result to get the result of the
    /// initialization.
    ///
    /// \param appId The app ID to use for initialization. If null, the app ID will be retrieved from the Oculus Platform Settings.
    public static void Initialize(string appId = null)
    {
      appId = getAppID(appId);

      if (UnityEngine.Application.isEditor && PlatformSettings.UseStandalonePlatform) {
        var platform = new StandalonePlatform();
        IsPlatformInitialized = platform.InitializeInEditor() != null;
      }
      else if (UnityEngine.Application.platform == RuntimePlatform.WindowsEditor ||
               UnityEngine.Application.platform == RuntimePlatform.WindowsPlayer) {
        var platform = new WindowsPlatform();
        IsPlatformInitialized = platform.Initialize(appId);
      }
      else if (UnityEngine.Application.platform == RuntimePlatform.Android) {
        var platform = new AndroidPlatform();
        IsPlatformInitialized = platform.Initialize(appId);
      }
      else {
        throw new NotImplementedException("Oculus platform is not implemented on this platform yet.");
      }

      if (!IsPlatformInitialized)
      {
        throw new UnityException("Oculus Platform failed to initialize.");
      }

      if (LogMessages) {
        Debug.LogWarning("Oculus.Platform.Core.LogMessages is set to true. This will cause extra heap allocations, and should not be used outside of testing and debugging.");
      }

      // Create the GameObject that will run the callbacks
      (new GameObject("Oculus.Platform.CallbackRunner")).AddComponent<CallbackRunner>();
    }

    private static string GetAppIDFromConfig()
    {
      if (UnityEngine.Application.platform == RuntimePlatform.Android)
      {
        return PlatformSettings.MobileAppID;
      }
      else
      {
        if (PlatformSettings.UseMobileAppIDInEditor) {
          return PlatformSettings.MobileAppID;
        }
        return PlatformSettings.AppID;
      }
    }
  }

  public static partial class ApplicationLifecycle
  {
    ///Returns information about how the application was started. This function provides details about the launch intent,
    ///such as the type of intent LaunchDetails#LaunchType and any additional data that was passed along with it.
    ///By calling this function, you can gain insight into how your application was launched and take appropriate action based on that information.
    public static Models.LaunchDetails GetLaunchDetails() {
      return new Models.LaunchDetails(CAPI.ovr_ApplicationLifecycle_GetLaunchDetails());
    }

    /// Logs if the user successfully deeplinked to a destination. This function takes two parameters: a string tracking ID and a launch result.
    /// The tracking ID is used to identify the specific deeplink attempt, while the launch result indicates whether the deeplink was LaunchResult#Success or not.
    /// By logging this information, you can track the effectiveness of your deeplinking efforts and make adjustments as needed.
    /// \param trackingID The Tracking ID is a unique identifier assigned to each deeplink attempt. It allows developers to track the success or failure of individual deeplink attempts and gain insights into the effectiveness of their deeplinking efforts.
    /// \param result An enum that indicates the outcome of an attempt to launch this application through a deeplink, including whether the attempt was LaunchResult#Success or not, and if not, the specific reasons for the failure.
    public static void LogDeeplinkResult(string trackingID, LaunchResult result) {
      CAPI.ovr_ApplicationLifecycle_LogDeeplinkResult(trackingID, result);
    }
  }

  public static partial class Leaderboards
  {
    public static Request<Models.LeaderboardEntryList> GetNextEntries(Models.LeaderboardEntryList list)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LeaderboardEntryList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, (int)Message.MessageType.Leaderboard_GetNextEntries));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    public static Request<Models.LeaderboardEntryList> GetPreviousEntries(Models.LeaderboardEntryList list)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LeaderboardEntryList>(CAPI.ovr_HTTP_GetWithMessageType(list.PreviousUrl, (int)Message.MessageType.Leaderboard_GetPreviousEntries));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }
  }

  public static partial class Challenges
  {
    /// Retrieves the next page of challenge entries. If there is no next page, this field will be empty.
    public static Request<Models.ChallengeEntryList> GetNextEntries(Models.ChallengeEntryList list)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ChallengeEntryList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, (int)Message.MessageType.Challenges_GetNextEntries));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieves the previous page of challenge entries. If there is no previous page, this field will be empty.
    public static Request<Models.ChallengeEntryList> GetPreviousEntries(Models.ChallengeEntryList list)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ChallengeEntryList>(CAPI.ovr_HTTP_GetWithMessageType(list.PreviousUrl, (int)Message.MessageType.Challenges_GetPreviousEntries));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieves the next page of challenges. If there is no next page, this field will be empty.
    public static Request<Models.ChallengeList> GetNextChallenges(Models.ChallengeList list)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ChallengeList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, (int)Message.MessageType.Challenges_GetNextChallenges));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieves the previous page of challenges. If there is no previous page, this field will be empty.
    public static Request<Models.ChallengeList> GetPreviousChallenges(Models.ChallengeList list)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ChallengeList>(CAPI.ovr_HTTP_GetWithMessageType(list.PreviousUrl, (int)Message.MessageType.Challenges_GetPreviousChallenges));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }
  }

  public static partial class Voip
  {
    /// Attempts to establish a Voip session with the specified user. A message of type Message::MessageType::Notification_Voip_StateChange will be posted when the session is established.
    /// This function can be safely called from any thread.
    public static void Start(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        CAPI.ovr_Voip_Start(userID);
      }
    }

    /// Accepts a Voip connection from a given Models.User.
    public static void Accept(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        CAPI.ovr_Voip_Accept(userID);
      }
    }

    /// Terminates a Voip session with the specified user.  Note that Voip.SetMicrophoneMuted should be used to temporarily stop sending audio; stopping and restarting a Voip session after tearing it down may be an expensive operation.
    /// This function can be safely called from any thread.
    public static void Stop(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        CAPI.ovr_Voip_Stop(userID);
      }
    }

    /// This function allows you to set a callback that will be called every time audio data is captured by the microphone. The callback function must match this signature: void filterCallback(int16_t pcmData[], size_t pcmDataLength, int frequency, int numChannels); The pcmData param is used for both input and output. pcmDataLength is the size of pcmData in elements. numChannels will be 1 or 2. If numChannels is 2, then the channel data will be interleaved in pcmData. Frequency is the input data sample rate in hertz.
    /// This function can be safely called from any thread.
    public static void SetMicrophoneFilterCallback(CAPI.FilterCallback callback)
    {
      if (Core.IsInitialized())
      {
        CAPI.ovr_Voip_SetMicrophoneFilterCallbackWithFixedSizeBuffer(callback, (UIntPtr)CAPI.VoipFilterBufferSize);
      }
    }

    /// This function is used to enable or disable the local microphone.  When muted, the microphone will not transmit any audio. Voip connections are unaffected by this state.  New connections can be established or closed whether the microphone is muted or not. This can be used to implement push-to-talk, or a local mute button.  The default state is unmuted.
    /// This function can be safely called from any thread.
    public static void SetMicrophoneMuted(VoipMuteState state)
    {
      if (Core.IsInitialized())
      {
        CAPI.ovr_Voip_SetMicrophoneMuted(state);
      }
    }

    /// Returns SystemVoip microphone's mute state. The different states are #VoipMuteState.
    public static VoipMuteState GetSystemVoipMicrophoneMuted()
    {
      if (Core.IsInitialized())
      {
        return CAPI.ovr_Voip_GetSystemVoipMicrophoneMuted();
      }
      return VoipMuteState.Unknown;
    }

    /// Returns SystemVoip status. The different statuses are #SystemVoipStatus.
    public static SystemVoipStatus GetSystemVoipStatus()
    {
      if (Core.IsInitialized())
      {
        return CAPI.ovr_Voip_GetSystemVoipStatus();
      }
      return SystemVoipStatus.Unknown;
    }

    /// Gets whether or not a voice connection is using discontinuous transmission (DTX). Both sides must set to using DTX when their connection is established in order for this to be true. Returns unknown if there is no connection.
    public static Oculus.Platform.VoipDtxState GetIsConnectionUsingDtx(UInt64 peerID)
    {
      if (Core.IsInitialized())
      {
        return CAPI.ovr_Voip_GetIsConnectionUsingDtx(peerID);
      }
      return Oculus.Platform.VoipDtxState.Unknown;
    }

    /// Gets the current local bitrate used for the connection to the specified user.  This is set by the current client. Returns unknown if there is no connection.
    public static Oculus.Platform.VoipBitrate GetLocalBitrate(UInt64 peerID)
    {
      if (Core.IsInitialized())
      {
        return CAPI.ovr_Voip_GetLocalBitrate(peerID);
      }
      return Oculus.Platform.VoipBitrate.Unknown;
    }

    /// Gets the current remote bitrate used for the connection to the specified user.  This is set by the client on the other side of the connection.  Returns unknown if there is no connection.
    public static Oculus.Platform.VoipBitrate GetRemoteBitrate(UInt64 peerID)
    {
      if (Core.IsInitialized())
      {
        return CAPI.ovr_Voip_GetRemoteBitrate(peerID);
      }
      return Oculus.Platform.VoipBitrate.Unknown;
    }

    /// The options set for newly created connections to use. Existing connections will continue to use their current settings until they are destroyed and recreated.
    public static void SetNewConnectionOptions(VoipOptions voipOptions)
    {
      if (Core.IsInitialized())
      {
        CAPI.ovr_Voip_SetNewConnectionOptions((IntPtr)voipOptions);
      }
    }
  }

  public static partial class Users
  {
    public static string GetLoggedInUserLocale()
    {
      if (Core.IsInitialized())
      {
        return CAPI.ovr_GetLoggedInUserLocale();
      }
      return "";
    }
  }

  /// The Abuse Report API provides a way for users to report abusive behavior or
  /// content within the platform. It allows developers to submit reports for
  /// various types of content, including users AbuseReportType.User, or an
  /// object/content AbuseReportType.Object.
  public static partial class AbuseReport
  {
    /// The currently running application has indicated they want to show their in-
    /// app reporting flow or that they choose to ignore the request.
    /// \param response Possible states that an app can respond to the platform notification that the in-app reporting flow has been requested by the user.
    ///
    public static Request ReportRequestHandled(ReportRequestResponse response)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_AbuseReport_ReportRequestHandled(response));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// The user has tapped the report button in the panel that appears after
    /// pressing the Oculus button.
    ///
    public static void SetReportButtonPressedNotificationCallback(Message<string>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_AbuseReport_ReportButtonPressed,
        callback
      );
    }

  }

  /// The Achievements API enables developers to create engaging experiences by
  /// awarding trophies, badges, and awards for reaching goals. Users can see
  /// friends' achievements Achievements.GetAllDefinitions(), fostering
  /// competition, and earned achievements are displayed in Meta Quest Home,
  /// showcasing progress Achievements.Unlock() and driving engagement.
  public static partial class Achievements
  {
    /// Add 'count' to the achievement with the given name. This must be a COUNT
    /// achievement. The largest number that is supported by this method is the max
    /// value of a signed 64-bit integer. If the number is larger than that, it is
    /// clamped to that max value before being passed to the servers.
    /// \param name The api_name of the achievement that will be adding count, which can be retrieved by AchievementDefinition#Name.
    /// \param count The value of count that will be added to the achievement.
    ///
    public static Request<Models.AchievementUpdate> AddCount(string name, ulong count)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementUpdate>(CAPI.ovr_Achievements_AddCount(name, count));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Unlock fields of a BITFIELD achievement.
    /// \param name The api_name of the Bitfield achievement whose field(s) will be unlocked, which can be retrieved by AchievementDefinition#Name.
    /// \param fields A string containing either '0' or '1' characters. Every '1' will unlock the field in the corresponding position.
    ///
    public static Request<Models.AchievementUpdate> AddFields(string name, string fields)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementUpdate>(CAPI.ovr_Achievements_AddFields(name, fields));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieve all achievement definitions for the app, including their name,
    /// unlock requirements, and any additional details.
    ///
    public static Request<Models.AchievementDefinitionList> GetAllDefinitions()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementDefinitionList>(CAPI.ovr_Achievements_GetAllDefinitions());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieve the progress for the user on all achievements in the app.
    ///
    public static Request<Models.AchievementProgressList> GetAllProgress()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementProgressList>(CAPI.ovr_Achievements_GetAllProgress());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieve the achievement definitions that match the specified names,
    /// including their name, unlock requirements, and any additional details.
    /// \param names The api_names of the achievements used to retrieve the definition information, which can be retrieved by AchievementDefinition#Name.
    ///
    public static Request<Models.AchievementDefinitionList> GetDefinitionsByName(string[] names)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementDefinitionList>(CAPI.ovr_Achievements_GetDefinitionsByName(names, (names != null ? names.Length : 0)));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieve the user's progress on the achievements that match the specified
    /// names.
    /// \param names The api_names of the achievements used to retrieve the progress information, which can be retrieved by AchievementDefinition#Name.
    ///
    public static Request<Models.AchievementProgressList> GetProgressByName(string[] names)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementProgressList>(CAPI.ovr_Achievements_GetProgressByName(names, (names != null ? names.Length : 0)));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Unlock the achievement with the given name. This can be of any achievement
    /// type: a simple unlock, count-based, or bitfield-based achievement. The Meta
    /// Quest Platform supports three types of achievements: simple, count and
    /// bitfield. Each achievement type has a different unlock mechanism. Simple
    /// achievements are all-or-nothing. They are unlocked by a single event or
    /// objective completion. For example, a simple achievement is unlocked when
    /// Frodo reaches Mount Doom. Count achievements are unlocked when a counter
    /// reaches a defined target. Define the AchievementDefinition#Target to reach
    /// that triggers the achievement. For example, a target achievement is
    /// unlocked when Darth Vader chokes 3 disappointing Imperial officers.
    /// Bitfield achievements are unlocked when a target number of bits in a
    /// bitfield are set. Define the AchievementDefinition#Target and
    /// AchievementDefinition#BitfieldLength that triggers the achievement. For
    /// example, a bitfield achievement is unlocked when Harry destroys 5 of the 7
    /// Horcruxes.
    /// \param name The api_name of the achievement that will be unlocked, which can be retrieved by AchievementDefinition#Name.
    ///
    public static Request<Models.AchievementUpdate> Unlock(string name)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementUpdate>(CAPI.ovr_Achievements_Unlock(name));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// The Application API provides ways to manage and interact with applications
  /// on the platform, including retrieving information about installed apps, for
  /// example: getting ApplicationVersion#CurrentCode, launching other apps and
  /// managing app downloads and updates.
  public static partial class Application
  {
    /// Cancel an app download that is in progress. It will return a result when
    /// the download is cancelled.
    ///
    public static Request<Models.AppDownloadResult> CancelAppDownload()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AppDownloadResult>(CAPI.ovr_Application_CancelAppDownload());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Track download progress for an app.
    ///
    public static Request<Models.AppDownloadProgressResult> CheckAppDownloadProgress()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AppDownloadProgressResult>(CAPI.ovr_Application_CheckAppDownloadProgress());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Requests version information, including the ApplicationVersion#CurrentCode
    /// and ApplicationVersion#CurrentName of the currently installed app and
    /// ApplicationVersion#LatestCode, ApplicationVersion#LatestName,
    /// ApplicationVersion#Size and ApplicationVersion#ReleaseDate of the latest
    /// app update.
    ///
    public static Request<Models.ApplicationVersion> GetVersion()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ApplicationVersion>(CAPI.ovr_Application_GetVersion());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Installs the app update that was previously downloaded. Once the install
    /// begins the application will exit automatically. After the installation
    /// process is complete, the app will be relaunched based on the options passed
    /// in.
    /// \param deeplink_options Additional configuration for this relaunch, which is optional. It contains 5 fields ApplicationOptions.SetDeeplinkMessage(), ApplicationOptions.SetDestinationApiName(), ApplicationOptions.SetLobbySessionId(), ApplicationOptions.SetMatchSessionId() and ApplicationOptions.SetRoomId().
    ///
    public static Request<Models.AppDownloadResult> InstallAppUpdateAndRelaunch(ApplicationOptions deeplink_options = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AppDownloadResult>(CAPI.ovr_Application_InstallAppUpdateAndRelaunch((IntPtr)deeplink_options));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Launches a different application in the user's library. If the user does
    /// not have that application installed, they will be taken to that app's page
    /// in the Oculus Store
    /// \param appID The unique ID of the app to be launched.
    /// \param deeplink_options Additional configuration for this request, which is optional. It contains 5 fields ApplicationOptions.SetDeeplinkMessage(), ApplicationOptions.SetDestinationApiName(), ApplicationOptions.SetLobbySessionId(), ApplicationOptions.SetMatchSessionId() and ApplicationOptions.SetRoomId().
    ///
    public static Request<string> LaunchOtherApp(UInt64 appID, ApplicationOptions deeplink_options = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<string>(CAPI.ovr_Application_LaunchOtherApp(appID, (IntPtr)deeplink_options));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Starts an app download. It will return a result when the download is
    /// finished. Download progress can be monitored using the
    /// Application.CheckAppDownloadProgress().
    ///
    public static Request<Models.AppDownloadResult> StartAppDownload()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AppDownloadResult>(CAPI.ovr_Application_StartAppDownload());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// This ApplicationLifecycle API provides methods for managing the lifecycle
  /// of an application, including retrieving information about how the
  /// application was started, such as the type of intent
  /// LaunchDetails#LaunchType, logging the results of deeplinking attempts,
  /// whether it was LaunchResult.Success or not, and handling changes to the
  /// launch intent.
  public static partial class ApplicationLifecycle
  {
    /// This event is triggered when a launch intent is received, whether it's a
    /// cold or warm start. The payload contains the type of intent that was
    /// received. To obtain additional details, you should call the
    /// ApplicationLifecycle.GetLaunchDetails() function.
    ///
    public static void SetLaunchIntentChangedNotificationCallback(Message<string>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_ApplicationLifecycle_LaunchIntentChanged,
        callback
      );
    }

  }

  /// The Asset File API provides methods for managing asset files in a virtual
  /// environment. The methods include getting details and status
  /// AssetDetails#DownloadStatus, downloading by ID or name, canceling
  /// downloads, and deleting files by ID or name.
  public static partial class AssetFile
  {
    /// \param assetFileID The uuid of the asset file.
    /// \deprecated Use AssetFile.DeleteById()
    ///
    public static Request<Models.AssetFileDeleteResult> Delete(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDeleteResult>(CAPI.ovr_AssetFile_Delete(assetFileID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Removes a previously installed asset file from the device by its ID.
    /// Returns an object containing the asset ID and file name, and a success
    /// flag.
    /// \param assetFileID The asset file ID
    ///
    public static Request<Models.AssetFileDeleteResult> DeleteById(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDeleteResult>(CAPI.ovr_AssetFile_DeleteById(assetFileID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Removes a previously installed asset file from the device by its name.
    /// Returns an object containing the asset ID and file name, and a success
    /// flag.
    /// \param assetFileName The asset file name
    ///
    public static Request<Models.AssetFileDeleteResult> DeleteByName(string assetFileName)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDeleteResult>(CAPI.ovr_AssetFile_DeleteByName(assetFileName));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// \param assetFileID The uuid of the file.
    /// \deprecated Use AssetFile.DownloadById()
    ///
    public static Request<Models.AssetFileDownloadResult> Download(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDownloadResult>(CAPI.ovr_AssetFile_Download(assetFileID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Downloads an asset file by its ID on demand. Returns an object containing
    /// the asset ID and filepath. Sends periodic
    /// Message::MessageType::Notification_AssetFile_DownloadUpdate to track the
    /// downloads.
    /// \param assetFileID The asset file ID
    ///
    public static Request<Models.AssetFileDownloadResult> DownloadById(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDownloadResult>(CAPI.ovr_AssetFile_DownloadById(assetFileID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Downloads an asset file by its name on demand. Returns an object containing
    /// the asset ID and filepath. Sends periodic
    /// {notifications.asset_file.download_update}} to track the downloads.
    /// \param assetFileName The asset file name
    ///
    public static Request<Models.AssetFileDownloadResult> DownloadByName(string assetFileName)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDownloadResult>(CAPI.ovr_AssetFile_DownloadByName(assetFileName));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// \param assetFileID The uuid of the asset file.
    /// \deprecated Use AssetFile.DownloadCancelById()
    ///
    public static Request<Models.AssetFileDownloadCancelResult> DownloadCancel(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDownloadCancelResult>(CAPI.ovr_AssetFile_DownloadCancel(assetFileID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Cancels a previously spawned download request for an asset file by its ID.
    /// Returns an object containing the asset ID and file path, and a success
    /// flag.
    /// \param assetFileID The asset file ID
    ///
    public static Request<Models.AssetFileDownloadCancelResult> DownloadCancelById(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDownloadCancelResult>(CAPI.ovr_AssetFile_DownloadCancelById(assetFileID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Cancels a previously spawned download request for an asset file by its
    /// name. Returns an object containing the asset ID and file path, and a
    /// success flag.
    /// \param assetFileName The asset file name
    ///
    public static Request<Models.AssetFileDownloadCancelResult> DownloadCancelByName(string assetFileName)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDownloadCancelResult>(CAPI.ovr_AssetFile_DownloadCancelByName(assetFileName));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Returns an array of asset details with asset file names and their
    /// associated IDs AssetDetails#AssetId, and whether it's currently installed
    /// AssetDetails#DownloadStatus.
    ///
    public static Request<Models.AssetDetailsList> GetList()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetDetailsList>(CAPI.ovr_AssetFile_GetList());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// \param assetFileID The uuid of the asset file.
    /// \deprecated Use AssetFile.StatusById()
    ///
    public static Request<Models.AssetDetails> Status(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetDetails>(CAPI.ovr_AssetFile_Status(assetFileID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Returns the details Models.AssetDetails on a single asset: ID, file name,
    /// and whether it's currently installed
    /// \param assetFileID The asset file ID
    ///
    public static Request<Models.AssetDetails> StatusById(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetDetails>(CAPI.ovr_AssetFile_StatusById(assetFileID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Returns the details Models.AssetDetails on a single asset: ID, file name,
    /// and whether it's currently installed
    /// \param assetFileName The asset file name
    ///
    public static Request<Models.AssetDetails> StatusByName(string assetFileName)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetDetails>(CAPI.ovr_AssetFile_StatusByName(assetFileName));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Sent to indicate download progress for asset files.
    ///
    public static void SetDownloadUpdateNotificationCallback(Message<Models.AssetFileDownloadUpdate>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_AssetFile_DownloadUpdate,
        callback
      );
    }

  }

  /// The Avatars API allows developers to create highly expressive, diverse, and
  /// customizable avatar identities for the Meta ecosystem, Unity VR apps, and
  /// other multiplayer experiences. The Avatar.LaunchAvatarEditor() method
  /// launches the Avatar Editor, where users can create and customize their
  /// avatars, the result can be retrieved by AvatarEditorResult#RequestSent.
  public static partial class Avatar
  {
    /// Launches the Avatar Editor. Meta Avatars Editor is a feature that allows
    /// users to edit their Meta Avatars appearances within the VR application that
    /// they are currently using. This experience is often used by users to switch
    /// their outfit and accessories to better suit the VR experience they are
    /// experiencing. The result can be retrieved by
    /// AvatarEditorResult#RequestSent.
    /// \param options A AvatarEditorOptions() contains the options information, including an optional override for the source of the request, which is specified by AvatarEditorOptions.SetSourceOverride().
    ///
    public static Request<Models.AvatarEditorResult> LaunchAvatarEditor(AvatarEditorOptions options = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AvatarEditorResult>(CAPI.ovr_Avatar_LaunchAvatarEditor((IntPtr)options));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// The Challenges API enhances social interactions in your app, which use
  /// GroupPresence.SetDestination() to create shareable links for score-based
  /// competition. Players can repeatedly challenge others, get to know them, and
  /// compete asynchronously. Challenges can be ranked by highest/lowest scores
  /// within a time period. Leaderboard-integrated apps get Challenges for free,
  /// accessible through the Scoreboards UI. Players can create and invite others
  /// to Challenges via the Challenges app.
  public static partial class Challenges
  {
    /// \param leaderboardName A string represents the name of the leaderboard.
    /// \param challengeOptions This indicates the options of the challenge and it can be retrieved by ChallengeOptions().
    /// \deprecated Use server-to-server API call instead.
    ///
    public static Request<Models.Challenge> Create(string leaderboardName, ChallengeOptions challengeOptions)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Challenge>(CAPI.ovr_Challenges_Create(leaderboardName, (IntPtr)challengeOptions));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// If the current user has the necessary permissions, they can decline a
    /// challenge by providing the challenge ID, which can be obtained using
    /// Challenge#ID.
    /// \param challengeID The ID of challenge that the user is going to decline. It can be retrieved by Challenge#ID.
    ///
    public static Request<Models.Challenge> DeclineInvite(UInt64 challengeID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Challenge>(CAPI.ovr_Challenges_DeclineInvite(challengeID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// \param challengeID The uuid of the challenge.
    /// \deprecated Use server-to-server API call instead.
    ///
    public static Request Delete(UInt64 challengeID)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Challenges_Delete(challengeID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Gets detailed information for a single challenge by providing the challenge
    /// ID, which can be retrieved by calling Challenge#ID.
    /// \param challengeID The id of the challenge whose entries to return, which can be retrieved by calling Challenge#ID.
    ///
    public static Request<Models.Challenge> Get(UInt64 challengeID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Challenge>(CAPI.ovr_Challenges_Get(challengeID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieves a list of entries for a specific challenge, with options to
    /// filter and limit the results. By providing the challengeID, you can specify
    /// which challenge's entries you want to retrieve. The limit parameter allows
    /// you to control the number of entries returned. The filter parameter enables
    /// you to refine the results to only include entries from users who are
    /// bidirectional followers. The startAt parameter allows you to define whether
    /// to center the query on the user or start at the top of the challenge.
    /// \param challengeID The id of the challenge whose entries to return, which can be retrieved by calling Challenge#ID.
    /// \param limit Sets a limit on the maximum number of challenges to be fetched, which can be useful for pagination or performance reasons.
    /// \param filter By using the #LeaderboardFilterType, you can refine the results to only include entries from users who are bidirectional followers.
    /// \param startAt Defines whether to center the query on the user or start at the top of the challenge. If this is LeaderboardStartAt.CenteredOnViewer or LeaderboardStartAt.CenteredOnViewerOrTop, then the current user's ID will be automatically added to the query.
    ///
    public static Request<Models.ChallengeEntryList> GetEntries(UInt64 challengeID, int limit, LeaderboardFilterType filter, LeaderboardStartAt startAt)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ChallengeEntryList>(CAPI.ovr_Challenges_GetEntries(challengeID, limit, filter, startAt));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Returns a list of entries for a specific challenge, starting from a
    /// specified rank. By providing the challengeID and rank, you can specify
    /// which challenge's entries you want to retrieve and where to start the
    /// query. The limit parameter allows you to control the number of entries
    /// returned.
    /// \param challengeID The id of the challenge whose entries to return, which can be retrieved by calling Challenge#ID.
    /// \param limit Sets a limit on the maximum number of challenges to be fetched, which can be useful for pagination or performance reasons.
    /// \param afterRank The position after which to start. For example, 10 returns challenge results starting with the 11th user.
    ///
    public static Request<Models.ChallengeEntryList> GetEntriesAfterRank(UInt64 challengeID, int limit, ulong afterRank)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ChallengeEntryList>(CAPI.ovr_Challenges_GetEntriesAfterRank(challengeID, limit, afterRank));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieves a list of challenge entries for a specific set of user IDs, with
    /// options to filter and limit the results. This method is useful for
    /// retrieving a list of challenge entries for a specific set of users,
    /// allowing you to display their progress and rankings within the challenge.
    /// \param challengeID The id of the challenge whose entries to return, which can be retrieved by calling Challenge#ID.
    /// \param limit Sets a limit on the maximum number of challenges to be fetched, which can be useful for pagination or performance reasons.
    /// \param startAt Defines whether to center the query on the user or start at the top of the challenge. If this is LeaderboardStartAt.CenteredOnViewer or LeaderboardStartAt.CenteredOnViewerOrTop, then the current user's ID will be automatically added to the query.
    /// \param userIDs Defines a list of user ids to get entries for.
    ///
    public static Request<Models.ChallengeEntryList> GetEntriesByIds(UInt64 challengeID, int limit, LeaderboardStartAt startAt, UInt64[] userIDs)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ChallengeEntryList>(CAPI.ovr_Challenges_GetEntriesByIds(challengeID, limit, startAt, userIDs, (uint)(userIDs != null ? userIDs.Length : 0)));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Returns a list of challenges that match the specified options. The
    /// ChallengeOptions() parameter allows you to specify the criteria for the
    /// challenges you want to retrieve. The limit parameter allows you to control
    /// the number of challenges returned.
    /// \param challengeOptions This indicates the options of the challenge and it can be retrieved by ChallengeOptions().
    /// \param limit Sets a limit on the maximum number of challenges to be fetched, which can be useful for pagination or performance reasons.
    ///
    public static Request<Models.ChallengeList> GetList(ChallengeOptions challengeOptions, int limit)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ChallengeList>(CAPI.ovr_Challenges_GetList((IntPtr)challengeOptions, limit));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// If the current user has the necessary permissions to join, participate in a
    /// challenge by providing the challenge ID, which can be retrieved using
    /// Challenge#ID.
    /// \param challengeID The ID of challenge that the user is going to join. It can be retrieved by Challenge#ID.
    ///
    public static Request<Models.Challenge> Join(UInt64 challengeID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Challenge>(CAPI.ovr_Challenges_Join(challengeID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// If the current user has the necessary permissions, they can leave a
    /// challenge by providing the challenge ID, which can be obtained using
    /// Challenge#ID.
    /// \param challengeID The ID of challenge that the user is going to leave. It can be retrieved by Challenge#ID.
    ///
    public static Request<Models.Challenge> Leave(UInt64 challengeID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Challenge>(CAPI.ovr_Challenges_Leave(challengeID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// \param challengeID The uuid of the challenge.
    /// \param challengeOptions This indicates the options of the challenge and it can be retrieved by ChallengeOptions().
    /// \deprecated Use server-to-server API call instead.
    ///
    public static Request<Models.Challenge> UpdateInfo(UInt64 challengeID, ChallengeOptions challengeOptions)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Challenge>(CAPI.ovr_Challenges_UpdateInfo(challengeID, (IntPtr)challengeOptions));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// The Cowatching API provides a set of methods for managing cowatching
  /// sessions in a shared virtual home environment. It allows users to request
  /// to present, resign from presenting, join or leave a session, check if they
  /// are in a session CowatchingState#InSession, set and get presenter data, set
  /// and get viewer data, and launch an invite dialog.
  public static partial class Cowatching
  {
    /// Retrieve the presenter data that drives an active cowatching session. This
    /// method can be called when there is an ongoing cowatching session, allowing
    /// developers to access and utilize the presenter data to enhance the user
    /// experience.
    ///
    public static Request<string> GetPresenterData()
    {
      if (Core.IsInitialized())
      {
        return new Request<string>(CAPI.ovr_Cowatching_GetPresenterData());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieve the viewer data of everyone who is in a cowatching session whose
    /// data was set by Cowatching.SetViewerData() viewer_data. This can be called
    /// when there is an active cowatching session.
    ///
    public static Request<Models.CowatchViewerList> GetViewersData()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.CowatchViewerList>(CAPI.ovr_Cowatching_GetViewersData());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Check whether the current user is participating in the ongoing cowatching
    /// session. It returns a boolean value CowatchingState#InSession indicating
    /// the user's presence in the session.
    ///
    public static Request<Models.CowatchingState> IsInSession()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.CowatchingState>(CAPI.ovr_Cowatching_IsInSession());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Join the ongoing cowatching session as a viewer, updating data only
    /// possible for users already in the session.
    ///
    public static Request JoinSession()
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Cowatching_JoinSession());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Launch a dialog for inviting users to cowatch in Copresent Home.
    ///
    public static Request LaunchInviteDialog()
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Cowatching_LaunchInviteDialog());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Leave the current cowatching session, rendering viewer data obsolete and no
    /// longer relevant to the ongoing experience.
    ///
    public static Request LeaveSession()
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Cowatching_LeaveSession());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Request to initiate a cowatching session as the presenter while being
    /// copresent in a shared virtual home environment.
    ///
    public static Request RequestToPresent()
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Cowatching_RequestToPresent());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Stop being the presenter and terminate the ongoing cowatching session. This
    /// action will effectively end the shared media experience.
    ///
    public static Request ResignFromPresenting()
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Cowatching_ResignFromPresenting());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Set the data that drives a cowatching session. This method is only callable
    /// by the presenter. The video title cannot exceed 100 characters, and the
    /// data size is limited to 500 characters. The data will be eventually
    /// consistent across all users.
    /// \param video_title A string representing the title of the video being played in the cowatching session. This parameter must not exceed 100 characters in length.
    /// \param presenter_data A string containing data that drives the cowatching session, such as video metadata or playback information. This parameter is limited to 500 characters in length and will be eventually consistent across all users participating in the session.
    ///
    public static Request SetPresenterData(string video_title, string presenter_data)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Cowatching_SetPresenterData(video_title, presenter_data));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Set the current user's viewer data to be shared with copresent users. This
    /// can be called when there is an active cowatching session. The data size is
    /// limited to 500 characters, and it will eventually become consistent across
    /// all users.
    /// \param viewer_data A string containing data about the current user's viewer state, such as their preferences or settings. This data is shared with copresent users during an active cowatching session and is limited to 500 characters in size. The data will eventually become consistent across all users participating in the session.
    ///
    public static Request SetViewerData(string viewer_data)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Cowatching_SetViewerData(viewer_data));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Sets a callback function that will be triggered when the user is no longer
    /// in a copresent state and cowatching actions should not be performed.
    ///
    public static void SetApiNotReadyNotificationCallback(Message<string>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Cowatching_ApiNotReady,
        callback
      );
    }

    /// Sets a callback function that will be triggered when the user is in a
    /// copresent state and cowatching is ready to begin.
    ///
    public static void SetApiReadyNotificationCallback(Message<string>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Cowatching_ApiReady,
        callback
      );
    }

    /// Sets a callback function that will be triggered when the current user
    /// joins/leaves the cowatching session.
    ///
    public static void SetInSessionChangedNotificationCallback(Message<Models.CowatchingState>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Cowatching_InSessionChanged,
        callback
      );
    }

    /// Sets a callback function that will be triggered when the cowatching API has
    /// been initialized. At this stage, the API is not yet ready for use.
    ///
    public static void SetInitializedNotificationCallback(Message<string>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Cowatching_Initialized,
        callback
      );
    }

    /// Sets a callback function that will be triggered when the presenter updates
    /// the presenter data.
    ///
    public static void SetPresenterDataChangedNotificationCallback(Message<string>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Cowatching_PresenterDataChanged,
        callback
      );
    }

    /// Sets a callback function that will be triggered when a user has started a
    /// cowatching session, and the ID of the session is reflected in the payload.
    ///
    public static void SetSessionStartedNotificationCallback(Message<string>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Cowatching_SessionStarted,
        callback
      );
    }

    /// Sets a callback function that will be triggered when a cowatching session
    /// has ended.
    ///
    public static void SetSessionStoppedNotificationCallback(Message<string>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Cowatching_SessionStopped,
        callback
      );
    }

    /// Sets a callback function that will be triggered when a user joins or
    /// updates their viewer data.
    ///
    public static void SetViewersDataChangedNotificationCallback(Message<Models.CowatchViewerUpdate>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Cowatching_ViewersDataChanged,
        callback
      );
    }

  }

  /// The Device Application Integrity API is a key function for developers
  /// committed to protecting their applications from unauthorized modifications
  /// and potential security breaches. Leveraging this API can greatly enhance
  /// the security of your applications, ensuring a seamless and safe user
  /// experience. See more details
  /// [here](https://developer.oculus.com/documentation/unity/ps-attestation-
  /// api/).
  public static partial class DeviceApplicationIntegrity
  {
    /// Returns Device and Application Integrity Attestation JSON Web Token. The
    /// token has format of header.claims.signature encoded in base64. Header
    /// contains algorithm type (PS256) and token type (JWT). See more details
    /// [here](https://developer.oculus.com/documentation/unity/ps-attestation-
    /// api/#how-does-this-work).
    /// \param challenge_nonce A string that represents a nonce value used to generate the attestation token, ensuring uniqueness and security.
    ///
    public static Request<string> GetIntegrityToken(string challenge_nonce)
    {
      if (Core.IsInitialized())
      {
        return new Request<string>(CAPI.ovr_DeviceApplicationIntegrity_GetIntegrityToken(challenge_nonce));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// The Entitlement API is a crucial component of the Meta Quest Store's app
  /// verification process. It allows developers to check whether a user has
  /// purchased or obtained their app legitimately, ensuring that only authorized
  /// users can access the app. The API must be called within 10 seconds of the
  /// user launching the app and does not require internet connectivity. In the
  /// event of a failed entitlement check, developers are responsible for
  /// handling the error in their app code, such as by displaying an error
  /// message and quitting the app. This API plays a vital role in maintaining
  /// the security and integrity of the Meta Quest Store ecosystem. See more
  /// details [here](https://developer.oculus.com/documentation/unity/ps-
  /// entitlement-check/).
  public static partial class Entitlements
  {
    /// Returns whether the current user is entitled to the current app. The
    /// primary purpose of this function is to verify user access rights to the
    /// application, ensuring that the user is authorized to use it. See example
    /// usage [here](https://developer.oculus.com/documentation/unity/ps-
    /// entitlement-check/#entitlement).
    ///
    public static Request IsUserEntitledToApplication()
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Entitlement_GetIsViewerEntitled());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// The Group Presence API currently supports immersive apps and is designed to
  /// update the platform with a user's current Destination#ApiName and status,
  /// including whether they are GroupPresenceOptions.SetIsJoinable(), their
  /// GroupPresenceOptions.SetLobbySessionId(), and
  /// GroupPresenceOptions.SetMatchSessionId(). This allows a user's location to
  /// be displayed both in VR and outside of it on social platforms, and
  /// highlights popular destinations in your app. "Joinable" indicates that a
  /// user is in an area of your app that supports other users interacting with
  /// them.
  ///
  /// Note These APIs are currently supported only for immersive mode. For non-
  /// immersive apps, such as regular Android-based panel apps or 2D experiences,
  /// this functionality is not yet supported.
  public static partial class GroupPresence
  {
    /// Clears the current group presence settings for your app. Use this when a
    /// user’s group presence setting in your app needs to be changed when moving
    /// to new destinations in your app.
    ///
    public static Request Clear()
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_GroupPresence_Clear());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Returns a list of users that can be invited to your current lobby. These
    /// are pulled from your bidirectional followers and recently met lists.
    /// \param options It contains two methods. 1. InviteOptions.AddSuggestedUser() - Takes the userID as a parameter and adds it to the inevitable users list. 2. InviteOptions.ClearSuggestedUsers() - Clears the inevitable users list.
    ///
    public static Request<Models.UserList> GetInvitableUsers(InviteOptions options)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.UserList>(CAPI.ovr_GroupPresence_GetInvitableUsers((IntPtr)options));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Get the application invites which have been sent by the user.
    ///
    public static Request<Models.ApplicationInviteList> GetSentInvites()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ApplicationInviteList>(CAPI.ovr_GroupPresence_GetSentInvites());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Launches the system invite dialog with a roster of eligible users for the
    /// current user to invite to the app. It is recommended that you surface a
    /// button in your UI that triggers this dialog when a user is joinable.
    /// \param options It contains two methods. 1. InviteOptions.AddSuggestedUser() - Takes the userID as a parameter and adds it to the inevitable users list. 2. InviteOptions.ClearSuggestedUsers() - Clears the inevitable users list.
    ///
    public static Request<Models.InvitePanelResultInfo> LaunchInvitePanel(InviteOptions options)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.InvitePanelResultInfo>(CAPI.ovr_GroupPresence_LaunchInvitePanel((IntPtr)options));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Launch an error dialog window with predefined messages for commonly
    /// occurring multiplayer errors. Check the Invokable Error Dialogs
    /// documentation for more information about these error messages and their
    /// values.
    /// \param options It contains a MultiplayerErrorOptions.SetErrorKey() associated with the predefined error message to be shown to users.
    ///
    public static Request LaunchMultiplayerErrorDialog(MultiplayerErrorOptions options)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_GroupPresence_LaunchMultiplayerErrorDialog((IntPtr)options));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Launch the dialog allowing users to rejoin a previous lobby or match.
    /// Either the user’s GroupPresenceOptions.SetLobbySessionId(), their
    /// GroupPresenceOptions.SetMatchSessionId(), or both must be populated as
    /// valid rejoinable destinations. Check the Rejoin documentation for use cases
    /// and information on this feature.
    /// \param lobby_session_id The unique identifier of the lobby session to rejoin.
    /// \param match_session_id The unique identifier of the match session to rejoin.
    /// \param destination_api_name The unique name of the in-app destination to rejoin.
    ///
    public static Request<Models.RejoinDialogResult> LaunchRejoinDialog(string lobby_session_id, string match_session_id, string destination_api_name)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.RejoinDialogResult>(CAPI.ovr_GroupPresence_LaunchRejoinDialog(lobby_session_id, match_session_id, destination_api_name));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Launch the panel displaying the current users in the roster. We do not
    /// recommend using this API because the list current users is surfaced in the
    /// Destination UI when the Meta Quest button is pressed.
    /// \param options It contains 2 methods. 1. RosterOptions.AddSuggestedUser() - it takes userID as a parameter and adds it to the inevitable users list. 2. RosterOptions.ClearSuggestedUsers() - it clears the inevitable users list.
    ///
    public static Request LaunchRosterPanel(RosterOptions options)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_GroupPresence_LaunchRosterPanel((IntPtr)options));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Sends invites to the current application to the list of userIDs passed in.
    /// You can fetch a list of users to pass in via the
    /// GroupPresence.GetInvitableUsers(). This API works as an alternative to
    /// GroupPresence.LaunchInvitePanel() which delegates the invite flow to the
    /// system invite module. GroupPresence.LaunchInvitePanel() is the recommended
    /// approach.
    /// \param userIDs userIDs is a list of users’ ids to send invites to.
    ///
    public static Request<Models.SendInvitesResult> SendInvites(UInt64[] userIDs)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.SendInvitesResult>(CAPI.ovr_GroupPresence_SendInvites(userIDs, (uint)(userIDs != null ? userIDs.Length : 0)));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Sets group presence information for your current app. It is recommended
    /// that you use this parameter and its methods to set group presence
    /// information for your app. An example of using this parameter can be found
    /// on the Group Presence overview page where the methods to set
    /// GroupPresenceOptions.SetDestinationApiName(),
    /// GroupPresenceOptions.SetMatchSessionId(), and
    /// GroupPresenceOptions.SetLobbySessionId() are used.
    /// \param groupPresenceOptions The groupPresenceOptions parameter contains five methods. 1. GroupPresenceOptions.SetDeeplinkMessageOverride() - Use GroupPresenceOptions.SetLobbySessionId() or GroupPresenceOptions.SetMatchSessionId() to specify the session. Use the GroupPresenceOptions.SetDeeplinkMessageOverride() for any additional data in whatever format you wish to aid in bringing users together. If not specified, the deeplink_message for the user will default to the one on the destination. 2.GroupPresenceOptions.SetDestinationApiName() - This the unique API Name that refers to an in-app destination. 3.GroupPresenceOptions.SetIsJoinable() - Set whether or not the person is shown as joinable or not to others. A user that is joinable can invite others to join them. Set this to false if other users would not be able to join this user. For example, the current session is full, or only the host can invite others and the current user is not the host. 4.GroupPresenceOptions.SetLobbySessionId() - This is a session that represents a closer group/squad/party of users. It is expected that all users with the same lobby session id can see or hear each other. Users with the same lobby session id in their group presence will show up in the roster and will show up as "Recently Played With" for future invites if they aren't already Oculus friends. This must be set in addition to GroupPresenceOptions.SetIsJoinable() being true for a user to use invites. 5.GroupPresenceOptions.SetMatchSessionId() - This is a session that represents all the users that are playing a specific instance of a map, game mode, round, etc. This can include users from multiple different lobbies that joined together and the users may or may not remain together after the match is over. Users with the same match session id in their group presence will not show up in the Roster, but will show up as "Recently Played with" for future invites.
    ///
    public static Request Set(GroupPresenceOptions groupPresenceOptions)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_GroupPresence_Set((IntPtr)groupPresenceOptions));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Sets the user's GroupPresenceOptions.SetDeeplinkMessageOverride() while
    /// keeping the other group presence parameters the same. If the destination of
    /// the user is not set, the deeplink message cannot be set as there's no
    /// deeplink message to override. This method does not power travel from the
    /// Meta Quest platform to your app. You must set a user’s
    /// GroupPresenceOptions.SetDestinationApiName(),
    /// GroupPresenceOptions.SetIsJoinable() status, and
    /// GroupPresenceOptions.SetLobbySessionId() to enable travel to your app.
    /// Check Group Presence overview for more information about these values.
    /// Note: Instead of using this standalone API, we recommend setting all
    /// GroupPresence parameters in one call to GroupPresence.Set().
    /// \param deeplink_message deeplink_message is the new GroupPresenceOptions.SetDeeplinkMessageOverride() to set for the user, overriding the current deeplink message.
    ///
    public static Request SetDeeplinkMessageOverride(string deeplink_message)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_GroupPresence_SetDeeplinkMessageOverride(deeplink_message));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Replaces the user's current GroupPresenceOptions.SetDestinationApiName()
    /// with the provided one. Use this to set a user's current destination while
    /// keeping all the other Group Presence parameters the same. Setting a user's
    /// destination is required to enable travel from the Meta Quest Platform to
    /// your app. NOTE instead of using the standalone API, we recommend setting
    /// all GroupPresence parameters in one call to GroupPresence.Set(). This helps
    /// ensure that all relevant presence information is singularly updated and
    /// helps reduce presence errors.
    /// \param api_name api_name is the unique name of the in-app desination to set, replacing the user's current GroupPresenceOptions.SetDestinationApiName().
    ///
    public static Request SetDestination(string api_name)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_GroupPresence_SetDestination(api_name));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Sets a user’s current presence as joinable. Use this method to update a
    /// user’s joinability as it changes. For example, when the game starts, the
    /// lobby becomes full, the user moves to a private, non joinable instance
    /// while keeping all other GroupPresence parameters (i.e
    /// GroupPresenceOptions.SetDestinationApiName(),
    /// GroupPresenceOptions.SetLobbySessionId(),
    /// GroupPresenceOptions.SetMatchSessionId()) the same. Setting a user’s
    /// destination is required to enable travel from the Meta Quest Platform to
    /// your app. Note: Instead of using this individual API, we recommend setting
    /// all GroupPresence information with the GroupPresence.Set() method and its
    /// associated parameters to simply managing all presence information. This
    /// helps ensure that all relevant presence information is singularly updated
    /// and helps reduce presence errors.
    /// \param is_joinable If GroupPresenceOptions.SetIsJoinable() is true, the user can invite others to join them. If false, other users cannot join this user, for example, if the current session is full or only the host can invite others and the current user is not the host.
    ///
    public static Request SetIsJoinable(bool is_joinable)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_GroupPresence_SetIsJoinable(is_joinable));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Replaces the user's current GroupPresenceOptions.SetLobbySessionId() for
    /// the provided string. Use this to set a user's current lobby session id
    /// while keeping all other GroupPresence parameters the same. Setting a user's
    /// lobby session id is required to enable travel from the Meta Quest Platform
    /// to your app. Check Group presence overview for more information. NOTE
    /// instead of using the standalone API, we recommend setting all GroupPresence
    /// parameters in one call to GroupPresence.Set(). This helps ensure that all
    /// relevant presence information is singularly updated and helps reduce
    /// presence errors.
    /// \param id id is the unique identifier of the lobby session to set, replacing the user's current GroupPresenceOptions.SetLobbySessionId().
    ///
    public static Request SetLobbySession(string id)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_GroupPresence_SetLobbySession(id));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Replaces the user's current GroupPresenceOptions.SetMatchSessionId() for
    /// the provided one. Use this to update the user's current match session id
    /// while keeping all other GroupPresence parameters the same.
    /// GroupPresenceOptions.SetMatchSessionId() works in conjuction with
    /// GroupPresenceOptions.SetLobbySessionId() to determine if users are playing
    /// together. If a user's match and lobby session ids are the same, they should
    /// be in the same multiplayer instance together. Users with the same lobby
    /// session id but different match session ids may be in the same lobby for
    /// things like voice chat while in different instances in your app. WARNING
    /// match session id is often treated the same as lobby session id, but this is
    /// in fact a distinct parameter and is not used for travel from the Meta Quest
    /// Platform. NOTE instead of using the standalone API, we recommend setting
    /// all GroupPresence parameters in one call to GroupPresence.Set().
    /// \param id id is the unique identifier of the match session to set, replacing the user's current GroupPresenceOptions.SetMatchSessionId().
    ///
    public static Request SetMatchSession(string id)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_GroupPresence_SetMatchSession(id));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Sent when the user is finished using the invite panel to send out
    /// invitations. Contains a list of invitees. Parameter: Callback is a function
    /// that will be called when the invitation sent status changes.
    /// Models.LaunchInvitePanelFlowResult has 1 member: UserList
    /// LaunchInvitePanelFlowResult#InvitedUsers - A list of users that were sent
    /// an invitation to the session.
    ///
    public static void SetInvitationsSentNotificationCallback(Message<Models.LaunchInvitePanelFlowResult>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_GroupPresence_InvitationsSent,
        callback
      );
    }

    /// Sent when a user has chosen to join the destination/lobby/match. Read all
    /// the fields to figure out where the user wants to go and take the
    /// appropriate actions to bring them there. If the user is unable to go there,
    /// provide adequate messaging to the user on why they cannot go there. These
    /// notifications should be responded to immediately. Parameter: Callback is a
    /// function that will be called when a user has chosen to join the
    /// destination/lobby/match. Models.GroupPresenceJoinIntent has 4 members:
    /// string GroupPresenceJoinIntent#DeeplinkMessage - An opaque string provided
    /// by the developer to help them deeplink to content. string
    /// GroupPresenceJoinIntent#DestinationApiName - The destination the current
    /// user wants to go to. string GroupPresenceJoinIntent#LobbySessionId - The
    /// lobby session the current user wants to go to. string
    /// GroupPresenceJoinIntent#MatchSessionId - The match session the current user
    /// wants to go to.
    ///
    public static void SetJoinIntentReceivedNotificationCallback(Message<Models.GroupPresenceJoinIntent>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_GroupPresence_JoinIntentReceived,
        callback
      );
    }

    /// Sent when the user has chosen to leave the destination/lobby/match from the
    /// Oculus menu. Read the specific fields to check the user is currently from
    /// the destination/lobby/match and take the appropriate actions to remove
    /// them. Update the user's presence clearing the appropriate fields to
    /// indicate the user has left. Parameter: Callback is a function that will be
    /// called when the user has chosen to leave the destination/lobby/match.
    /// Models.GroupPresenceLeaveIntent has 3 members: string
    /// GroupPresenceLeaveIntent#DestinationApiName - The destination the current
    /// user wants to leave. string GroupPresenceLeaveIntent#LobbySessionId - The
    /// lobby session the current user wants to leave. string
    /// GroupPresenceLeaveIntent#MatchSessionId - The match session the current
    /// user wants to leave.
    ///
    public static void SetLeaveIntentReceivedNotificationCallback(Message<Models.GroupPresenceLeaveIntent>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_GroupPresence_LeaveIntentReceived,
        callback
      );
    }

  }

  /// The IAP (In-App Purchases) API provides methods for managing in-app
  /// purchases, including retrieving purchase history, getting detailed product
  /// information by Purchase#Sku, and consuming purchases as needed. For more
  /// information, see
  /// [here](https://developer.oculus.com/documentation/unity/ps-iap/).
  public static partial class IAP
  {
    /// Allow the consumable IAP product to be purchased again. Conceptually, this
    /// indicates that the item was used or consumed. Important: Make sure to pass
    /// the correct SKU of the purchase that will be consumed. This value is case-
    /// sensitive and should match exactly with the product SKU set in the
    /// Developer Dashboard.
    /// \param sku The SKU of the product of the purchase that will be consumed. This value is case-sensitive and should match exactly with the product SKU set in the Developer Dashboard.
    ///
    public static Request ConsumePurchase(string sku)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_IAP_ConsumePurchase(sku));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieve a list of IAP products that can be purchased. Note: You must
    /// provide a list of SKUs (Stock Keeping Units) to retrieve the corresponding
    /// product information. The SKUs are used to identify the products in the
    /// Oculus store, which can be retrieved by accessing the Developer Dashboard
    /// or by Purchase#Sku.
    /// \param skus An array of SKUs of the products to retrieve. Each SKU should be a string value that matches exactly with the product SKU set in the Oculus Developer Dashboard.
    ///
    public static Request<Models.ProductList> GetProductsBySKU(string[] skus)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ProductList>(CAPI.ovr_IAP_GetProductsBySKU(skus, (skus != null ? skus.Length : 0)));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieve a list of Purchase that the Logged-In-User has made. This list
    /// will also contain consumable purchases that have not been consumed. Note:
    /// This method returns all purchases, including consumable and non-consumable
    /// ones. If you only want to retrieve durable purchases, use
    /// get_viewer_purchases_durable_cache instead.
    ///
    public static Request<Models.PurchaseList> GetViewerPurchases()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.PurchaseList>(CAPI.ovr_IAP_GetViewerPurchases());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieve a list of Purchase that the Logged-In-User has made. This list
    /// will only contain durable purchase (non-consumable) and is populated from a
    /// device cache. Important: It is recommended to use IAP.GetViewerPurchases()
    /// first and only check the cache if that fails. This method is intended as a
    /// fallback mechanism and may not always return up-to-date results.
    ///
    public static Request<Models.PurchaseList> GetViewerPurchasesDurableCache()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.PurchaseList>(CAPI.ovr_IAP_GetViewerPurchasesDurableCache());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Launch the checkout flow to purchase the existing product. Oculus Home
    /// tries handle and fix as many errors as possible. Home returns the
    /// appropriate error message and how to resolve it, if possible. Returns a
    /// purchase on success, and an error on user cancellation or other errors.
    ///
    /// In the case of a user cancelation, the Error#Message value will contain a
    /// JSON object with a `"category"` property containing a value of
    /// `"user_canceled"`.
    /// \param sku IAP sku for the item the user wishes to purchase.
    ///
    public static Request<Models.Purchase> LaunchCheckoutFlow(string sku)
    {
      if (Core.IsInitialized())
      {
        if (UnityEngine.Application.isEditor) {
          throw new NotImplementedException("LaunchCheckoutFlow() is not implemented in the editor yet.");
        }

        return new Request<Models.Purchase>(CAPI.ovr_IAP_LaunchCheckoutFlow(sku));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// The LanguagePack API provides a way to manage language packs for an
  /// application. A language pack is a collection of assets that are specific to
  /// a particular language, such as translations of text, audio files, and
  /// images. For more information, see
  /// [here](https://developer.oculus.com/documentation/unity/ps-language-
  /// packs/).
  public static partial class LanguagePack
  {
    /// Returns currently installed and selected language pack for an app in the
    /// view of the Models.AssetDetails. Use AssetDetails#Language field to extract
    /// needed language info. A particular language can be download and installed
    /// by a user from the Oculus app on the application page.
    ///
    public static Request<Models.AssetDetails> GetCurrent()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetDetails>(CAPI.ovr_LanguagePack_GetCurrent());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Sets the current language to specified. The parameter is the BCP47 language
    /// tag. If a language pack is not downloaded yet, spawns automatically the
    /// AssetFile.DownloadByName() request, and sends periodic
    /// Message::MessageType::Notification_AssetFile_DownloadUpdate to track the
    /// downloads. Once the language asset file is downloaded, call
    /// LanguagePack.GetCurrent() to retrieve the data, and use the language at
    /// runtime.
    /// \param tag The BCP47 language tag that identifies the language to be set as the current language.
    ///
    public static Request<Models.AssetFileDownloadResult> SetCurrent(string tag)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDownloadResult>(CAPI.ovr_LanguagePack_SetCurrent(tag));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// The Leaderboards API provides a way to manage and interact with
  /// leaderboards in your application. The API allows you to retrieve
  /// information about a single leaderboard, write entries to a leaderboard, and
  /// retrieve blocks of leaderboard entries based on different criterias.
  /// Leaderboard-integrated apps get Challenges for free, accessible through the
  /// Scoreboards UI. Visit our
  /// [website](https://developer.oculus.com/documentation/unity/ps-
  /// leaderboards/) for more information about leaderboards.
  public static partial class Leaderboards
  {
    /// Retrieves detailed information for a single leaderboard with a specified
    /// name, returning an array of Models.Leaderboard.
    /// \param leaderboardName The name of the leaderboard to retrieve.
    ///
    public static Request<Models.LeaderboardList> Get(string leaderboardName)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LeaderboardList>(CAPI.ovr_Leaderboard_Get(leaderboardName));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieves a list of leaderboard entries for a specified leaderboardName,
    /// with options to filter and limit the number of results returned.
    /// \param leaderboardName The name of the leaderboard from which to retrieve entries.
    /// \param limit Specifies the maximum number of entries to be returned.
    /// \param filter By using ovrLeaderboard_FilterFriends, this allows you to filter the returned values to bidirectional followers.
    /// \param startAt Defines whether to center the query on the user or start at the top of the leaderboard.
    ///
    /// <b>Error codes</b>
    /// - \b 100: Parameter {parameter}: invalid user id: {user_id}
    /// - \b 100: Something went wrong.
    /// - \b 12074: You're not yet ranked on this leaderboard.
    ///
    public static Request<Models.LeaderboardEntryList> GetEntries(string leaderboardName, int limit, LeaderboardFilterType filter, LeaderboardStartAt startAt)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LeaderboardEntryList>(CAPI.ovr_Leaderboard_GetEntries(leaderboardName, limit, filter, startAt));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieves a block of leaderboard entries starting from a specific rank.
    /// \param leaderboardName The name of the leaderboard from which to retrieve entries.
    /// \param limit The maximum number of entries to return.
    /// \param afterRank The position after which to start.  For example, 10 returns leaderboard results starting with the 11th user.
    ///
    public static Request<Models.LeaderboardEntryList> GetEntriesAfterRank(string leaderboardName, int limit, ulong afterRank)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LeaderboardEntryList>(CAPI.ovr_Leaderboard_GetEntriesAfterRank(leaderboardName, limit, afterRank));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieves a block of leaderboard entries that match the specified user IDs.
    /// Only entries corresponding to the provided user IDs will be returned.
    /// \param leaderboardName The name of the leaderboard from which to retrieve entries.
    /// \param limit The maximum number of entries to return.
    /// \param startAt Defines whether to center the query on the user or start at the top of the leaderboard. If this is LeaderboardStartAt.CenteredOnViewer or LeaderboardStartAt.CenteredOnViewerOrTop, then the current user's ID will be automatically added to the query.
    /// \param userIDs Defines a list of user ids to get entries for.
    ///
    public static Request<Models.LeaderboardEntryList> GetEntriesByIds(string leaderboardName, int limit, LeaderboardStartAt startAt, UInt64[] userIDs)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LeaderboardEntryList>(CAPI.ovr_Leaderboard_GetEntriesByIds(leaderboardName, limit, startAt, userIDs, (uint)(userIDs != null ? userIDs.Length : 0)));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Writes a single entry to the leaderboard, returning
    /// Models.LeaderboardUpdateStatus indicating whether the update was successful
    /// and providing the updated challenge IDs.
    /// \param leaderboardName The name of the leaderboard to which the entry should be written.
    /// \param score The score to be written in the leaderboard.
    /// \param extraData A 2KB custom data field that is associated with the leaderboard entry. This can be a game replay or any additional information that provides more context about the entry for the viewer.
    /// \param forceUpdate If true, the score always updates.  This happens even if it is not the user's best score.
    ///
    /// <b>Error codes</b>
    /// - \b 100: Parameter {parameter}: invalid user id: {user_id}
    /// - \b 100: Something went wrong.
    /// - \b 100: This leaderboard entry is too late for the leaderboard's allowed time window.
    ///
    public static Request<bool> WriteEntry(string leaderboardName, long score, byte[] extraData = null, bool forceUpdate = false)
    {
      if (Core.IsInitialized())
      {
        return new Request<bool>(CAPI.ovr_Leaderboard_WriteEntry(leaderboardName, score, extraData, (uint)(extraData != null ? extraData.Length : 0), forceUpdate));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Writes a single entry to a leaderboard which can include supplementary
    /// metrics, returning Models.LeaderboardUpdateStatus indicating whether the
    /// update was successful and providing the updated challenge IDs.
    /// \param leaderboardName The name of the leaderboard to which the entry should be written.
    /// \param score The score to be written in the leaderboard.
    /// \param supplementaryMetric Supplemental piece of data that can be used for tiebreakers.
    /// \param extraData A 2KB custom data field that is associated with the leaderboard entry. This can be a game replay or any additional information that provides more context about the entry for the viewer.
    /// \param forceUpdate If true, the score always updates. This happens even if it is not the user's best score.
    ///
    /// <b>Error codes</b>
    /// - \b 100: Parameter {parameter}: invalid user id: {user_id}
    /// - \b 100: Something went wrong.
    /// - \b 100: This leaderboard entry is too late for the leaderboard's allowed time window.
    ///
    public static Request<bool> WriteEntryWithSupplementaryMetric(string leaderboardName, long score, long supplementaryMetric, byte[] extraData = null, bool forceUpdate = false)
    {
      if (Core.IsInitialized())
      {
        return new Request<bool>(CAPI.ovr_Leaderboard_WriteEntryWithSupplementaryMetric(leaderboardName, score, supplementaryMetric, extraData, (uint)(extraData != null ? extraData.Length : 0), forceUpdate));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// The livestreaming API provides a way to receive notifications
  /// Message::MessageType::Notification_Livestreaming_StatusChange when the
  /// streaming session changes, such as when the user starts or stops streaming,
  /// or when the streaming status changes. This allows developers to respond to
  /// changes in the streaming session in real-time, providing a seamless and
  /// engaging experience for users.
  public static partial class Livestreaming
  {
    /// Indicates that the livestreaming session has been updated. You can use this
    /// information to throttle your game performance or increase CPU/GPU
    /// performance. Use Models.LivestreamingStatus to extract the updated
    /// livestreaming status.
    ///
    public static void SetStatusUpdateNotificationCallback(Message<Models.LivestreamingStatus>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Livestreaming_StatusChange,
        callback
      );
    }

  }

  /// The media API provides a convenient and seamless way to share local media
  /// files, such as photos (currently the only supported type), directly to
  /// Facebook from within your application. This allows users to easily share
  /// their favorite moments and memories with their friends and family on the
  /// world's largest social media platform. With just a few simple steps, you
  /// can enable your users to share their media files to Facebook, making it
  /// easy for them to spread the word about your app and increase its
  /// visibility. The payload returned for the sharing result is defined as
  /// Models.ShareMediaResult.
  public static partial class Media
  {
    /// Launch the Share to Facebook modal, allowing users to share local media
    /// files to Facebook. Accepts a postTextSuggestion string for the default text
    /// of the Facebook post. Requires a filePath string as the path to the image
    /// to be shared to Facebook. This image should be located in your app's
    /// internal storage directory. Requires a contentType indicating the type of
    /// media to be shared (only 'photo' is currently supported). The payload for
    /// the result is defined as Models.ShareMediaResult.
    /// \param postTextSuggestion this text will prepopulate the facebook status text-input box within the share modal
    /// \param filePath path to the file to be shared to facebook
    /// \param contentType content type of the media to be shared
    ///
    public static Request<Models.ShareMediaResult> ShareToFacebook(string postTextSuggestion, string filePath, MediaContentType contentType)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ShareMediaResult>(CAPI.ovr_Media_ShareToFacebook(postTextSuggestion, filePath, contentType));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// It represents a network synchronization system that allows multiple clients
  /// to connect and communicate with each other in real-time. It provides a way
  /// to manage and facilitate real-time communication and data synchronization
  /// between multiple clients in a networked environment and it can be retrieved
  /// using Models.NetSyncConnection.
  public static partial class NetSync
  {
    /// Sent when the status of a connection has changed. The payload will be a
    /// type of Models.NetSyncConnection.
    ///
    public static void SetConnectionStatusChangedNotificationCallback(Message<Models.NetSyncConnection>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_NetSync_ConnectionStatusChanged,
        callback
      );
    }

    /// Sent when the list of known connected sessions has changed. Contains the
    /// new list of sessions. The payload will be a type of
    /// Models.NetSyncSessionsChangedNotification.
    ///
    public static void SetSessionsChangedNotificationCallback(Message<Models.NetSyncSessionsChangedNotification>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_NetSync_SessionsChanged,
        callback
      );
    }

  }

  /// The Notifications class provides a way to manage and display notifications
  /// to the user. Notifications can be used to inform the user of important
  /// events, such as new messages, friend requests, or updates to installed
  /// apps. See more info about Platform Solutions
  /// [here](https://developer.oculus.com/documentation/unity/ps-platform-
  /// intro/).
  public static partial class Notifications
  {
    /// Marks a notification as read, causing it to disappear from various surfaces
    /// such as the Universal Menu and in-app retrieval. This action is useful for
    /// indicating that the user has acknowledged or acted upon the notification.
    /// \param notificationID The unique identifier (UUID) of the notification to be marked as read.
    ///
    public static Request MarkAsRead(UInt64 notificationID)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Notification_MarkAsRead(notificationID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// The party API allows you to retrieve information about parties and manage
  /// various interactions between users and their respective Models.Party.
  /// Calling Parties.GetCurrent() with a party ID will grab the Party object and
  /// the associated metadata.
  public static partial class Parties
  {
    /// Load the current party the current Models.User is in. The returned
    /// Models.Party will then contain information about other users in the party
    /// and invited users.
    ///
    public static Request<Models.Party> GetCurrent()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Party>(CAPI.ovr_Party_GetCurrent());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Indicates that party has been updated. This will return a
    /// Models.PartyUpdateNotification object.
    ///
    public static void SetPartyUpdateNotificationCallback(Message<Models.PartyUpdateNotification>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Party_PartyUpdate,
        callback
      );
    }

  }

  /// Push notification Models.PushNotificationResult provides a simple and
  /// efficient way for devices to register for and receive push notifications,
  /// enabling developers to build engaging and interactive applications that
  /// deliver timely updates and alerts to users.
  public static partial class PushNotification
  {
    /// Register the device to receive push notification. The registered
    /// notification id can be fetched by PushNotificationResult#Id.
    ///
    public static Request<Models.PushNotificationResult> Register()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.PushNotificationResult>(CAPI.ovr_PushNotification_Register());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// Rich Presence has been deprecated in favor of [Group
  /// Presence](https://developers.meta.com/horizon/documentation/unity/ps-group-
  /// presence-overview).
  public static partial class RichPresence
  {
    /// \deprecated Use GroupPresence.Clear()
    ///
    public static Request Clear()
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_RichPresence_Clear());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Gets all the Models.Destination that the presence can be set to
    ///
    public static Request<Models.DestinationList> GetDestinations()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.DestinationList>(CAPI.ovr_RichPresence_GetDestinations());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// \param richPresenceOptions The options of the rich presence.
    /// \deprecated Use GroupPresence.Set().
    ///
    public static Request Set(RichPresenceOptions richPresenceOptions)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_RichPresence_Set((IntPtr)richPresenceOptions));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// This class provides methods to access information about the Models.User. It
  /// allows you to retrieve a user's ID, access token, and org-scoped ID, as
  /// well as their friends list and recently met users. Additionally, it
  /// provides methods to launch various flows such as blocking, unblocking,
  /// reporting, and sending friend requests. It's useful when you need to manage
  /// user relationships or perform actions that require user authentication
  /// within your application.
  public static partial class Users
  {
    /// Retrieve the user with the given ID. This might fail if the ID is invalid
    /// or the user is blocked.
    ///
    /// NOTE: Users will have a unique ID per application.
    /// \param userID User ID retrieved with this application.
    ///
    public static Request<Models.User> Get(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.User>(CAPI.ovr_User_Get(userID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Return an access token string for this user, suitable for making REST calls
    /// against graph.oculus.com.
    ///
    public static Request<string> GetAccessToken()
    {
      if (Core.IsInitialized())
      {
        return new Request<string>(CAPI.ovr_User_GetAccessToken());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Return the IDs of users entitled to use the current app that are blocked by
    /// the specified user
    ///
    public static Request<Models.BlockedUserList> GetBlockedUsers()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.BlockedUserList>(CAPI.ovr_User_GetBlockedUsers());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Returns a list of linked accounts that are associated with the specified
    /// service providers.
    ///
    /// Customization can be done via UserOptions. Create this object
    /// withUserOptions(). The params that could be used are:
    ///
    /// 1. UserOptions.AddServiceProvider() - returns the list of linked accounts
    /// that are associated with these specified service providers.
    ///
    /// Example custom C++ usage:
    ///
    ///   auto options = ovr_UserOptions_Create();
    ///   ovr_UserOptions_AddServiceProvider(options, ovrServiceProvider_Google);
    ///   ovr_UserOptions_AddServiceProvider(options, ovrServiceProvider_Dropbox);
    ///   ovr_User_GetLinkedAccounts(options);
    ///   ovr_UserOptions_Destroy(options);
    /// \param userOptions Additional configuration for this request It is optional and the options can be created by UserOptions()
    ///
    public static Request<Models.LinkedAccountList> GetLinkedAccounts(UserOptions userOptions)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LinkedAccountList>(CAPI.ovr_User_GetLinkedAccounts((IntPtr)userOptions));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieve the currently signed in user. This call is available offline.
    ///
    /// NOTE: This will not return the user's presence as it should always be
    /// 'online' in your application.
    ///
    /// NOTE: Users will have a unique ID per application.
    ///
    /// <b>Error codes</b>
    /// - \b 100: Something went wrong.
    ///
    public static Request<Models.User> GetLoggedInUser()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.User>(CAPI.ovr_User_GetLoggedInUser());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieve a list of the logged in user's bidirectional followers. The
    /// payload type will be an array of Models.User
    ///
    public static Request<Models.UserList> GetLoggedInUserFriends()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.UserList>(CAPI.ovr_User_GetLoggedInUserFriends());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Retrieve the currently signed in user's managed info. This call is not
    /// available offline.
    ///
    /// NOTE: This will return data only if the logged in user is a managed Meta
    /// account (MMA).
    ///
    public static Request<Models.User> GetLoggedInUserManagedInfo()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.User>(CAPI.ovr_User_GetLoggedInUserManagedInfo());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// returns an ovrID which is unique per org. allows different apps within the
    /// same org to identify the user.
    /// \param userID The id of the user that we are going to get its org scoped ID Models.OrgScopedID.
    ///
    public static Request<Models.OrgScopedID> GetOrgScopedID(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.OrgScopedID>(CAPI.ovr_User_GetOrgScopedID(userID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Returns all accounts belonging to this user. Accounts are the Oculus user
    /// and x-users that are linked to this user.
    ///
    public static Request<Models.SdkAccountList> GetSdkAccounts()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.SdkAccountList>(CAPI.ovr_User_GetSdkAccounts());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Part of the scheme to confirm the identity of a particular user in your
    /// backend. You can pass the result of Users.GetUserProof() and a user ID from
    /// User#ID to your backend. Your server can then use our api to verify
    /// identity. 'https://graph.oculus.com/user_nonce_validate?nonce=USER_PROOF&us
    /// er_id=USER_ID&access_token=ACCESS_TOKEN'
    ///
    /// NOTE: The nonce is only good for one check and then it is invalidated.
    ///
    public static Request<Models.UserProof> GetUserProof()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.UserProof>(CAPI.ovr_User_GetUserProof());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Launch the flow for blocking the given user. You can't follow, be followed,
    /// invited, or searched by a blocked user, for example. You can remove the
    /// block via ovr_User_LaunchUnblockFlow.
    /// \param userID The ID of the user that the viewer is going to launch the block flow request.
    ///
    public static Request<Models.LaunchBlockFlowResult> LaunchBlockFlow(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LaunchBlockFlowResult>(CAPI.ovr_User_LaunchBlockFlow(userID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Launch the flow for sending a follow request to a user.
    /// \param userID The ID of the target user that is going to send the friend follow request to.
    ///
    public static Request<Models.LaunchFriendRequestFlowResult> LaunchFriendRequestFlow(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LaunchFriendRequestFlowResult>(CAPI.ovr_User_LaunchFriendRequestFlow(userID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Launch the flow for unblocking a user that the viewer has blocked.
    /// \param userID The ID of the user that the viewer is going to launch the unblock flow request.
    ///
    public static Request<Models.LaunchUnblockFlowResult> LaunchUnblockFlow(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LaunchUnblockFlowResult>(CAPI.ovr_User_LaunchUnblockFlow(userID));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// Users are divided into three categories based on their age:
  /// -AccountAgeCategory.Ch: Child age group for users between the ages of 10-12
  /// (or applicable age in user's region). -AccountAgeCategory.Tn: Teenage age
  /// group for users between the ages of 13-17 (or applicable age in user's
  /// region). -AccountAgeCategory.Ad: Adult age group for users ages 18 and up
  /// (or applicable age in user's region).
  public static partial class UserAgeCategory
  {
    /// Retrieve the user age category for the current user. It can be used in
    /// UserAccountAgeCategory#AgeCategory
    ///
    public static Request<Models.UserAccountAgeCategory> Get()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.UserAccountAgeCategory>(CAPI.ovr_UserAgeCategory_Get());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Report the current user's age category to Meta.
    /// \param age_category Age category for developers to send to Meta. There are two members, children age group (AppAgeCategory.Ch) and non-children age group (AppAgeCategory.Nch).
    ///
    public static Request Report(AppAgeCategory age_category)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_UserAgeCategory_Report(age_category));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  /// The voip API provides platform methods to interact with the voip
  /// connections, inputs, syncing, and systems. You can also retrieve different
  /// statuses for the current voip state. These methods exist to help integrate
  /// a platform solution for voip in your app. Read more about voip in our
  /// [docs](https://developer.oculus.com/documentation/unity/ps-parties/)
  public static partial class Voip
  {
    /// Gets whether the microphone is currently available to the app. This can be
    /// used to show if the user's voice is able to be heard by other users.
    /// Returns a microphone availability state flag which determines whether it is
    /// available or not - Models.MicrophoneAvailabilityState.
    ///
    public static Request<Models.MicrophoneAvailabilityState> GetMicrophoneAvailability()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.MicrophoneAvailabilityState>(CAPI.ovr_Voip_GetMicrophoneAvailability());
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Sets whether SystemVoip should be suppressed so that this app's Voip can
    /// use the microphone and play incoming Voip audio. Once microphone switching
    /// functionality for the user is released, this function will no longer work.
    /// You can use Voip.GetMicrophoneAvailability() to see if the user has allowed
    /// the app access to the microphone. This returns a Models.SystemVoipState
    /// object which contains statuses about whether the microphone is muted or
    /// whether passthrough is enabled.
    /// \param suppressed A boolean indicates if the voip is supressed or not.
    ///
    public static Request<Models.SystemVoipState> SetSystemVoipSuppressed(bool suppressed)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.SystemVoipState>(CAPI.ovr_Voip_SetSystemVoipSuppressed(suppressed));
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    /// Indicates that the current microphone availability state has been updated.
    /// Use Voip.GetMicrophoneAvailability() to extract the microphone availability
    /// state.
    ///
    public static void SetMicrophoneAvailabilityStateUpdateNotificationCallback(Message<string>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Voip_MicrophoneAvailabilityStateUpdate,
        callback
      );
    }

    /// Sent to indicate that some part of the overall state of SystemVoip has
    /// changed. Use SystemVoipState#Status and the properties of
    /// Models.SystemVoipState to extract the state that triggered the
    /// notification. Note that the state may have changed further since the
    /// notification was generated, and that you may call the `GetSystemVoip...()`
    /// family of functions at any time to get the current state directly.
    ///
    public static void SetSystemVoipStateNotificationCallback(Message<Models.SystemVoipState>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Voip_SystemVoipState,
        callback
      );
    }

  }

  /// The VRCamera class provides a set of methods for interacting with the VR
  /// camera system. This includes getting updates on the surface and data
  /// channel messages related to the VR camera. These methods are used to ensure
  /// that the VR camera system is functioning correctly and that any necessary
  /// updates are applied. See more info about Platform Solutions
  /// [here](https://developer.oculus.com/documentation/unity/ps-platform-
  /// intro/).
  public static partial class Vrcamera
  {
    /// Gets VR camera related WebRTC data channel messages for update. This method
    /// is used to retrieve messages that are sent over the WebRTC data channel,
    /// which can include information about the VR camera system, such as its
    /// current state or any errors that may have occurred.
    ///
    public static void SetGetDataChannelMessageUpdateNotificationCallback(Message<string>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Vrcamera_GetDataChannelMessageUpdate,
        callback
      );
    }

    /// Gets the surface and update action from the platform WebRTC for update.
    /// This method is used to retrieve information about the current state of the
    /// VR camera system, including any updates that may be required. See more info
    /// about Platform Solutions
    /// [here](https://developer.oculus.com/documentation/unity/ps-platform-
    /// intro/).
    ///
    public static void SetGetSurfaceUpdateNotificationCallback(Message<string>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Vrcamera_GetSurfaceUpdate,
        callback
      );
    }

  }


  public static partial class Achievements {
    public static Request<Models.AchievementDefinitionList> GetNextAchievementDefinitionListPage(Models.AchievementDefinitionList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextAchievementDefinitionListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementDefinitionList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.Achievements_GetNextAchievementDefinitionArrayPage
          )
        );
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    public static Request<Models.AchievementProgressList> GetNextAchievementProgressListPage(Models.AchievementProgressList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextAchievementProgressListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementProgressList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.Achievements_GetNextAchievementProgressArrayPage
          )
        );
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  public static partial class Cowatching {
    public static Request<Models.CowatchViewerList> GetNextCowatchViewerListPage(Models.CowatchViewerList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextCowatchViewerListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.CowatchViewerList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.Cowatching_GetNextCowatchViewerArrayPage
          )
        );
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  public static partial class GroupPresence {
    public static Request<Models.ApplicationInviteList> GetNextApplicationInviteListPage(Models.ApplicationInviteList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextApplicationInviteListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.ApplicationInviteList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.GroupPresence_GetNextApplicationInviteArrayPage
          )
        );
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  public static partial class IAP {
    public static Request<Models.ProductList> GetNextProductListPage(Models.ProductList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextProductListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.ProductList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.IAP_GetNextProductArrayPage
          )
        );
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    public static Request<Models.PurchaseList> GetNextPurchaseListPage(Models.PurchaseList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextPurchaseListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.PurchaseList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.IAP_GetNextPurchaseArrayPage
          )
        );
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  public static partial class Leaderboards {
    public static Request<Models.LeaderboardList> GetNextLeaderboardListPage(Models.LeaderboardList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextLeaderboardListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.LeaderboardList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.Leaderboard_GetNextLeaderboardArrayPage
          )
        );
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  public static partial class Notifications {
  }

  public static partial class RichPresence {
    public static Request<Models.DestinationList> GetNextDestinationListPage(Models.DestinationList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextDestinationListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.DestinationList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.RichPresence_GetNextDestinationArrayPage
          )
        );
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }

  public static partial class Users {
    public static Request<Models.BlockedUserList> GetNextBlockedUserListPage(Models.BlockedUserList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextBlockedUserListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.BlockedUserList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.User_GetNextBlockedUserArrayPage
          )
        );
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    public static Request<Models.UserList> GetNextUserListPage(Models.UserList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextUserListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.UserList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.User_GetNextUserArrayPage
          )
        );
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

    public static Request<Models.UserCapabilityList> GetNextUserCapabilityListPage(Models.UserCapabilityList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextUserCapabilityListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.UserCapabilityList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.User_GetNextUserCapabilityArrayPage
          )
        );
      }

      Debug.LogError(Oculus.Platform.Core.PlatformUninitializedError);
      return null;
    }

  }


}
