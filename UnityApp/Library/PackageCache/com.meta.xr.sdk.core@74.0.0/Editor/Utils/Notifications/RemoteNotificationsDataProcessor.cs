/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Linq;
using Meta.XR.Editor.UserInterface;
using UnityEditor;

namespace Meta.XR.Editor.Notifications
{
    internal static class RemoteNotificationsDataProcessor
    {
        private static IEnumerable<IUserInterfaceItem> GetNotificationGuideItems(this NotificationData data)
        {
            yield return new Label(data.title, UIStyles.GUIStyles.Title);
            yield return new Label(data.message, UIStyles.GUIStyles.Label);
        }

        public static Notification BuildNotificationFromData(this NotificationData data)
        {
            var notification = new Notification(data.id)
            {
                Items = data.GetNotificationGuideItems(),
                ShowCloseButton = !data.hideCloseButton,
                Duration = data.duration
            };

            notification.OnShow += () =>
            {
                EditorPrefs.SetInt(
                    data.GetShownNotificationKey(),
                    EditorPrefs.GetInt(data.GetShownNotificationKey()) + 1);
            };

            return notification;
        }

        public static bool IsNotificationValid(this NotificationData data, Validator validator)
        {
            var hasReachedShowLimit = EditorPrefs.GetInt(data.GetShownNotificationKey(), -1) >= data.timesShown;
            return !hasReachedShowLimit
                   && (data.filters ?? Enumerable.Empty<NotificationFilter>()).All(validator.ValidateFilter);
        }

        private static string GetShownNotificationKey(this NotificationData data) => $"Notification_Shown_{data.id}";
    }
}
