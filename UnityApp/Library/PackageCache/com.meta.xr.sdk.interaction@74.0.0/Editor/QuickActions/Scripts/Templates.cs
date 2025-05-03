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
using UnityEditor;
using UnityEngine;
using InputModality = Oculus.Interaction.Editor.QuickActions.InteractorTemplate.InputModality;
using Object = UnityEngine.Object;

namespace Oculus.Interaction.Editor.QuickActions
{
    internal class Template
    {
        /// <summary>
        /// The instantiated prefab will be given this name
        /// </summary>
        public readonly string DisplayName;

        /// <summary>
        /// The GUID of the prefab asset
        /// </summary>
        public readonly string AssetGUID;

        /// <param name="displayName">The instantiated GameObject will be given this name.
        /// Does not need to correspond to the prefab asset name.</param>
        /// <param name="assetGUID">The GUID of the prefab asset.</param>
        public Template(string displayName, string assetGUID)
        {
            DisplayName = displayName;
            AssetGUID = assetGUID;
        }
    }

    internal class InteractorTemplate : Template
    {
        /// <summary>
        /// Indicates the prefered inputs for this
        /// interactor
        /// </summary>
        internal enum InputModality
        {
            Hand,
            Controller,
            HandAndNoController,
            ControllerAndNoHand,
            HandAndController,
            Any
        }

        public readonly InputModality Modality;

        public InteractorTemplate(string displayName, string assetGUID, InputModality modality = InputModality.Any) : base(displayName, assetGUID)
        {
            Modality = modality;
        }
    }

    internal static class Templates
    {
        public static event Action<Template, GameObject> WhenObjectCreated = delegate { };

        #region Interactables

        public static readonly Template RayCanvasInteractable =
            new Template(
                "ISDK_RayInteraction",
                "8369d93f7b6b99742bbea0649a41b7b1");

        public static readonly Template PokeCanvasInteractable =
            new Template(
                "ISDK_PokeInteraction",
                "4db41829582c7d24f80ee9603868dd67");

        public static readonly Template HandGrabInteractable =
            new Template(
                "ISDK_HandGrabInteraction",
                "6ee61821e0d5b094a8d732834b365b21");

        public static readonly Template DistanceGrabInteractable_ToHand =
            new Template(
                "ISDK_DistanceHandGrabInteraction",
                "e9f3427813a446742ad647fe9aa1547c"
                );

        public static readonly Template DistanceGrabInteractable_HandTo =
            new Template(
                "ISDK_DistanceHandGrabInteraction",
                "86bc3e593d22eec429ae3910a86ffb03"
                );

        public static readonly Template DistanceGrabInteractable_AnchorAtHand =
            new Template(
                "ISDK_DistanceHandGrabInteraction",
                "4c91b23cd5294f84eb9ab4b61a183f63"
                );

        public static readonly Template DistanceGrabInteractable_SnapZone =
            new Template(
                "ISDK_DistanceGrabSnapZone",
                "b63e95c77d701bd44a848c316f6e9fa9"
                );

        public static readonly Template RayGrabInteractable =
            new Template(
                "ISDK_RayGrabInteraction",
                "f1c9f92f4fa1883459a0dfa57e136262");


        public static readonly Template TeleportInteractable =
            new Template(
                "ISDK_TeleportInteraction",
                "70cf16e50c457a54cad0b0a504b7d646");

        #endregion Interactables

        #region Interactors

        public static readonly InteractorTemplate HandGrabInteractor =
            new InteractorTemplate(
                "HandGrabInteractor",
                "f0a90b2d303e7744fa8c9d3c6e2418a4",
                InputModality.Hand);

        public static readonly InteractorTemplate HandPokeInteractor =
            new InteractorTemplate(
                "PokeInteractor",
                "abe5a2b766edc96438786a6785a2f74b",
                InputModality.Hand);

        public static readonly InteractorTemplate HandRayInteractor =
            new InteractorTemplate(
                "RayInteractor",
                "a6df867c95b07224498cb3ea2d410ce5",
                InputModality.HandAndNoController);

        public static readonly InteractorTemplate DistanceHandGrabInteractor =
            new InteractorTemplate(
                "DistanceHandGrabInteractor",
                "7ea5ce61c81c5ba40a697e2642e80c83",
                InputModality.Hand);

        public static readonly InteractorTemplate HandTeleportInteractor =
            new InteractorTemplate(
                "HandTeleportInteractorGroup",
                "1b3597a7837cb3545b4a4f6e30856067",
                InputModality.HandAndNoController);

        public static readonly InteractorTemplate MicrogestureTeleportInteractor =
            new InteractorTemplate(
                "MicroGesturesLocomotionHandInteractorGroup",
                "dd5c3bd9d99285c4984565b9e3dbfb98",
                InputModality.HandAndNoController);

