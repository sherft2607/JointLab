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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meta.Voice.Logging;
using Meta.WitAi.Composer.Integrations;
using Meta.WitAi.Json;
using UnityEngine.Events;

namespace Meta.WitAi.Composer.Data
{
    /// <summary>
    /// The composer context map is a json object shared between this client
    /// and the wit.ai server, used to pass information back and forth.
    ///
    /// There are a few special cases within the context map which are handled
    /// with their own CRUD methods.
    /// </summary>
    public class ComposerContextMap : PluggableBase<IContextMapReservedPathExtension>
    {
        /// <summary>
        /// These are paths which have special significance and should be handled with care.
        /// </summary>
        internal static HashSet<string> ReservedPaths = new HashSet<string>();

        // Project specific context data
        public WitResponseClass Data { get; private set; }

        /// <inheritdoc/>
        public IVLogger Logger { get; }  = LoggerRegistry.Instance.GetLogger(LogCategory.Composer);

        public UnityEvent OnContextMapChanged { get; } = new UnityEvent();

        public ComposerContextMap()
        {
            CheckForPlugins();
            Data = new WitResponseClass();
        }

        #region General CRUD
        // Return true if key exists
        public bool HasData(string key) => Data != null && Data.HasChild(key);

        /// <summary>
        /// Gets the parent node and returns the value of the last child key name.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="childNodeName">The name of the child that was used to find the parent</param>
        private WitResponseClass GetParentAndNodeName(string key, out string childNodeName)
        {
            // Split key on periods and iterate all portions except the last
            var children = key.Split(".");
            var parent = Data;
            for (int i = 0; i < children.Length - 1; i++)
            {
                // Get name
                var childName = children[i];
                if (!parent.HasChild(childName))
                {
                    // Get array name & index if possible
                    GetArrayNameAndIndex(childName, out string arrayName, out int arrayIndex);
                    if (!string.IsNullOrEmpty(arrayName) && parent.HasChild(arrayName))
                    {
                        // Use desired index
                        var array = parent[arrayName].AsArray;
                        if (arrayIndex >= 0 && arrayIndex < array.Count)
                        {
                            parent = array[arrayIndex].AsObject;
                            continue;
                        }
                        // Use index 0
                        if (array.Count > 0)
                        {
                            parent = array[0].AsObject;
                            continue;
                        }
                    }
                }

                // Use child name to find subchild
                parent = parent[childName].AsObject;
            }

            // Get last name & return parent node
            childNodeName = children.Last();
            return parent;
        }

        /// <summary>
        /// Get array name & index if possible from a child name
        /// </summary>
        private void GetArrayNameAndIndex(string childName, out string arrayName, out int arrayIndex)
        {
            // Check for array start
            int start = childName.IndexOf('[');
            if (start == -1)
            {
                arrayName = string.Empty;
                arrayIndex = -1;
                return;
            }

            // Set array name
            arrayName = childName.Substring(0, start);

            // Check for array end
            string remainder = childName.Substring(start + 1);
            int end = remainder.IndexOf(']');
            if (end != -1 &&
                int.TryParse(remainder.Substring(0, end), out int index))
            {
                arrayIndex = index;
            }
            else
            {
                VLog.W(GetType().Name, $"Could not determine array index for child: {childName}");
                arrayIndex = -1;
            }
        }

        /// <summary>
        /// Retrieves specific data associated with the given key from the context map.
        /// </summary>
        /// <typeparam name="T">The expected type of the data to retrieve.</typeparam>
        /// <param name="key">The key of the data to retrieve from the context map.</param>
        /// <param name="defaultValue">The default value to return if the key is not found.</param>
        /// <returns>The data of type T if found, defaultValue otherwise.</returns>
        public T GetData<T>(string key, T defaultValue = default(T))
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Invalid key");
            var parent = GetParentAndNodeName(key, out var nodeKey);
            if (parent == null)
            {
                return defaultValue;
            }
            return parent.GetChild<T>(nodeKey, defaultValue);
        }

        /// <summary>
        /// Sets or updates specific data in the context map with the provided value.  Allows for
        /// child setting via '.' for example: SetData("action_data.question_selection.points", 15)
        /// </summary>
        /// <typeparam name="T">The type of the data to set.</typeparam>
        /// <param name="key">The key under which to set the data in the context map.</param>
        /// <param name="newValue">The new value to be set for the specified key.</param>
        public void SetData<T>(string key, T newValue)
        {
            // Ensure an error is thrown for an invalid key
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Invalid key");

            // Finds/generates desired parent node
            var parent = GetParentAndNodeName(key, out var nodeKey);

            // Use token directly
            if (newValue is WitResponseNode responseNode)
            {
                parent[nodeKey] = responseNode;
            }
            // Serialize into token and assign to data
            else
            {
                parent[nodeKey] = JsonConvert.SerializeToken<T>(newValue);
            }

            OnContextMapChanged?.Invoke();
        }

