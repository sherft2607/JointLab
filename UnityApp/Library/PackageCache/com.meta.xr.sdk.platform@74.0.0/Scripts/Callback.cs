namespace Oculus.Platform
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// This class represents a callback function that can be used to handle messages from the platform.
    /// It provides a way to register and execute callback functions for specific message types (Message.MessageType),
    /// allowing developers to customize the handling of incoming messages.
    public static class Callback
    {
        #region Notification Callbacks: Exposed through Oculus.Platform.Platform

        /// This method sets the callback function for a specific notification message type.
        /// It takes two parameters: the type of notification message to set the callback for, and the callback function itself.
        /// If the provided callback is null, it throws an exception.
        /// It stores the callback function in a dictionary, using the notification message type as the key.
        /// If the notification message type is Notification_GroupPresence_JoinIntentReceived,
        /// it flushes the join intent notification queue by calling FlushJoinIntentNotificationQueue().
        internal static void SetNotificationCallback<T>(Message.MessageType type, Message<T>.Callback callback)
        {
            if (callback == null)
            {
                throw new Exception("Cannot provide a null notification callback.");
            }

            notificationCallbacks[type] = new RequestCallback<T>(callback);

            if (type == Message.MessageType.Notification_GroupPresence_JoinIntentReceived)
            {
                FlushJoinIntentNotificationQueue();
            }
        }

        /// This method sets the callback function for a specific notification message type.
        /// It takes two parameters: the type of notification message to set the callback for, and the callback function itself.
        /// If the provided callback is null, it throws an exception.
        /// It stores the callback function in a dictionary, using the notification message type as the key.
        internal static void SetNotificationCallback(Message.MessageType type, Message.Callback callback)
        {
            if (callback == null)
            {
                throw new Exception("Cannot provide a null notification callback.");
            }

            notificationCallbacks[type] = new RequestCallback(callback);
        }

        #endregion

        #region Adding and running request handlers

        /// This method adds a request to the mapping of callbacks.
        /// It takes one parameter: the request to be added.
        /// If the request ID is 0, it means an early out error happened in the C SDK and the request is not added to the mapping.
        /// An error message is logged in this case.
        internal static void AddRequest(Request request)
        {
            if (request.RequestID == 0)
            {
                // An early out error happened in the C SDK. Do not add it to the mapping of callbacks
                Debug.LogError("An unknown error occurred. Request failed.");
                return;
            }

            requestIDsToRequests[request.RequestID] = request;
        }

        /// This method runs the callbacks for all pending messages.
        /// It repeatedly pops a message from the message queue using Platform.Message.PopMessage() and handles it until the queue is empty.
        internal static void RunCallbacks()
        {
            while (true)
            {
                var msg = Platform.Message.PopMessage();
                if (msg == null)
                {
                    break;
                }

                HandleMessage(msg);
            }
        }

        /// This method runs a limited number of callbacks for pending messages.
        /// It pops a message from the message queue using Platform.Message.PopMessage() and handles it up to the specified limit.
        internal static void RunLimitedCallbacks(uint limit)
        {
            for (var i = 0; i < limit; ++i)
            {
                var msg = Platform.Message.PopMessage();
                if (msg == null)
                {
                    break;
                }

                HandleMessage(msg);
            }
        }

        /// This method is called when the application quits.
        /// It clears out all outstanding callbacks by clearing the request IDs to requests and notification callbacks dictionaries.
        internal static void OnApplicationQuit()
        {
            // Clear out all outstanding callbacks
            requestIDsToRequests.Clear();
            notificationCallbacks.Clear();
        }

        #endregion

        #region Callback Internals

        private static Dictionary<ulong, Request> requestIDsToRequests = new Dictionary<ulong, Request>();

        private static Dictionary<Message.MessageType, RequestCallback> notificationCallbacks =
            new Dictionary<Message.MessageType, RequestCallback>();

        private static bool hasRegisteredJoinIntentNotificationHandler = false;
        private static Message latestPendingJoinIntentNotifications;

        private static void FlushJoinIntentNotificationQueue()
        {
            hasRegisteredJoinIntentNotificationHandler = true;
            if (latestPendingJoinIntentNotifications != null)
            {
                HandleMessage(latestPendingJoinIntentNotifications);
            }

            latestPendingJoinIntentNotifications = null;
        }

        /// This class represents a callback function that can be used to handle responses HandleMessage(msg) to asynchronous requests made to the platform.
        /// It provides a way for developers to specify how they want to handle incoming messages, allowing them to customize the behavior of their application.
        /// By using this class, developers can ensure that their application is able to handle responses to asynchronous requests in a consistent and efficient manner.
        private class RequestCallback
        {
            private Message.Callback messageCallback;

            /// This method initializes a new instance of the RequestCallback class.
            /// It is used to handle asynchronous requests made to the platform.
            public RequestCallback()
            {
            }

            /// This method initializes a new instance of the RequestCallback class with a specified callback function.
            /// It sets the message callback to the provided callback function, which will be executed when a response is received for the associated request.
            public RequestCallback(Message.Callback callback)
            {
                this.messageCallback = callback;
            }

            /// This method handles a message by executing the associated callback function, if one is provided.
            /// It checks if a message callback has been set, and if so, it calls the callback function: messageCallback(msg) with the provided message as an argument.
            public virtual void HandleMessage(Message msg)
            {
                if (messageCallback != null)
                {
                    messageCallback(msg);
                }
            }
        }

        /// This class represents a callback function that can be used to handle responses HandleMessage(msg) to asynchronous requests made to the platform.
        /// It provides a way for developers to specify how they want to handle incoming messages, allowing them to customize the behavior of their application.
        /// By using this class, developers can ensure that their application is able to handle responses to asynchronous requests in a consistent and efficient manner.
        private sealed class RequestCallback<T> : RequestCallback
        {
            private Message<T>.Callback callback;

            /// This method initializes a new instance of the RequestCallback class with a specified callback function.
            /// It sets the message callback to the provided callback function, which will be executed when a response is received for the associated request.
            public RequestCallback(Message<T>.Callback callback)
            {
                this.callback = callback;
            }

            /// This method handles a message by executing the associated callback function, if one is provided.
            /// It checks if a message callback has been set, and if so, it calls the callback function: callback(msg) with the provided message as an argument.
            public override void HandleMessage(Message msg)
            {
                if (callback != null)
                {
                    if (msg is Message<T>)
                    {
                        callback((Message<T>)msg);
                    }
                    else
                    {
                        Debug.LogError("Unable to handle message: " + msg.GetType());
                    }
                }
            }
        }

        /// This method handles a message by executing the associated callback function.
        /// It checks if a message callback has been set for the request ID, and if so, it calls the request.HandleMessage(msg) method on the corresponding request object.
        /// If no callback is registered for the request ID, it checks if there is a callback handler registered for the message type and calls the callbackHolder.HandleMessage(msg) method on it.
        /// If there is no callback handler registered, it checks if the message is a Join Intent notification and queues it up for processing.
        internal static void HandleMessage(Message msg)
        {
            Request request;
            if (msg.RequestID != 0 && requestIDsToRequests.TryGetValue(msg.RequestID, out request))
            {
                try
                {
                    request.HandleMessage(msg);
                }
                finally
                {
                    requestIDsToRequests.Remove(msg.RequestID);
                }

                return;
            }

            RequestCallback callbackHolder;
            if (notificationCallbacks.TryGetValue(msg.Type, out callbackHolder))
            {
                callbackHolder.HandleMessage(msg);
            }
            // We need to queue up Join Intents because the callback runner will be called before a handler has beeen set.
            else if (!hasRegisteredJoinIntentNotificationHandler &&
                     msg.Type == Message.MessageType.Notification_GroupPresence_JoinIntentReceived)
            {
                latestPendingJoinIntentNotifications = msg;
            }
        }

        #endregion
    }
}
