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

#nullable enable
using UnityEditor;
using UnityEngine;
using Oculus.Interaction.Editor.QuickActions;
using Unity.XR.CoreUtils;

namespace Oculus.Interaction.Editor.UnityXR.QuickActions
{
    internal class UnityXRComprehensiveRigWizard : QuickActionsWizard
    {
        private const string MENU_NAME_ADD_NEW_RIG = QuickActionsWizard.MENU_FOLDER +
            "Add UnityXR Interaction Rig";

        public static readonly Template UnityXRCameraRig =
            new Template(
                "UnityXRCameraRig",
                "f9f107e8ecae49048bda9d4fd7e1d1ec");

        public static readonly Template UnityXRInteractionComprehensive =
            new Template(
                "UnityXRInteractionComprehensive",
                "5c149baf016750b42ac7e9cdbc905b96");

        [SerializeField]
        [Tooltip("The Interaction Rig will be added under this UnityXRCameraRig and reference its hands, controllers and hmd.")]
        [WizardDependency(Category = Category.Required,
            FindMethod = nameof(AssignExistingCameraRig),
            FixMethod = nameof(CreateMissingCameraRig))]
        private XROrigin _cameraRig;

        [MenuItem(MENU_NAME_ADD_NEW_RIG, priority = 100)]
        internal static void OpenWizard()
        {
            ShowWindow<UnityXRComprehensiveRigWizard>(null);
        }

        protected override void Create()
        {
            CreateInteractionRig();
        }

        internal GameObject CreateInteractionRig()
        {
            GameObject interactionRig = Templates.CreateFromTemplate(
                _cameraRig.transform, UnityXRInteractionComprehensive, asPrefab: true);

            UnityObjectAddedBroadcaster.HandleObjectWasAdded(interactionRig);
            Selection.activeObject = interactionRig;
            return interactionRig;
        }

        internal void AssignExistingCameraRig()
        {
            _cameraRig = Object.FindObjectOfType<XROrigin>();
        }

        public static XROrigin FindExistingCameraRig()
        {
            return Object.FindObjectOfType<XROrigin>();
        }

        internal void CreateMissingCameraRig()
        {
            GameObject cameraRig = Templates.CreateFromTemplate(
                null, UnityXRCameraRig, asPrefab: true);

            cameraRig.TryGetComponent(out _cameraRig);
            AssignExistingCameraRig();
        }
    }
}