        public static readonly InteractorTemplate ControllerPokeInteractor =
            new InteractorTemplate(
                "PokeInteractor",
                "ef9bd966f1a997b4cb9eef15b0620b24",
                InputModality.ControllerAndNoHand);

        public static readonly InteractorTemplate ControllerRayInteractor =
            new InteractorTemplate(
                "RayInteractor",
                "074f70ff54d0c6d489aaeba17f4bc66d",
                InputModality.Controller);

        public static readonly InteractorTemplate ControllerGrabInteractor =
            new InteractorTemplate(
                "GrabInteractor",
                "069b845e75891f04bb2e512a8ebf3b78",
                InputModality.ControllerAndNoHand);

        public static readonly InteractorTemplate ControllerDistanceGrabInteractor =
            new InteractorTemplate(
                "DistanceGrabInteractor",
                "d9ef0d4c78b4bfd409cb884dfe1524d6",
                InputModality.ControllerAndNoHand);

        public static readonly InteractorTemplate ControllerTeleportInteractor =
            new InteractorTemplate(
                "ControllerTeleportInteractorGroup",
                "dd6fa3a95e908604fa8656608ea793a1",
                InputModality.Controller);

        private static Dictionary<InteractorTypes, InteractorTemplate> _handInteractorTemplates = new()
        {
            [InteractorTypes.Grab] = HandGrabInteractor,
            [InteractorTypes.Poke] = HandPokeInteractor,
            [InteractorTypes.Ray] = HandRayInteractor,
            [InteractorTypes.DistanceGrab] = DistanceHandGrabInteractor,
            [InteractorTypes.Teleport] = HandTeleportInteractor,

        };

        private static Dictionary<InteractorTypes, InteractorTemplate> _controllerInteractorTemplates = new()
        {
            [InteractorTypes.Grab] = ControllerGrabInteractor,
            [InteractorTypes.Poke] = ControllerPokeInteractor,
            [InteractorTypes.Ray] = ControllerRayInteractor,
            [InteractorTypes.DistanceGrab] = ControllerDistanceGrabInteractor,
            [InteractorTypes.Teleport] = ControllerTeleportInteractor,
        };

        private static Dictionary<InteractorTypes, InteractorTemplate> _controllerHandInteractorTemplates = new()
        {
            [InteractorTypes.Grab] = HandGrabInteractor,
            [InteractorTypes.Poke] = HandPokeInteractor,
            [InteractorTypes.Ray] = ControllerRayInteractor,
            [InteractorTypes.DistanceGrab] = DistanceHandGrabInteractor,
            [InteractorTypes.Teleport] = ControllerTeleportInteractor,
        };

        /// <summary>
        /// Gets the <see cref="Template"/> for a Hand interactor type
        /// </summary>
        public static bool TryGetHandInteractorTemplate(InteractorTypes type, out InteractorTemplate template)
        {
            return _handInteractorTemplates.TryGetValue(type, out template);
        }

        /// <summary>
        /// Gets the <see cref="Template"/> for a Controller interactor type
        /// </summary>
        public static bool TryGetControllerInteractorTemplate(InteractorTypes type, out InteractorTemplate template)
        {
            return _controllerInteractorTemplates.TryGetValue(type, out template);
        }

        /// <summary>
        /// Gets the <see cref="Template"/> for a Controller driven Hand interactor type
        /// </summary>
        public static bool TryGetControllerHandInteractorTemplate(InteractorTypes type, out InteractorTemplate template)
        {
            return _controllerHandInteractorTemplates.TryGetValue(type, out template);
        }

        #endregion Interactors

        /// <summary>
        /// Add an interactable prefab to a GameObject and register it in the Undo stack.
        /// Also registers with the cleanup list, to be optionally removed
        /// when the user cancels out of the wizard.
        /// </summary>
        /// <param name="parent">The Transform the prefab will be instantiated under</param>
        /// <param name="template">The <see cref="Template"/>to be instantiated</param>
        /// <returns>The GameObject at the root of the prefab.</returns>
        public static GameObject CreateFromTemplate(Transform parent, Template template, bool asPrefab = false)
        {
            GameObject result;

            // Retain prefab link
            if (asPrefab)
            {
                result = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadMainAssetAtPath(
                    AssetDatabase.GUIDToAssetPath(template.AssetGUID))) as GameObject;

            }
            else
            {
                result = Object.Instantiate(AssetDatabase.LoadMainAssetAtPath(
                    AssetDatabase.GUIDToAssetPath(template.AssetGUID))) as GameObject;
            }
            result.name = template.DisplayName;
            result.transform.SetParent(parent?.transform, false);
            Undo.RegisterCreatedObjectUndo(result, "Add " + template.DisplayName);
            WhenObjectCreated.Invoke(template, result);
            return result;
        }
    }
}