        /// <summary>
        /// Removes the specified data from the context map.
        /// </summary>
        /// <param name="key">the key of context item to remove</param>
        public void ClearData(string key)
        {
            // Ignore with invalid key
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            Data?.Remove(key);
            OnContextMapChanged?.Invoke();
        }

        /// <summary>
        /// Removes all data which hasn't been tagged as "reserved".
        /// </summary>
        public void ClearAllNonReservedData()
        {
            foreach (var key in Data.ChildNodeNames)
            {
                if (ReservedPaths.Contains(key)) continue;
                Data.Remove(key);
            }

            OnContextMapChanged?.Invoke();
        }

        /// <returns>A collection of paths which are reserved by the system</returns>
        public List<string> GetReservedPaths() => ReservedPaths.ToList();

        /// <summary>
        /// Exports the context map as a JSON string.
        /// </summary>
        /// <returns>The JSON string representation of the context map.</returns>
        public string GetJson(bool ignoreEmptyFields = false)
        {
            if (Data == null)
            {
                return "{}";
            }

            try
            {
                return Data.ToString(ignoreEmptyFields);
            }
            catch (Exception e)
            {
                VLog.E($"Composer Context Map - Decode Failed\n{e}");
            }

            return "{}";
        }

        /// <summary>
        /// Quick string log of all data within data class
        /// </summary>
        public override string ToString()
        {
            if (Data == null || Data.ChildNodeNames.Length == 0)
            {
                return "No Data";
            }
            var results = new StringBuilder();
            foreach (var key in Data.ChildNodeNames)
            {
                results.AppendLine($"\t{key}: {GetData<string>(key, "-")}");
            }
            return results.ToString();
        }
        #endregion

        /// <summary>
        /// Update data from a response node
        /// </summary>
        public bool UpdateData(WitResponseNode responseNode)
        {
            // Get new data if applicable
            var checkData = responseNode?[WitComposerConstants.ENDPOINT_COMPOSER_PARAM_CONTEXT_MAP]?.AsObject;
            if (checkData == null || checkData.Count == 0)
            {
                return false;
            }

            // Check and update each changed parameter
            var updated = UpdateDataObject(Data, checkData);

            if (updated)
            {
                OnContextMapChanged?.Invoke();
            }

            // Return update determination
            return updated;
        }

        /// <summary>
        /// Update data object if node is not equal
        /// </summary>
        private bool UpdateDataObject(WitResponseClass oldClass, WitResponseClass newClass)
        {
            var updated = false;
            foreach (var nodeName in newClass.ChildNodeNames)
            {
                var oldVal = oldClass[nodeName];
                var newVal = newClass[nodeName];
                if (ReservedPaths.Contains(nodeName)
                    || WitResponseNode.Equals(oldVal, newVal))
                {
                    continue;
                }
                updated = true;
                Data[nodeName] = newVal;
                Logger.Verbose("Update Context Map Key: '{0}'\nFrom: {1}\nTo: {2}", nodeName, oldVal, newVal);
            }
            return updated;
        }

        /// <summary>
        /// Links all the persistent data we don't want to erase in the given map to this one.
        /// </summary>
        /// <param name="otherMap">the map object to copy</param>
        /// <param name="composerTarget">the composer object whose to whose context map we should be copying</param>
        public void CopyPersistentData(ComposerContextMap otherMap, ComposerService composerTarget)
        {
            if (otherMap.LoadedPlugins == null) return;

            LoadedPlugins = otherMap.LoadedPlugins;

            // reinitialize each plugin to new composer, then update
            foreach (var contextMapReservedPathExtension in otherMap.LoadedPlugins)
            {
                var plugin = (BaseReservedContextPath)contextMapReservedPathExtension;
                plugin.AssignTo(composerTarget);  //set new composer
                plugin.UpdateContextMap();  //copy the data
            }
        }
    }

    /// <summary>
    /// An interface tag for loading in external plugins which manipulate
    /// the context map entries.
    /// </summary>
    public interface IContextMapReservedPathExtension
    {
        /// <summary>
        /// Adds the specific reserved path to Composer and completes whatever
        /// other initialization is required.
        /// </summary>
        public void AssignTo(ComposerService composer);
    }
}
