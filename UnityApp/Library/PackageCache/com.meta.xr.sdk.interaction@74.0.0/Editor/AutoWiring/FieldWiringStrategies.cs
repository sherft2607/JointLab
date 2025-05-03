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
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Oculus.Interaction.Editor
{
    public delegate Boolean FieldWiringStrategy(MonoBehaviour monoBehaviour, FieldInfo fieldInfo, Type type);

    public struct ComponentWiringStrategyConfig
    {
        public string FieldName { get; }
        public FieldWiringStrategy[] Methods { get; }

        public ComponentWiringStrategyConfig(string fieldName, FieldWiringStrategy[] methods)
        {
            FieldName = fieldName;
            Methods = methods;
        }
    }

    public class FieldWiringStrategies
    {
        public static bool WireFieldToAncestors(MonoBehaviour monoBehaviour, FieldInfo field, Type targetType)
        {
            for (var transform = monoBehaviour.transform.parent; transform != null; transform = transform.parent)
            {
                var component = transform.gameObject.GetComponent(targetType);
                if (component)
                {
                    field.SetValue(monoBehaviour, component);
                    EditorUtility.SetDirty(monoBehaviour);
                    return true;
                }
            }

            return false;
        }

        public static bool WireFieldToSceneComponent(MonoBehaviour monoBehaviour, FieldInfo field, Type targetType)
        {
            var rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var rootGameObject in rootObjs)
            {
                var component = rootGameObject.GetComponentInChildren(targetType, true);
                if (component != null)
                {
                    field.SetValue(monoBehaviour, component);
                    EditorUtility.SetDirty(monoBehaviour);
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Searches for the component in an array of GameObject Paths and wires the first found component under it
        /// </summary>
        public static bool WireFieldToPathComponent(MonoBehaviour monoBehaviour, FieldInfo field, Type targetType, params string[] gameobjectPaths)
        {
            foreach (var path in gameobjectPaths)
            {
                GameObject gameObject = GameObject.Find(path);
                if (gameObject != null)
                {
                    var component = gameObject.GetComponentInChildren(targetType, true);
                    if (component != null)
                    {
                        field.SetValue(monoBehaviour, component);
                        EditorUtility.SetDirty(monoBehaviour);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Searches for the component to wire in the local hierarchy of the target. It tries to minimise the score
        /// of each potential component by adding +1 if needs to go up in the hierarchy, and +3 if it needs to go down.
        /// </summary>
        public static bool WireFieldToNearestComponent(MonoBehaviour monoBehaviour, FieldInfo field, Type targetType)
        {
            GameObject[] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();

            List<Component> components = new List<Component>();
            foreach (GameObject rootGameObject in rootObjs)
            {
                components.AddRange(rootGameObject.GetComponentsInChildren(targetType, true));
            }

            List<Transform> ancestors = GetAncestors(monoBehaviour.transform);
            int bestScore = int.MaxValue;
            Component bestComponent = null;
            foreach (Component component in components)
            {
                List<Transform> ancestorsB = GetAncestors(component.transform);
                int score = GetHierarchyScore(ancestors, ancestorsB, 1, 3);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestComponent = component;
                }
            }

            if (bestComponent != null)
            {
                field.SetValue(monoBehaviour, bestComponent);
                EditorUtility.SetDirty(monoBehaviour);
                return true;
            }

            return false;

            int GetHierarchyScore(List<Transform> ancestorsA, List<Transform> ancestorsB,
                int upPenalty, int downPenalty)
            {
                int indexA = 0;
                int indexB = -1;

                for (indexA = 0; indexA < ancestorsA.Count; indexA++)
                {
                    indexB = ancestorsB.FindIndex(t => t == ancestorsA[indexA]);
                    if (indexB != -1)
                    {
                        break;
                    }
                }

                //no common hierarchy found
                //the score is the sum of both entire lists of ancestors
                if (indexB == -1)
                {
                    return ancestorsA.Count * upPenalty
                        + ancestorsB.Count * downPenalty;
                }

                return indexA * upPenalty
                    + indexB * downPenalty;
            }

            List<Transform> GetAncestors(Transform transform)
            {
                List<Transform> ancestors = new List<Transform>();
                while (transform != null)
                {
                    ancestors.Add(transform);
                    transform = transform.parent;
                }
                return ancestors;
            }
        }
    }
}
