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

#if META_INTERACTION_SDK_DEFINED
using Oculus.Interaction;
#endif // META_INTERACTION_SDK_DEFINED

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meta.XR.BuildingBlocks;
using Meta.XR.BuildingBlocks.Editor;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.MRUtilityKit.BuildingBlocks
{
    public class InstantContentPlacementInstallationRoutine : InstallationRoutine
    {
        internal enum InteractionPreset
        {
            PointAndPlaceWithController,
            GrabAndPlace,
            CustomMode
        }

        [SerializeField, Variant(Behavior = VariantAttribute.VariantBehavior.Definition,
            Description = "Interaction mode for placing the GameObject. Choose CustomMode if you want to use your own interaction system.",
            Default = InteractionPreset.PointAndPlaceWithController)]
        internal InteractionPreset _interactionPreset;

        public override async Task<List<GameObject>> InstallAsync(BlockData block, GameObject selectedGameObject)
        {
            if (_interactionPreset == InteractionPreset.CustomMode)
            {
                return new List<GameObject>();
            }

            selectedGameObject ??= await InstallBlockData(BlockDataIds.Cube);
            TryAddComponent<PlaceWithAnchor>(selectedGameObject, out var placeWithAnchor);
            placeWithAnchor.Target = selectedGameObject.transform;
            Undo.RegisterCompleteObjectUndo(selectedGameObject, $"Modify {selectedGameObject.name}");

            if (_interactionPreset == InteractionPreset.PointAndPlaceWithController)
            {
                var rayVisualizer = FindFirstObjectByType<VisualizeEnvRaycast>();
                var hasRaycastVisualizer = rayVisualizer != null;
                if (!hasRaycastVisualizer)
                {
                    rayVisualizer = Instantiate(Prefab.transform.GetChild(0)).GetComponent<VisualizeEnvRaycast>();
                    rayVisualizer.gameObject.name = $"{Utils.BlockPublicTag} RaycastVisualizer";
                    Undo.RegisterCreatedObjectUndo(rayVisualizer.gameObject, "Create RaycastVisualizer");
                }

                TryAddComponent<PointAndLocate>(selectedGameObject, out var pointAndLocate);
                rayVisualizer._spaceLocator = pointAndLocate;

                var cameraRigBB = Utils.GetBlocksWithType<OVRCameraRig>().First();
                pointAndLocate._raycastOrigin = cameraRigBB.rightControllerAnchor;
                pointAndLocate.PreferredSurfaceOrientation = SpaceLocator.SurfaceOrientation.Vertical | SpaceLocator.SurfaceOrientation.HorizontalFaceUp |
                                                            SpaceLocator.SurfaceOrientation.HorizontalFaceDown;

                var controllerMapper = (await InstallBlockData(BlockDataIds.ControllerButtonsMapper)).GetComponent<ControllerButtonsMapper>();
                var buttonActionCreate = new ControllerButtonsMapper.ButtonClickAction
                {
                    Title = "Place Object",
                    Button = OVRInput.Button.PrimaryIndexTrigger,
                    ButtonMode = ControllerButtonsMapper.ButtonClickAction.ButtonClickMode.OnButtonUp,
                    Callback = new UnityEvent()
                };

                UnityEventTools.AddPersistentListener(buttonActionCreate.Callback, pointAndLocate.Locate);
                controllerMapper.ButtonClickActions.Add(buttonActionCreate);

                UnityEventTools.AddPersistentListener(pointAndLocate.OnSpaceLocateCompleted, placeWithAnchor.OnLocateSpace);
            }
            else if (_interactionPreset == InteractionPreset.GrabAndPlace)
            {
#if META_INTERACTION_SDK_DEFINED
                // Grab interaction block on selected gameobject
                var grabbable = selectedGameObject.GetComponentInChildren<Grabbable>();
                var hasHandGrab = grabbable != null;

                if(!hasHandGrab)
                {
                    var handGrabId = Oculus.Interaction.Editor.BuildingBlocks.BlockDataIds.HandGrab;
                    grabbable = (await InstallBlockData(handGrabId, selectedGameObject)).GetComponent<Grabbable>();
                }
                grabbable.InjectOptionalThrowWhenUnselected(false);

                TryAddComponent<GrabAndLocate>(selectedGameObject, out var grabAndLocate);

                UnityEventTools.AddPersistentListener(grabAndLocate.OnSpaceLocateCompleted, placeWithAnchor.OnLocateSpace);
#endif // META_INTERACTION_SDK_DEFINED
            }

            return new List<GameObject>();
        }

        private bool TryAddComponent<T>(GameObject gameObject, out T component) where T : Component
        {
            if (!gameObject.TryGetComponent(out component))
            {
                component = Undo.AddComponent<T>(gameObject);
            }

            return component != null;
        }

        private static async Task<GameObject> InstallBlockData(string blockId, GameObject selectedGameObject = null)
        {
            var blockData = Utils.GetBlockData(blockId);
            var gameObjects = await blockData.InstallWithDependencies(selectedGameObject);
            return gameObjects.First();
        }
    }
}
