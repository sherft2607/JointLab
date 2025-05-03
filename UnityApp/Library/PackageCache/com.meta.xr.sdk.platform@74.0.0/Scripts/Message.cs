// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using UnityEngine;
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using Oculus.Platform.Models;

  public abstract class Message<T> : Message
  {
    /// The Callback delegate represents a method that will be called when a message of type T is received.
    /// The method takes a single parameter, message, which is an instance of the Message<T> class containing the message data.
    public new delegate void Callback(Message<T> message);
    /// The Message(IntPtr c_message) constructor takes an IntPtr parameter c_message which represents the native message object.
    /// It calls the base constructor Message() to initialize the message object. If the message is not an error message,
    /// it retrieves the data from the message using the GetDataFromMessage() method and stores it in the data field.
    public Message(IntPtr c_message) : base(c_message) {
      if (!IsError)
      {
        data = GetDataFromMessage(c_message);
      }
    }

    /// The data property provides access to the message's contents, allowing you to retrieve and manipulate the data within.
    public T Data { get { return data; } }
    protected abstract T GetDataFromMessage(IntPtr c_message);
    private T data;
  }

  /// The Message class represents a message object that contains information about a specific event or action.
  /// It provides a way to access and manipulate the message's contents, including its type, error status, request ID, and message.
  /// See details [here](https://developer.oculus.com/documentation/native/ps-requests-and-messages/).
  public class Message
  {
    /// The Callback delegate represents a method that will be called when a Message.MessageType is received.
    /// The method takes a single parameter, message, which is an instance of the Message.MessageType class containing the message data.
    public delegate void Callback(Message message);
    /// The Message(IntPtr c_message) constructor takes an IntPtr parameter c_message which represents the native message object.
    /// It initializes the Message object with the contents of the native message object.
    public Message(IntPtr c_message)
    {
      type = (MessageType)CAPI.ovr_Message_GetType(c_message);
      var isError = CAPI.ovr_Message_IsError(c_message);
      requestID = CAPI.ovr_Message_GetRequestID(c_message);

      if (!isError) {
        var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
        if (CAPI.ovr_Message_IsError(msg)) {
          IntPtr errorHandle = CAPI.ovr_Message_GetError(msg);
          error = new Error(
            CAPI.ovr_Error_GetCode(errorHandle),
            CAPI.ovr_Error_GetMessage(errorHandle),
            CAPI.ovr_Error_GetHttpCode(errorHandle));
        }
      }

      if (isError)
      {
        IntPtr errorHandle = CAPI.ovr_Message_GetError(c_message);
        error = new Error(
          CAPI.ovr_Error_GetCode(errorHandle),
          CAPI.ovr_Error_GetMessage(errorHandle),
          CAPI.ovr_Error_GetHttpCode(errorHandle));
      }
      else if (Core.LogMessages)
      {
        var message = CAPI.ovr_Message_GetString(c_message);
        if (message != null)
        {
          Debug.Log(message);
        }
        else
        {
          Debug.Log(string.Format("null message string {0}", c_message));
        }
      }
    }

    ~Message()
    {
    }

    // Keep this enum in sync with ovrMessageType in OVR_Platform.h
    public enum MessageType : uint
    { //TODO - rename this to type; it's already in Message class
      Unknown,

      AbuseReport_ReportRequestHandled                   = 0x4B8EFC86,
      Achievements_AddCount                              = 0x03E76231,
      Achievements_AddFields                             = 0x14AA2129,
      Achievements_GetAllDefinitions                     = 0x03D3458D,
      Achievements_GetAllProgress                        = 0x4F9FDE1D,
      Achievements_GetDefinitionsByName                  = 0x629101BC,
      Achievements_GetNextAchievementDefinitionArrayPage = 0x2A7DD255,
      Achievements_GetNextAchievementProgressArrayPage   = 0x2F42E727,
      Achievements_GetProgressByName                     = 0x152663B1,
      Achievements_Unlock                                = 0x593CCBDD,
      ApplicationLifecycle_GetRegisteredPIDs             = 0x04E5CF62,
      ApplicationLifecycle_GetSessionKey                 = 0x3AAF591D,
      ApplicationLifecycle_RegisterSessionKey            = 0x4DB6AFF8,
      Application_CancelAppDownload                      = 0x7C2060DE,
      Application_CheckAppDownloadProgress               = 0x5534A924,
      Application_GetVersion                             = 0x68670A0E,
      Application_InstallAppUpdateAndRelaunch            = 0x14806B85,
      Application_LaunchOtherApp                         = 0x54E2D1F8,
      Application_StartAppDownload                       = 0x44FC006E,
      AssetFile_Delete                                   = 0x6D5D7886,
      AssetFile_DeleteById                               = 0x5AE8CD52,
      AssetFile_DeleteByName                             = 0x420AC1CF,
      AssetFile_Download                                 = 0x11449FC5,
      AssetFile_DownloadById                             = 0x2D008992,
      AssetFile_DownloadByName                           = 0x6336CEFA,
      AssetFile_DownloadCancel                           = 0x080AD3C7,
      AssetFile_DownloadCancelById                       = 0x51659514,
      AssetFile_DownloadCancelByName                     = 0x446AECFA,
      AssetFile_GetList                                  = 0x4AFC6F74,
      AssetFile_Status                                   = 0x02D32F60,
      AssetFile_StatusById                               = 0x5D955D38,
      AssetFile_StatusByName                             = 0x41CFDA50,
      Avatar_LaunchAvatarEditor                          = 0x05F1E153,
      Challenges_Create                                  = 0x6859D641,
      Challenges_DeclineInvite                           = 0x568E76C0,
      Challenges_Delete                                  = 0x264885CA,
      Challenges_Get                                     = 0x77584EF3,
      Challenges_GetEntries                              = 0x121AB45F,
      Challenges_GetEntriesAfterRank                     = 0x08891A7F,
      Challenges_GetEntriesByIds                         = 0x316509DC,
      Challenges_GetList                                 = 0x43264356,
      Challenges_GetNextChallenges                       = 0x5B7CA1B6,
      Challenges_GetNextEntries                          = 0x7F4CA0C6,
      Challenges_GetPreviousChallenges                   = 0x0EB4040D,
      Challenges_GetPreviousEntries                      = 0x78C90470,
      Challenges_Join                                    = 0x21248069,
      Challenges_Leave                                   = 0x296116E5,
      Challenges_UpdateInfo                              = 0x1175BE60,
      Cowatching_GetNextCowatchViewerArrayPage           = 0x1D403932,
      Cowatching_GetPresenterData                        = 0x49864735,
      Cowatching_GetViewersData                          = 0x5CD7A24F,
      Cowatching_IsInSession                             = 0x651B4884,
      Cowatching_JoinSession                             = 0x6388A554,
      Cowatching_LaunchInviteDialog                      = 0x22933297,
      Cowatching_LeaveSession                            = 0x3C9E46CD,
      Cowatching_RequestToPresent                        = 0x7F79BCAA,
      Cowatching_ResignFromPresenting                    = 0x4B49C202,
      Cowatching_SetPresenterData                        = 0x6D1C8906,
      Cowatching_SetViewerData                           = 0x3CDBE826,
      DeviceApplicationIntegrity_GetIntegrityToken       = 0x3271ABDA,
      Entitlement_GetIsViewerEntitled                    = 0x186B58B1,
      GroupPresence_Clear                                = 0x6DAA9CC3,
      GroupPresence_GetInvitableUsers                    = 0x234BC3F1,
      GroupPresence_GetNextApplicationInviteArrayPage    = 0x04F8C0F2,
      GroupPresence_GetSentInvites                       = 0x08260AB1,
      GroupPresence_LaunchInvitePanel                    = 0x0F9ECF9F,
      GroupPresence_LaunchMultiplayerErrorDialog         = 0x2955AF24,
      GroupPresence_LaunchRejoinDialog                   = 0x1577036F,
      GroupPresence_LaunchRosterPanel                    = 0x35728882,
      GroupPresence_SendInvites                          = 0x0DCBD364,
      GroupPresence_Set                                  = 0x675F5C24,
      GroupPresence_SetDeeplinkMessageOverride           = 0x521ADF0D,
      GroupPresence_SetDestination                       = 0x4C5B268A,
      GroupPresence_SetIsJoinable                        = 0x2A8F1055,
      GroupPresence_SetLobbySession                      = 0x48FF55BE,
      GroupPresence_SetMatchSession                      = 0x314C84B8,
      IAP_ConsumePurchase                                = 0x1FBB72D9,
      IAP_GetNextProductArrayPage                        = 0x1BD94AAF,
      IAP_GetNextPurchaseArrayPage                       = 0x47570A95,
      IAP_GetProductsBySKU                               = 0x7E9ACAF5,
      IAP_GetViewerPurchases                             = 0x3A0F8419,
      IAP_GetViewerPurchasesDurableCache                 = 0x63599E2B,
      IAP_LaunchCheckoutFlow                             = 0x3F9B0D0D,
      LanguagePack_GetCurrent                            = 0x1F90F0D5,
      LanguagePack_SetCurrent                            = 0x5B4FBBE0,
      Leaderboard_Get                                    = 0x6AD44EF8,
      Leaderboard_GetEntries                             = 0x5DB3474C,
      Leaderboard_GetEntriesAfterRank                    = 0x18378BEF,
      Leaderboard_GetEntriesByIds                        = 0x39607BFC,
      Leaderboard_GetNextEntries                         = 0x4E207CD9,
      Leaderboard_GetNextLeaderboardArrayPage            = 0x35F6769B,
      Leaderboard_GetPreviousEntries                     = 0x4901DAC0,
      Leaderboard_WriteEntry                             = 0x117FC8FE,
      Leaderboard_WriteEntryWithSupplementaryMetric      = 0x72C692FA,
      Media_ShareToFacebook                              = 0x00E38AEF,
      Notification_MarkAsRead                            = 0x717259E3,
      Party_GetCurrent                                   = 0x47933760,
      PushNotification_Register                          = 0x663A8B5F,
      RichPresence_Clear                                 = 0x57B752B3,
      RichPresence_GetDestinations                       = 0x586F2D14,
      RichPresence_GetNextDestinationArrayPage           = 0x67367F45,
      RichPresence_Set                                   = 0x3C147509,
      UserAgeCategory_Get                                = 0x21CBE0C0,
      UserAgeCategory_Report                             = 0x2E4DD8D6,
      User_Get                                           = 0x6BCF9E47,
      User_GetAccessToken                                = 0x06A85ABE,
      User_GetBlockedUsers                               = 0x7D201556,
      User_GetLinkedAccounts                             = 0x5793F456,
      User_GetLoggedInUser                               = 0x436F345D,
      User_GetLoggedInUserFriends                        = 0x587C2A8D,
      User_GetLoggedInUserManagedInfo                    = 0x70BA3AEE,
      User_GetNextBlockedUserArrayPage                   = 0x7C2AFDCB,
      User_GetNextUserArrayPage                          = 0x267CF743,
      User_GetNextUserCapabilityArrayPage                = 0x2309F399,
      User_GetOrgScopedID                                = 0x18F0B01B,
      User_GetSdkAccounts                                = 0x67526A83,
      User_GetUserProof                                  = 0x22810483,
      User_LaunchBlockFlow                               = 0x6FD62528,
      User_LaunchFriendRequestFlow                       = 0x0904B598,
      User_LaunchUnblockFlow                             = 0x14A22A97,
      Voip_GetMicrophoneAvailability                     = 0x744CE345,
      Voip_SetSystemVoipSuppressed                       = 0x453FC9AA,

      /// The user has tapped the report button in the panel that appears after
      /// pressing the Oculus button.
      Notification_AbuseReport_ReportButtonPressed = 0x24472F6C,

      /// This event is triggered when a launch intent is received, whether it's a
      /// cold or warm start. The payload contains the type of intent that was
      /// received. To obtain additional details, you should call the
      /// ApplicationLifecycle.GetLaunchDetails() function.
      Notification_ApplicationLifecycle_LaunchIntentChanged = 0x04B34CA3,

      /// Sent to indicate download progress for asset files.
      Notification_AssetFile_DownloadUpdate = 0x2FDD0CCD,

      /// Sets a callback function that will be triggered when the user is no longer
      /// in a copresent state and cowatching actions should not be performed.
      Notification_Cowatching_ApiNotReady = 0x66093981,

      /// Sets a callback function that will be triggered when the user is in a
      /// copresent state and cowatching is ready to begin.
      Notification_Cowatching_ApiReady = 0x09956693,

      /// Sets a callback function that will be triggered when the current user
      /// joins/leaves the cowatching session.
      Notification_Cowatching_InSessionChanged = 0x0DF93113,

      /// Sets a callback function that will be triggered when the cowatching API has
      /// been initialized. At this stage, the API is not yet ready for use.
      Notification_Cowatching_Initialized = 0x74D948F3,

      /// Sets a callback function that will be triggered when the presenter updates
      /// the presenter data.
      Notification_Cowatching_PresenterDataChanged = 0x4E078EEE,

      /// Sets a callback function that will be triggered when a user has started a
      /// cowatching session, and the ID of the session is reflected in the payload.
      Notification_Cowatching_SessionStarted = 0x7321939C,

      /// Sets a callback function that will be triggered when a cowatching session
      /// has ended.
      Notification_Cowatching_SessionStopped = 0x49E6DBFA,

      /// Sets a callback function that will be triggered when a user joins or
      /// updates their viewer data.
      Notification_Cowatching_ViewersDataChanged = 0x68F2F1FF,

      /// Sent when the user is finished using the invite panel to send out
      /// invitations. Contains a list of invitees. Parameter: Callback is a function
      /// that will be called when the invitation sent status changes.
      /// Models.LaunchInvitePanelFlowResult has 1 member: UserList
      /// LaunchInvitePanelFlowResult#InvitedUsers - A list of users that were sent
      /// an invitation to the session.
      Notification_GroupPresence_InvitationsSent = 0x679A84B6,

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
      Notification_GroupPresence_JoinIntentReceived = 0x773889F6,

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
      Notification_GroupPresence_LeaveIntentReceived = 0x4737EA1D,

      /// Sent to indicate that more data has been read or an error occured.
      Notification_HTTP_Transfer = 0x7DD46E2F,

      /// Indicates that the livestreaming session has been updated. You can use this
      /// information to throttle your game performance or increase CPU/GPU
      /// performance. Use Models.LivestreamingStatus to extract the updated
      /// livestreaming status.
      Notification_Livestreaming_StatusChange = 0x2247596E,

      /// Sent when the status of a connection has changed. The payload will be a
      /// type of Models.NetSyncConnection.
      Notification_NetSync_ConnectionStatusChanged = 0x073484CA,

      /// Sent when the list of known connected sessions has changed. Contains the
      /// new list of sessions. The payload will be a type of
      /// Models.NetSyncSessionsChangedNotification.
      Notification_NetSync_SessionsChanged = 0x387E7F36,

      /// Indicates that party has been updated. This will return a
      /// Models.PartyUpdateNotification object.
      Notification_Party_PartyUpdate = 0x1D118AB2,

      /// Indicates that the current microphone availability state has been updated.
      /// Use Voip.GetMicrophoneAvailability() to extract the microphone availability
      /// state.
      Notification_Voip_MicrophoneAvailabilityStateUpdate = 0x3E20CB57,

      /// Sent to indicate that some part of the overall state of SystemVoip has
      /// changed. Use SystemVoipState#Status and the properties of
      /// Models.SystemVoipState to extract the state that triggered the
      /// notification. Note that the state may have changed further since the
      /// notification was generated, and that you may call the `GetSystemVoip...()`
      /// family of functions at any time to get the current state directly.
      Notification_Voip_SystemVoipState = 0x58D254A5,

      /// Gets VR camera related WebRTC data channel messages for update. This method
      /// is used to retrieve messages that are sent over the WebRTC data channel,
      /// which can include information about the VR camera system, such as its
      /// current state or any errors that may have occurred.
      Notification_Vrcamera_GetDataChannelMessageUpdate = 0x6EE4F33C,

      /// Gets the surface and update action from the platform WebRTC for update.
      /// This method is used to retrieve information about the current state of the
      /// VR camera system, including any updates that may be required. See more info
      /// about Platform Solutions
      /// [here](https://developer.oculus.com/documentation/unity/ps-platform-
      /// intro/).
      Notification_Vrcamera_GetSurfaceUpdate = 0x37F21084,


      Platform_InitializeWithAccessToken = 0x35692F2B,
      Platform_InitializeStandaloneOculus = 0x51F8CE0C,
      Platform_InitializeAndroidAsynchronous = 0x1AD307B4,
      Platform_InitializeWindowsAsynchronous = 0x6DA7BA8F,
    };

    /// Returns the message type, which is defined as Message.MessageType, indicating the purpose or category of the message.
    public MessageType Type { get { return type; } }
    /// Indicates whether the message represents an error or not, based on the presence of an error object.
    public bool IsError { get { return error != null; } }
    /// Returns the unique identifier of the request that generated this message, if applicable.
    public ulong RequestID { get { return requestID; } }

    private MessageType type;
    private ulong requestID;
    private Error error;

    /// Returns the error object associated with this message, or null if there is no error.
    public virtual Error GetError() { return error; }
    /// Returns an object containing information about the HTTP transfer update, or null if there is no update.
    public virtual HttpTransferUpdate GetHttpTransferUpdate() { return null; }
    /// Returns an object containing information about the platform initialization, or null if there is no initialization.
    public virtual PlatformInitialize GetPlatformInitialize() { return null; }

    /// Returns the value of the AbuseReportRecording property, or false/null depending on the type of the property.
    public virtual AbuseReportRecording GetAbuseReportRecording() { return null; }
    /// Returns the value of the AchievementDefinitions property, or false/null depending on the type of the property.
    public virtual AchievementDefinitionList GetAchievementDefinitions() { return null; }
    /// Returns the value of the AchievementProgressList property, or false/null depending on the type of the property.
    public virtual AchievementProgressList GetAchievementProgressList() { return null; }
    /// Returns the value of the AchievementUpdate property, or false/null depending on the type of the property.
    public virtual AchievementUpdate GetAchievementUpdate() { return null; }
    /// Returns the value of the AppDownloadProgressResult property, or false/null depending on the type of the property.
    public virtual AppDownloadProgressResult GetAppDownloadProgressResult() { return null; }
    /// Returns the value of the AppDownloadResult property, or false/null depending on the type of the property.
    public virtual AppDownloadResult GetAppDownloadResult() { return null; }
    /// Returns the value of the ApplicationInviteList property, or false/null depending on the type of the property.
    public virtual ApplicationInviteList GetApplicationInviteList() { return null; }
    /// Returns the value of the ApplicationVersion property, or false/null depending on the type of the property.
    public virtual ApplicationVersion GetApplicationVersion() { return null; }
    /// Returns the value of the AssetDetails property, or false/null depending on the type of the property.
    public virtual AssetDetails GetAssetDetails() { return null; }
    /// Returns the value of the AssetDetailsList property, or false/null depending on the type of the property.
    public virtual AssetDetailsList GetAssetDetailsList() { return null; }
    /// Returns the value of the AssetFileDeleteResult property, or false/null depending on the type of the property.
    public virtual AssetFileDeleteResult GetAssetFileDeleteResult() { return null; }
    /// Returns the value of the AssetFileDownloadCancelResult property, or false/null depending on the type of the property.
    public virtual AssetFileDownloadCancelResult GetAssetFileDownloadCancelResult() { return null; }
    /// Returns the value of the AssetFileDownloadResult property, or false/null depending on the type of the property.
    public virtual AssetFileDownloadResult GetAssetFileDownloadResult() { return null; }
    /// Returns the value of the AssetFileDownloadUpdate property, or false/null depending on the type of the property.
    public virtual AssetFileDownloadUpdate GetAssetFileDownloadUpdate() { return null; }
    /// Returns the value of the AvatarEditorResult property, or false/null depending on the type of the property.
    public virtual AvatarEditorResult GetAvatarEditorResult() { return null; }
    /// Returns the value of the BlockedUserList property, or false/null depending on the type of the property.
    public virtual BlockedUserList GetBlockedUserList() { return null; }
    /// Returns the value of the Challenge property, or false/null depending on the type of the property.
    public virtual Challenge GetChallenge() { return null; }
    /// Returns the value of the ChallengeEntryList property, or false/null depending on the type of the property.
    public virtual ChallengeEntryList GetChallengeEntryList() { return null; }
    /// Returns the value of the ChallengeList property, or false/null depending on the type of the property.
    public virtual ChallengeList GetChallengeList() { return null; }
    /// Returns the value of the CowatchingState property, or false/null depending on the type of the property.
    public virtual CowatchingState GetCowatchingState() { return null; }
    /// Returns the value of the CowatchViewerList property, or false/null depending on the type of the property.
    public virtual CowatchViewerList GetCowatchViewerList() { return null; }
    /// Returns the value of the CowatchViewerUpdate property, or false/null depending on the type of the property.
    public virtual CowatchViewerUpdate GetCowatchViewerUpdate() { return null; }
    /// Returns the value of the DestinationList property, or false/null depending on the type of the property.
    public virtual DestinationList GetDestinationList() { return null; }
    /// Returns the value of the GroupPresenceJoinIntent property, or false/null depending on the type of the property.
    public virtual GroupPresenceJoinIntent GetGroupPresenceJoinIntent() { return null; }
    /// Returns the value of the GroupPresenceLeaveIntent property, or false/null depending on the type of the property.
    public virtual GroupPresenceLeaveIntent GetGroupPresenceLeaveIntent() { return null; }
    /// Returns the value of the InstalledApplicationList property, or false/null depending on the type of the property.
    public virtual InstalledApplicationList GetInstalledApplicationList() { return null; }
    /// Returns the value of the InvitePanelResultInfo property, or false/null depending on the type of the property.
    public virtual InvitePanelResultInfo GetInvitePanelResultInfo() { return null; }
    /// Returns the value of the LaunchBlockFlowResult property, or false/null depending on the type of the property.
    public virtual LaunchBlockFlowResult GetLaunchBlockFlowResult() { return null; }
    /// Returns the value of the LaunchFriendRequestFlowResult property, or false/null depending on the type of the property.
    public virtual LaunchFriendRequestFlowResult GetLaunchFriendRequestFlowResult() { return null; }
    /// Returns the value of the LaunchInvitePanelFlowResult property, or false/null depending on the type of the property.
    public virtual LaunchInvitePanelFlowResult GetLaunchInvitePanelFlowResult() { return null; }
    /// Returns the value of the LaunchReportFlowResult property, or false/null depending on the type of the property.
    public virtual LaunchReportFlowResult GetLaunchReportFlowResult() { return null; }
    /// Returns the value of the LaunchUnblockFlowResult property, or false/null depending on the type of the property.
    public virtual LaunchUnblockFlowResult GetLaunchUnblockFlowResult() { return null; }
    /// Returns the value of the LeaderboardDidUpdate property, or false/null depending on the type of the property.
    public virtual bool GetLeaderboardDidUpdate() { return false; }
    /// Returns the value of the LeaderboardEntryList property, or false/null depending on the type of the property.
    public virtual LeaderboardEntryList GetLeaderboardEntryList() { return null; }
    /// Returns the value of the LeaderboardList property, or false/null depending on the type of the property.
    public virtual LeaderboardList GetLeaderboardList() { return null; }
    /// Returns the value of the LinkedAccountList property, or false/null depending on the type of the property.
    public virtual LinkedAccountList GetLinkedAccountList() { return null; }
    /// Returns the value of the LivestreamingApplicationStatus property, or false/null depending on the type of the property.
    public virtual LivestreamingApplicationStatus GetLivestreamingApplicationStatus() { return null; }
    /// Returns the value of the LivestreamingStartResult property, or false/null depending on the type of the property.
    public virtual LivestreamingStartResult GetLivestreamingStartResult() { return null; }
    /// Returns the value of the LivestreamingStatus property, or false/null depending on the type of the property.
    public virtual LivestreamingStatus GetLivestreamingStatus() { return null; }
    /// Returns the value of the LivestreamingVideoStats property, or false/null depending on the type of the property.
    public virtual LivestreamingVideoStats GetLivestreamingVideoStats() { return null; }
    /// Returns the value of the MicrophoneAvailabilityState property, or false/null depending on the type of the property.
    public virtual MicrophoneAvailabilityState GetMicrophoneAvailabilityState() { return null; }
    /// Returns the value of the NetSyncConnection property, or false/null depending on the type of the property.
    public virtual NetSyncConnection GetNetSyncConnection() { return null; }
    /// Returns the value of the NetSyncSessionList property, or false/null depending on the type of the property.
    public virtual NetSyncSessionList GetNetSyncSessionList() { return null; }
    /// Returns the value of the NetSyncSessionsChangedNotification property, or false/null depending on the type of the property.
    public virtual NetSyncSessionsChangedNotification GetNetSyncSessionsChangedNotification() { return null; }
    /// Returns the value of the NetSyncSetSessionPropertyResult property, or false/null depending on the type of the property.
    public virtual NetSyncSetSessionPropertyResult GetNetSyncSetSessionPropertyResult() { return null; }
    /// Returns the value of the NetSyncVoipAttenuationValueList property, or false/null depending on the type of the property.
    public virtual NetSyncVoipAttenuationValueList GetNetSyncVoipAttenuationValueList() { return null; }
    /// Returns the value of the OrgScopedID property, or false/null depending on the type of the property.
    public virtual OrgScopedID GetOrgScopedID() { return null; }
    /// Returns the value of the Party property, or false/null depending on the type of the property.
    public virtual Party GetParty() { return null; }
    /// Returns the value of the PartyID property, or false/null depending on the type of the property.
    public virtual PartyID GetPartyID() { return null; }
    /// Returns the value of the PartyUpdateNotification property, or false/null depending on the type of the property.
    public virtual PartyUpdateNotification GetPartyUpdateNotification() { return null; }
    /// Returns the value of the PidList property, or false/null depending on the type of the property.
    public virtual PidList GetPidList() { return null; }
    /// Returns the value of the ProductList property, or false/null depending on the type of the property.
    public virtual ProductList GetProductList() { return null; }
    /// Returns the value of the Purchase property, or false/null depending on the type of the property.
    public virtual Purchase GetPurchase() { return null; }
    /// Returns the value of the PurchaseList property, or false/null depending on the type of the property.
    public virtual PurchaseList GetPurchaseList() { return null; }
    /// Returns the value of the PushNotificationResult property, or false/null depending on the type of the property.
    public virtual PushNotificationResult GetPushNotificationResult() { return null; }
    /// Returns the value of the RejoinDialogResult property, or false/null depending on the type of the property.
    public virtual RejoinDialogResult GetRejoinDialogResult() { return null; }
    /// Returns the value of the SdkAccountList property, or false/null depending on the type of the property.
    public virtual SdkAccountList GetSdkAccountList() { return null; }
    /// Returns the value of the SendInvitesResult property, or false/null depending on the type of the property.
    public virtual SendInvitesResult GetSendInvitesResult() { return null; }
    /// Returns the value of the ShareMediaResult property, or false/null depending on the type of the property.
    public virtual ShareMediaResult GetShareMediaResult() { return null; }
    /// Returns the value of the String property, or false/null depending on the type of the property.
    public virtual string GetString() { return null; }
    /// Returns the value of the SystemVoipState property, or false/null depending on the type of the property.
    public virtual SystemVoipState GetSystemVoipState() { return null; }
    /// Returns the value of the User property, or false/null depending on the type of the property.
    public virtual User GetUser() { return null; }
    /// Returns the value of the UserAccountAgeCategory property, or false/null depending on the type of the property.
    public virtual UserAccountAgeCategory GetUserAccountAgeCategory() { return null; }
    /// Returns the value of the UserCapabilityList property, or false/null depending on the type of the property.
    public virtual UserCapabilityList GetUserCapabilityList() { return null; }
    /// Returns the value of the UserList property, or false/null depending on the type of the property.
    public virtual UserList GetUserList() { return null; }
    /// Returns the value of the UserProof property, or false/null depending on the type of the property.
    public virtual UserProof GetUserProof() { return null; }
    /// Returns the value of the UserReportID property, or false/null depending on the type of the property.
    public virtual UserReportID GetUserReportID() { return null; }

    internal static Message ParseMessageHandle(IntPtr messageHandle)
    {
      if (messageHandle.ToInt64() == 0)
      {
        return null;
      }

      Message message = null;
      Message.MessageType message_type = (Message.MessageType)CAPI.ovr_Message_GetType(messageHandle);

      switch(message_type) {
        // OVR_MESSAGE_TYPE_START
        case Message.MessageType.Achievements_GetAllDefinitions:
        case Message.MessageType.Achievements_GetDefinitionsByName:
        case Message.MessageType.Achievements_GetNextAchievementDefinitionArrayPage:
          message = new MessageWithAchievementDefinitions(messageHandle);
          break;

        case Message.MessageType.Achievements_GetAllProgress:
        case Message.MessageType.Achievements_GetNextAchievementProgressArrayPage:
        case Message.MessageType.Achievements_GetProgressByName:
          message = new MessageWithAchievementProgressList(messageHandle);
          break;

        case Message.MessageType.Achievements_AddCount:
        case Message.MessageType.Achievements_AddFields:
        case Message.MessageType.Achievements_Unlock:
          message = new MessageWithAchievementUpdate(messageHandle);
          break;

        case Message.MessageType.Application_CheckAppDownloadProgress:
          message = new MessageWithAppDownloadProgressResult(messageHandle);
          break;

        case Message.MessageType.Application_CancelAppDownload:
        case Message.MessageType.Application_InstallAppUpdateAndRelaunch:
        case Message.MessageType.Application_StartAppDownload:
          message = new MessageWithAppDownloadResult(messageHandle);
          break;

        case Message.MessageType.GroupPresence_GetNextApplicationInviteArrayPage:
        case Message.MessageType.GroupPresence_GetSentInvites:
          message = new MessageWithApplicationInviteList(messageHandle);
          break;

        case Message.MessageType.Application_GetVersion:
          message = new MessageWithApplicationVersion(messageHandle);
          break;

        case Message.MessageType.AssetFile_Status:
        case Message.MessageType.AssetFile_StatusById:
        case Message.MessageType.AssetFile_StatusByName:
        case Message.MessageType.LanguagePack_GetCurrent:
          message = new MessageWithAssetDetails(messageHandle);
          break;

        case Message.MessageType.AssetFile_GetList:
          message = new MessageWithAssetDetailsList(messageHandle);
          break;

        case Message.MessageType.AssetFile_Delete:
        case Message.MessageType.AssetFile_DeleteById:
        case Message.MessageType.AssetFile_DeleteByName:
          message = new MessageWithAssetFileDeleteResult(messageHandle);
          break;

        case Message.MessageType.AssetFile_DownloadCancel:
        case Message.MessageType.AssetFile_DownloadCancelById:
        case Message.MessageType.AssetFile_DownloadCancelByName:
          message = new MessageWithAssetFileDownloadCancelResult(messageHandle);
          break;

        case Message.MessageType.AssetFile_Download:
        case Message.MessageType.AssetFile_DownloadById:
        case Message.MessageType.AssetFile_DownloadByName:
        case Message.MessageType.LanguagePack_SetCurrent:
          message = new MessageWithAssetFileDownloadResult(messageHandle);
          break;

        case Message.MessageType.Notification_AssetFile_DownloadUpdate:
          message = new MessageWithAssetFileDownloadUpdate(messageHandle);
          break;

        case Message.MessageType.Avatar_LaunchAvatarEditor:
          message = new MessageWithAvatarEditorResult(messageHandle);
          break;

        case Message.MessageType.User_GetBlockedUsers:
        case Message.MessageType.User_GetNextBlockedUserArrayPage:
          message = new MessageWithBlockedUserList(messageHandle);
          break;

        case Message.MessageType.Challenges_Create:
        case Message.MessageType.Challenges_DeclineInvite:
        case Message.MessageType.Challenges_Get:
        case Message.MessageType.Challenges_Join:
        case Message.MessageType.Challenges_Leave:
        case Message.MessageType.Challenges_UpdateInfo:
          message = new MessageWithChallenge(messageHandle);
          break;

        case Message.MessageType.Challenges_GetList:
        case Message.MessageType.Challenges_GetNextChallenges:
        case Message.MessageType.Challenges_GetPreviousChallenges:
          message = new MessageWithChallengeList(messageHandle);
          break;

        case Message.MessageType.Challenges_GetEntries:
        case Message.MessageType.Challenges_GetEntriesAfterRank:
        case Message.MessageType.Challenges_GetEntriesByIds:
        case Message.MessageType.Challenges_GetNextEntries:
        case Message.MessageType.Challenges_GetPreviousEntries:
          message = new MessageWithChallengeEntryList(messageHandle);
          break;

        case Message.MessageType.Cowatching_GetNextCowatchViewerArrayPage:
        case Message.MessageType.Cowatching_GetViewersData:
          message = new MessageWithCowatchViewerList(messageHandle);
          break;

        case Message.MessageType.Notification_Cowatching_ViewersDataChanged:
          message = new MessageWithCowatchViewerUpdate(messageHandle);
          break;

        case Message.MessageType.Cowatching_IsInSession:
        case Message.MessageType.Notification_Cowatching_InSessionChanged:
          message = new MessageWithCowatchingState(messageHandle);
          break;

        case Message.MessageType.RichPresence_GetDestinations:
        case Message.MessageType.RichPresence_GetNextDestinationArrayPage:
          message = new MessageWithDestinationList(messageHandle);
          break;

        case Message.MessageType.AbuseReport_ReportRequestHandled:
        case Message.MessageType.ApplicationLifecycle_RegisterSessionKey:
        case Message.MessageType.Challenges_Delete:
        case Message.MessageType.Cowatching_JoinSession:
        case Message.MessageType.Cowatching_LaunchInviteDialog:
        case Message.MessageType.Cowatching_LeaveSession:
        case Message.MessageType.Cowatching_RequestToPresent:
        case Message.MessageType.Cowatching_ResignFromPresenting:
        case Message.MessageType.Cowatching_SetPresenterData:
        case Message.MessageType.Cowatching_SetViewerData:
        case Message.MessageType.Entitlement_GetIsViewerEntitled:
        case Message.MessageType.GroupPresence_Clear:
        case Message.MessageType.GroupPresence_LaunchMultiplayerErrorDialog:
        case Message.MessageType.GroupPresence_LaunchRosterPanel:
        case Message.MessageType.GroupPresence_Set:
        case Message.MessageType.GroupPresence_SetDeeplinkMessageOverride:
        case Message.MessageType.GroupPresence_SetDestination:
        case Message.MessageType.GroupPresence_SetIsJoinable:
        case Message.MessageType.GroupPresence_SetLobbySession:
        case Message.MessageType.GroupPresence_SetMatchSession:
        case Message.MessageType.IAP_ConsumePurchase:
        case Message.MessageType.Notification_MarkAsRead:
        case Message.MessageType.RichPresence_Clear:
        case Message.MessageType.RichPresence_Set:
        case Message.MessageType.UserAgeCategory_Report:
          message = new Message(messageHandle);
          break;

        case Message.MessageType.Notification_GroupPresence_JoinIntentReceived:
          message = new MessageWithGroupPresenceJoinIntent(messageHandle);
          break;

        case Message.MessageType.Notification_GroupPresence_LeaveIntentReceived:
          message = new MessageWithGroupPresenceLeaveIntent(messageHandle);
          break;

        case Message.MessageType.GroupPresence_LaunchInvitePanel:
          message = new MessageWithInvitePanelResultInfo(messageHandle);
          break;

        case Message.MessageType.User_LaunchBlockFlow:
          message = new MessageWithLaunchBlockFlowResult(messageHandle);
          break;

        case Message.MessageType.User_LaunchFriendRequestFlow:
          message = new MessageWithLaunchFriendRequestFlowResult(messageHandle);
          break;

        case Message.MessageType.Notification_GroupPresence_InvitationsSent:
          message = new MessageWithLaunchInvitePanelFlowResult(messageHandle);
          break;

        case Message.MessageType.User_LaunchUnblockFlow:
          message = new MessageWithLaunchUnblockFlowResult(messageHandle);
          break;

        case Message.MessageType.Leaderboard_Get:
        case Message.MessageType.Leaderboard_GetNextLeaderboardArrayPage:
          message = new MessageWithLeaderboardList(messageHandle);
          break;

        case Message.MessageType.Leaderboard_GetEntries:
        case Message.MessageType.Leaderboard_GetEntriesAfterRank:
        case Message.MessageType.Leaderboard_GetEntriesByIds:
        case Message.MessageType.Leaderboard_GetNextEntries:
        case Message.MessageType.Leaderboard_GetPreviousEntries:
          message = new MessageWithLeaderboardEntryList(messageHandle);
          break;

        case Message.MessageType.Leaderboard_WriteEntry:
        case Message.MessageType.Leaderboard_WriteEntryWithSupplementaryMetric:
          message = new MessageWithLeaderboardDidUpdate(messageHandle);
          break;

        case Message.MessageType.User_GetLinkedAccounts:
          message = new MessageWithLinkedAccountList(messageHandle);
          break;

        case Message.MessageType.Notification_Livestreaming_StatusChange:
          message = new MessageWithLivestreamingStatus(messageHandle);
          break;

        case Message.MessageType.Voip_GetMicrophoneAvailability:
          message = new MessageWithMicrophoneAvailabilityState(messageHandle);
          break;

        case Message.MessageType.Notification_NetSync_ConnectionStatusChanged:
          message = new MessageWithNetSyncConnection(messageHandle);
          break;

        case Message.MessageType.Notification_NetSync_SessionsChanged:
          message = new MessageWithNetSyncSessionsChangedNotification(messageHandle);
          break;

        case Message.MessageType.User_GetOrgScopedID:
          message = new MessageWithOrgScopedID(messageHandle);
          break;

        case Message.MessageType.Party_GetCurrent:
          message = new MessageWithPartyUnderCurrentParty(messageHandle);
          break;

        case Message.MessageType.Notification_Party_PartyUpdate:
          message = new MessageWithPartyUpdateNotification(messageHandle);
          break;

        case Message.MessageType.ApplicationLifecycle_GetRegisteredPIDs:
          message = new MessageWithPidList(messageHandle);
          break;

        case Message.MessageType.IAP_GetNextProductArrayPage:
        case Message.MessageType.IAP_GetProductsBySKU:
          message = new MessageWithProductList(messageHandle);
          break;

        case Message.MessageType.IAP_LaunchCheckoutFlow:
          message = new MessageWithPurchase(messageHandle);
          break;

        case Message.MessageType.IAP_GetNextPurchaseArrayPage:
        case Message.MessageType.IAP_GetViewerPurchases:
        case Message.MessageType.IAP_GetViewerPurchasesDurableCache:
          message = new MessageWithPurchaseList(messageHandle);
          break;

        case Message.MessageType.PushNotification_Register:
          message = new MessageWithPushNotificationResult(messageHandle);
          break;

        case Message.MessageType.GroupPresence_LaunchRejoinDialog:
          message = new MessageWithRejoinDialogResult(messageHandle);
          break;

        case Message.MessageType.User_GetSdkAccounts:
          message = new MessageWithSdkAccountList(messageHandle);
          break;

        case Message.MessageType.GroupPresence_SendInvites:
          message = new MessageWithSendInvitesResult(messageHandle);
          break;

        case Message.MessageType.Media_ShareToFacebook:
          message = new MessageWithShareMediaResult(messageHandle);
          break;

        case Message.MessageType.ApplicationLifecycle_GetSessionKey:
        case Message.MessageType.Application_LaunchOtherApp:
        case Message.MessageType.Cowatching_GetPresenterData:
        case Message.MessageType.DeviceApplicationIntegrity_GetIntegrityToken:
        case Message.MessageType.Notification_AbuseReport_ReportButtonPressed:
        case Message.MessageType.Notification_ApplicationLifecycle_LaunchIntentChanged:
        case Message.MessageType.Notification_Cowatching_ApiNotReady:
        case Message.MessageType.Notification_Cowatching_ApiReady:
        case Message.MessageType.Notification_Cowatching_Initialized:
        case Message.MessageType.Notification_Cowatching_PresenterDataChanged:
        case Message.MessageType.Notification_Cowatching_SessionStarted:
        case Message.MessageType.Notification_Cowatching_SessionStopped:
        case Message.MessageType.Notification_Voip_MicrophoneAvailabilityStateUpdate:
        case Message.MessageType.Notification_Vrcamera_GetDataChannelMessageUpdate:
        case Message.MessageType.Notification_Vrcamera_GetSurfaceUpdate:
        case Message.MessageType.User_GetAccessToken:
          message = new MessageWithString(messageHandle);
          break;

        case Message.MessageType.Voip_SetSystemVoipSuppressed:
          message = new MessageWithSystemVoipState(messageHandle);
          break;

        case Message.MessageType.User_Get:
        case Message.MessageType.User_GetLoggedInUser:
        case Message.MessageType.User_GetLoggedInUserManagedInfo:
          message = new MessageWithUser(messageHandle);
          break;

        case Message.MessageType.UserAgeCategory_Get:
          message = new MessageWithUserAccountAgeCategory(messageHandle);
          break;

        case Message.MessageType.GroupPresence_GetInvitableUsers:
        case Message.MessageType.User_GetLoggedInUserFriends:
        case Message.MessageType.User_GetNextUserArrayPage:
          message = new MessageWithUserList(messageHandle);
          break;

        case Message.MessageType.User_GetNextUserCapabilityArrayPage:
          message = new MessageWithUserCapabilityList(messageHandle);
          break;

        case Message.MessageType.User_GetUserProof:
          message = new MessageWithUserProof(messageHandle);
          break;

        case Message.MessageType.Notification_Voip_SystemVoipState:
          message = new MessageWithSystemVoipState(messageHandle);
          break;

        case Message.MessageType.Notification_HTTP_Transfer:
          message = new MessageWithHttpTransferUpdate(messageHandle);
          break;

        case Message.MessageType.Platform_InitializeWithAccessToken:
        case Message.MessageType.Platform_InitializeStandaloneOculus:
        case Message.MessageType.Platform_InitializeAndroidAsynchronous:
        case Message.MessageType.Platform_InitializeWindowsAsynchronous:
          message = new MessageWithPlatformInitialize(messageHandle);
          break;

        default:
          message = PlatformInternal.ParseMessageHandle(messageHandle, message_type);
          if (message == null)
          {
            Debug.LogError(string.Format("Unrecognized message type {0}\n", message_type));
          }
          break;

        // OVR_MESSAGE_TYPE_END
      }

      return message;
    }

    public static Message PopMessage()
    {
      if (!Core.IsInitialized())
      {
        return null;
      }

      var messageHandle = CAPI.ovr_PopMessage();

      Message message = ParseMessageHandle(messageHandle);

      CAPI.ovr_FreeMessage(messageHandle);
      return message;
    }

    internal delegate Message ExtraMessageTypesHandler(IntPtr messageHandle, Message.MessageType message_type);
    internal static ExtraMessageTypesHandler HandleExtraMessageTypes { set; private get; }
  }

  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithAbuseReportRecording : Message<AbuseReportRecording>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithAbuseReportRecording(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override AbuseReportRecording GetAbuseReportRecording() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override AbuseReportRecording GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetAbuseReportRecording(msg);
      return new AbuseReportRecording(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithAchievementDefinitions : Message<AchievementDefinitionList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithAchievementDefinitions(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override AchievementDefinitionList GetAchievementDefinitions() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override AchievementDefinitionList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetAchievementDefinitionArray(msg);
      return new AchievementDefinitionList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithAchievementProgressList : Message<AchievementProgressList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithAchievementProgressList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override AchievementProgressList GetAchievementProgressList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override AchievementProgressList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetAchievementProgressArray(msg);
      return new AchievementProgressList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithAchievementUpdate : Message<AchievementUpdate>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithAchievementUpdate(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override AchievementUpdate GetAchievementUpdate() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override AchievementUpdate GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetAchievementUpdate(msg);
      return new AchievementUpdate(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithAppDownloadProgressResult : Message<AppDownloadProgressResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithAppDownloadProgressResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override AppDownloadProgressResult GetAppDownloadProgressResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override AppDownloadProgressResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetAppDownloadProgressResult(msg);
      return new AppDownloadProgressResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithAppDownloadResult : Message<AppDownloadResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithAppDownloadResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override AppDownloadResult GetAppDownloadResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override AppDownloadResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetAppDownloadResult(msg);
      return new AppDownloadResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithApplicationInviteList : Message<ApplicationInviteList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithApplicationInviteList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override ApplicationInviteList GetApplicationInviteList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override ApplicationInviteList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetApplicationInviteArray(msg);
      return new ApplicationInviteList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithApplicationVersion : Message<ApplicationVersion>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithApplicationVersion(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override ApplicationVersion GetApplicationVersion() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override ApplicationVersion GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetApplicationVersion(msg);
      return new ApplicationVersion(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithAssetDetails : Message<AssetDetails>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithAssetDetails(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override AssetDetails GetAssetDetails() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override AssetDetails GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetAssetDetails(msg);
      return new AssetDetails(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithAssetDetailsList : Message<AssetDetailsList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithAssetDetailsList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override AssetDetailsList GetAssetDetailsList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override AssetDetailsList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetAssetDetailsArray(msg);
      return new AssetDetailsList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithAssetFileDeleteResult : Message<AssetFileDeleteResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithAssetFileDeleteResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override AssetFileDeleteResult GetAssetFileDeleteResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override AssetFileDeleteResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetAssetFileDeleteResult(msg);
      return new AssetFileDeleteResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithAssetFileDownloadCancelResult : Message<AssetFileDownloadCancelResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithAssetFileDownloadCancelResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override AssetFileDownloadCancelResult GetAssetFileDownloadCancelResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override AssetFileDownloadCancelResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetAssetFileDownloadCancelResult(msg);
      return new AssetFileDownloadCancelResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithAssetFileDownloadResult : Message<AssetFileDownloadResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithAssetFileDownloadResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override AssetFileDownloadResult GetAssetFileDownloadResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override AssetFileDownloadResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetAssetFileDownloadResult(msg);
      return new AssetFileDownloadResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithAssetFileDownloadUpdate : Message<AssetFileDownloadUpdate>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithAssetFileDownloadUpdate(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override AssetFileDownloadUpdate GetAssetFileDownloadUpdate() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override AssetFileDownloadUpdate GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetAssetFileDownloadUpdate(msg);
      return new AssetFileDownloadUpdate(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithAvatarEditorResult : Message<AvatarEditorResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithAvatarEditorResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override AvatarEditorResult GetAvatarEditorResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override AvatarEditorResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetAvatarEditorResult(msg);
      return new AvatarEditorResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithBlockedUserList : Message<BlockedUserList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithBlockedUserList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override BlockedUserList GetBlockedUserList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override BlockedUserList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetBlockedUserArray(msg);
      return new BlockedUserList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithChallenge : Message<Challenge>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithChallenge(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override Challenge GetChallenge() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override Challenge GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetChallenge(msg);
      return new Challenge(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithChallengeList : Message<ChallengeList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithChallengeList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override ChallengeList GetChallengeList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override ChallengeList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetChallengeArray(msg);
      return new ChallengeList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithChallengeEntryList : Message<ChallengeEntryList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithChallengeEntryList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override ChallengeEntryList GetChallengeEntryList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override ChallengeEntryList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetChallengeEntryArray(msg);
      return new ChallengeEntryList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithCowatchViewerList : Message<CowatchViewerList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithCowatchViewerList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override CowatchViewerList GetCowatchViewerList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override CowatchViewerList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetCowatchViewerArray(msg);
      return new CowatchViewerList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithCowatchViewerUpdate : Message<CowatchViewerUpdate>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithCowatchViewerUpdate(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override CowatchViewerUpdate GetCowatchViewerUpdate() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override CowatchViewerUpdate GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetCowatchViewerUpdate(msg);
      return new CowatchViewerUpdate(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithCowatchingState : Message<CowatchingState>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithCowatchingState(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override CowatchingState GetCowatchingState() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override CowatchingState GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetCowatchingState(msg);
      return new CowatchingState(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithDestinationList : Message<DestinationList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithDestinationList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override DestinationList GetDestinationList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override DestinationList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetDestinationArray(msg);
      return new DestinationList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithGroupPresenceJoinIntent : Message<GroupPresenceJoinIntent>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithGroupPresenceJoinIntent(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override GroupPresenceJoinIntent GetGroupPresenceJoinIntent() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override GroupPresenceJoinIntent GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetGroupPresenceJoinIntent(msg);
      return new GroupPresenceJoinIntent(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithGroupPresenceLeaveIntent : Message<GroupPresenceLeaveIntent>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithGroupPresenceLeaveIntent(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override GroupPresenceLeaveIntent GetGroupPresenceLeaveIntent() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override GroupPresenceLeaveIntent GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetGroupPresenceLeaveIntent(msg);
      return new GroupPresenceLeaveIntent(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithInstalledApplicationList : Message<InstalledApplicationList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithInstalledApplicationList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override InstalledApplicationList GetInstalledApplicationList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override InstalledApplicationList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetInstalledApplicationArray(msg);
      return new InstalledApplicationList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithInvitePanelResultInfo : Message<InvitePanelResultInfo>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithInvitePanelResultInfo(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override InvitePanelResultInfo GetInvitePanelResultInfo() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override InvitePanelResultInfo GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetInvitePanelResultInfo(msg);
      return new InvitePanelResultInfo(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithLaunchBlockFlowResult : Message<LaunchBlockFlowResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithLaunchBlockFlowResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override LaunchBlockFlowResult GetLaunchBlockFlowResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override LaunchBlockFlowResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetLaunchBlockFlowResult(msg);
      return new LaunchBlockFlowResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithLaunchFriendRequestFlowResult : Message<LaunchFriendRequestFlowResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithLaunchFriendRequestFlowResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override LaunchFriendRequestFlowResult GetLaunchFriendRequestFlowResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override LaunchFriendRequestFlowResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetLaunchFriendRequestFlowResult(msg);
      return new LaunchFriendRequestFlowResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithLaunchInvitePanelFlowResult : Message<LaunchInvitePanelFlowResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithLaunchInvitePanelFlowResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override LaunchInvitePanelFlowResult GetLaunchInvitePanelFlowResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override LaunchInvitePanelFlowResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetLaunchInvitePanelFlowResult(msg);
      return new LaunchInvitePanelFlowResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithLaunchReportFlowResult : Message<LaunchReportFlowResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithLaunchReportFlowResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override LaunchReportFlowResult GetLaunchReportFlowResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override LaunchReportFlowResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetLaunchReportFlowResult(msg);
      return new LaunchReportFlowResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithLaunchUnblockFlowResult : Message<LaunchUnblockFlowResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithLaunchUnblockFlowResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override LaunchUnblockFlowResult GetLaunchUnblockFlowResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override LaunchUnblockFlowResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetLaunchUnblockFlowResult(msg);
      return new LaunchUnblockFlowResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithLeaderboardList : Message<LeaderboardList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithLeaderboardList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override LeaderboardList GetLeaderboardList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override LeaderboardList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetLeaderboardArray(msg);
      return new LeaderboardList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithLeaderboardEntryList : Message<LeaderboardEntryList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithLeaderboardEntryList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override LeaderboardEntryList GetLeaderboardEntryList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override LeaderboardEntryList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetLeaderboardEntryArray(msg);
      return new LeaderboardEntryList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithLinkedAccountList : Message<LinkedAccountList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithLinkedAccountList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override LinkedAccountList GetLinkedAccountList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override LinkedAccountList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetLinkedAccountArray(msg);
      return new LinkedAccountList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithLivestreamingApplicationStatus : Message<LivestreamingApplicationStatus>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithLivestreamingApplicationStatus(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override LivestreamingApplicationStatus GetLivestreamingApplicationStatus() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override LivestreamingApplicationStatus GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetLivestreamingApplicationStatus(msg);
      return new LivestreamingApplicationStatus(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithLivestreamingStartResult : Message<LivestreamingStartResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithLivestreamingStartResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override LivestreamingStartResult GetLivestreamingStartResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override LivestreamingStartResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetLivestreamingStartResult(msg);
      return new LivestreamingStartResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithLivestreamingStatus : Message<LivestreamingStatus>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithLivestreamingStatus(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override LivestreamingStatus GetLivestreamingStatus() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override LivestreamingStatus GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetLivestreamingStatus(msg);
      return new LivestreamingStatus(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithLivestreamingVideoStats : Message<LivestreamingVideoStats>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithLivestreamingVideoStats(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override LivestreamingVideoStats GetLivestreamingVideoStats() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override LivestreamingVideoStats GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetLivestreamingVideoStats(msg);
      return new LivestreamingVideoStats(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithMicrophoneAvailabilityState : Message<MicrophoneAvailabilityState>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithMicrophoneAvailabilityState(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override MicrophoneAvailabilityState GetMicrophoneAvailabilityState() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override MicrophoneAvailabilityState GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetMicrophoneAvailabilityState(msg);
      return new MicrophoneAvailabilityState(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithNetSyncConnection : Message<NetSyncConnection>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithNetSyncConnection(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override NetSyncConnection GetNetSyncConnection() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override NetSyncConnection GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetNetSyncConnection(msg);
      return new NetSyncConnection(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithNetSyncSessionList : Message<NetSyncSessionList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithNetSyncSessionList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override NetSyncSessionList GetNetSyncSessionList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override NetSyncSessionList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetNetSyncSessionArray(msg);
      return new NetSyncSessionList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithNetSyncSessionsChangedNotification : Message<NetSyncSessionsChangedNotification>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithNetSyncSessionsChangedNotification(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override NetSyncSessionsChangedNotification GetNetSyncSessionsChangedNotification() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override NetSyncSessionsChangedNotification GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetNetSyncSessionsChangedNotification(msg);
      return new NetSyncSessionsChangedNotification(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithNetSyncSetSessionPropertyResult : Message<NetSyncSetSessionPropertyResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithNetSyncSetSessionPropertyResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override NetSyncSetSessionPropertyResult GetNetSyncSetSessionPropertyResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override NetSyncSetSessionPropertyResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetNetSyncSetSessionPropertyResult(msg);
      return new NetSyncSetSessionPropertyResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithNetSyncVoipAttenuationValueList : Message<NetSyncVoipAttenuationValueList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithNetSyncVoipAttenuationValueList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override NetSyncVoipAttenuationValueList GetNetSyncVoipAttenuationValueList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override NetSyncVoipAttenuationValueList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetNetSyncVoipAttenuationValueArray(msg);
      return new NetSyncVoipAttenuationValueList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithOrgScopedID : Message<OrgScopedID>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithOrgScopedID(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override OrgScopedID GetOrgScopedID() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override OrgScopedID GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetOrgScopedID(msg);
      return new OrgScopedID(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithParty : Message<Party>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithParty(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override Party GetParty() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override Party GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetParty(msg);
      return new Party(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithPartyUnderCurrentParty : Message<Party>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithPartyUnderCurrentParty(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override Party GetParty() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override Party GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetParty(msg);
      return new Party(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithPartyID : Message<PartyID>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithPartyID(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override PartyID GetPartyID() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override PartyID GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetPartyID(msg);
      return new PartyID(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithPartyUpdateNotification : Message<PartyUpdateNotification>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithPartyUpdateNotification(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override PartyUpdateNotification GetPartyUpdateNotification() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override PartyUpdateNotification GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetPartyUpdateNotification(msg);
      return new PartyUpdateNotification(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithPidList : Message<PidList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithPidList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override PidList GetPidList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override PidList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetPidArray(msg);
      return new PidList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithProductList : Message<ProductList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithProductList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override ProductList GetProductList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override ProductList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetProductArray(msg);
      return new ProductList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithPurchase : Message<Purchase>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithPurchase(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override Purchase GetPurchase() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override Purchase GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetPurchase(msg);
      return new Purchase(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithPurchaseList : Message<PurchaseList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithPurchaseList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override PurchaseList GetPurchaseList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override PurchaseList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetPurchaseArray(msg);
      return new PurchaseList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithPushNotificationResult : Message<PushNotificationResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithPushNotificationResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override PushNotificationResult GetPushNotificationResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override PushNotificationResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetPushNotificationResult(msg);
      return new PushNotificationResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithRejoinDialogResult : Message<RejoinDialogResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithRejoinDialogResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override RejoinDialogResult GetRejoinDialogResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override RejoinDialogResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetRejoinDialogResult(msg);
      return new RejoinDialogResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithSdkAccountList : Message<SdkAccountList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithSdkAccountList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override SdkAccountList GetSdkAccountList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override SdkAccountList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetSdkAccountArray(msg);
      return new SdkAccountList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithSendInvitesResult : Message<SendInvitesResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithSendInvitesResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override SendInvitesResult GetSendInvitesResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override SendInvitesResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetSendInvitesResult(msg);
      return new SendInvitesResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithShareMediaResult : Message<ShareMediaResult>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithShareMediaResult(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override ShareMediaResult GetShareMediaResult() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override ShareMediaResult GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetShareMediaResult(msg);
      return new ShareMediaResult(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithString : Message<string>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithString(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override string GetString() { return Data; }
    /// Retrieves the String payload from the response Message. Used internally by Platform SDK.
    protected override string GetDataFromMessage(IntPtr c_message)
    {
      return CAPI.ovr_Message_GetString(c_message);
    }
  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithSystemVoipState : Message<SystemVoipState>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithSystemVoipState(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override SystemVoipState GetSystemVoipState() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override SystemVoipState GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetSystemVoipState(msg);
      return new SystemVoipState(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithUser : Message<User>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithUser(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override User GetUser() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override User GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetUser(msg);
      return new User(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithUserAccountAgeCategory : Message<UserAccountAgeCategory>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithUserAccountAgeCategory(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override UserAccountAgeCategory GetUserAccountAgeCategory() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override UserAccountAgeCategory GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetUserAccountAgeCategory(msg);
      return new UserAccountAgeCategory(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithUserList : Message<UserList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithUserList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override UserList GetUserList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override UserList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetUserArray(msg);
      return new UserList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithUserCapabilityList : Message<UserCapabilityList>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithUserCapabilityList(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override UserCapabilityList GetUserCapabilityList() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override UserCapabilityList GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetUserCapabilityArray(msg);
      return new UserCapabilityList(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithUserProof : Message<UserProof>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithUserProof(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override UserProof GetUserProof() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override UserProof GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetUserProof(msg);
      return new UserProof(obj);
    }

  }
  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithUserReportID : Message<UserReportID>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithUserReportID(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override UserReportID GetUserReportID() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override UserReportID GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetUserReportID(msg);
      return new UserReportID(obj);
    }

  }

  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithLeaderboardDidUpdate : Message<bool>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithLeaderboardDidUpdate(IntPtr c_message) : base(c_message) { }
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override bool GetLeaderboardDidUpdate() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override bool GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetLeaderboardUpdateStatus(msg);
      return CAPI.ovr_LeaderboardUpdateStatus_GetDidUpdate(obj);
    }
  }

  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithHttpTransferUpdate : Message<HttpTransferUpdate>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithHttpTransferUpdate(IntPtr c_message) : base(c_message) {}
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override HttpTransferUpdate GetHttpTransferUpdate() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override HttpTransferUpdate GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetHttpTransferUpdate(msg);
      return new HttpTransferUpdate(obj);
    }
  }

  /// Represents a response from the backend with a typed and structured model payload. See more details here:'https://developer.oculus.com/documentation/native/ps-requests-and-messages/'.
  /// Your app should constantly check the message queue for messages from the Platform SDK. We recommend that you check the queue every frame for new messages.
  public class MessageWithPlatformInitialize : Message<PlatformInitialize>
  {
    /// A typed Message subclass that wraps a native message handle pointer. Used internally by Platform SDK to wrap the message.
    public MessageWithPlatformInitialize(IntPtr c_message) : base(c_message) {}
    /// Returns the retrieved the model payload. Intended to be used by clients to handle the structured payload.
    public override PlatformInitialize GetPlatformInitialize() { return Data; }
    /// Retrieves the model payload from the response Message. Used internally by Platform SDK to parse the response into the model.
    protected override PlatformInitialize GetDataFromMessage(IntPtr c_message)
    {
      var msg = CAPI.ovr_Message_GetNativeMessage(c_message);
      var obj = CAPI.ovr_Message_GetPlatformInitialize(msg);
      return new PlatformInitialize(obj);
    }
  }

}
