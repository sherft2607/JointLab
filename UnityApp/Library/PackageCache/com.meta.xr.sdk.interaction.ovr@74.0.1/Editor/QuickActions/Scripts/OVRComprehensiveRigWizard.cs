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

using Oculus.Interaction.Editor;
using Oculus.Interaction.Editor.QuickActions;
using Oculus.Interaction.Input;
using UnityEditor;
using UnityEngine;
using Oculus.Interaction.Locomotion;

#if UNITY_2022_1_OR_NEWER
using System.IO;
#endif

namespace Oculus.Interaction.OVR.Editor.QuickActions
{
    internal class OVRComprehensiveRigWizard : QuickActionsWizard
    {
        private const string MENU_NAME_ADD_NEW_RIG = MENU_FOLDER +
            "Add OVR Interaction Rig";

        private const string LEFT_INTERACTIONS_NAME = "LeftInteractions";
        private const string RIGHT_INTERACTIONS_NAME = "RightInteractions";

        public static readonly Template OVRCameraRig =
            new Template(
                "OVRCameraRig",
                "126d619cf4daa52469682f85c1378b4a");

        public static readonly Template OVRInteractionComprehensive =
            new Template(
                "OVRInteractionComprehensive",
                "0a7d2469f24041c4284c66706f84c45e");

        public static readonly Template ComprehensiveInteractions =
           new Template(
               "ComprehensiveInteractions",
               "fac1b34d244291e409464389de307e81");

#if UNITY_2022_1_OR_NEWER
        [SerializeField]
        [WizardSetting]
        [Tooltip("Unpacks the interaction rig prefab and creates a common variant between the left and right hierarchies " +
            "so you can easily inspect and apply modifications to both sides.")]
        private bool _generateAsEditableCopy = true;

        [SerializeField]
        [WizardSetting]
        [Tooltip("Path within Assets/ to store the editable variant if GenerateAsEditableCopy is selected")]
        [ConditionalHide(nameof(_generateAsEditableCopy), true,
            ConditionalHideAttribute.DisplayMode.ShowIfTrue)]
        private string _prefabPath = "InteractionSDK/ComprehensiveInteraction.prefab";
#endif

        [SerializeField]
        [WizardSetting]
        [Tooltip("Enables smooth locomotion in the FirstPersonLocomotor, including player velocity and falling.")]
        private bool _smoothLocomotion = true;

        [SerializeField]
        [Tooltip("The Interaction Rig will be added under this OVRCameraRig and reference its hands, controllers and hmd.")]
        [WizardDependency(Category = Category.Required,
            FindMethod = nameof(AssignExistingCameraRig),
            FixMethod = nameof(CreateMissingCameraRig))]
        private OVRCameraRig _cameraRig;

        [MenuItem(MENU_NAME_ADD_NEW_RIG, priority = 100)]
        internal static void OpenWizard()
        {
            ShowWindow<OVRComprehensiveRigWizard>(null);
        }

        protected override void Create()
        {
            CreateInteractionRig();
        }

        internal GameObject CreateInteractionRig()
        {
            Template comprehensiveRigTemplate = OVRInteractionComprehensive;

            GameObject interactionRig = Templates.CreateFromTemplate(
                _cameraRig.transform, comprehensiveRigTemplate, asPrefab: true);

#if UNITY_2022_1_OR_NEWER
            if (_generateAsEditableCopy)
            {
                MakeInteractionRigEditable(interactionRig);
            }
#endif
            foreach (OVRHand ovrCameraRigHand in _cameraRig.trackingSpace.GetComponentsInChildren<OVRHand>())
            {
                DisableDuplicateVisuals(ovrCameraRigHand);
            }

            UpdateSmoothLocomotion(interactionRig);

            UnityObjectAddedBroadcaster.HandleObjectWasAdded(interactionRig);
            Selection.activeObject = interactionRig;
            return interactionRig;
        }

#if UNITY_2022_1_OR_NEWER
        private void MakeInteractionRigEditable(GameObject interactionRig)
        {
            PrefabUtility.UnpackPrefabInstance(interactionRig,
                PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

            //Create the folder structure for storing the new prefab variant
            string fileName = Path.GetFileNameWithoutExtension(_prefabPath);
            string directoryName = Path.GetDirectoryName(_prefabPath);
            string prefabFolder = Path.Combine("Assets", directoryName);
            string savePath = Path.Combine(prefabFolder, $"{fileName}.prefab");
            if (!Directory.Exists(prefabFolder))
            {
                Directory.CreateDirectory(prefabFolder);
            }

            //generate an editable prefab variant out of the comprehensive prefab in the package
            Object comprehensiveInteractionsSource = AssetDatabase.LoadMainAssetAtPath(
                AssetDatabase.GUIDToAssetPath(ComprehensiveInteractions.AssetGUID));
            GameObject comprehensiveInteractionsPrefab = PrefabUtility.InstantiatePrefab(comprehensiveInteractionsSource) as GameObject;
            GameObject comprehensiveInteractionsVariant = PrefabUtility.SaveAsPrefabAsset(comprehensiveInteractionsPrefab, savePath);
            GameObject.DestroyImmediate(comprehensiveInteractionsPrefab);

            GameObject leftInteractions = interactionRig.transform.Find(LEFT_INTERACTIONS_NAME).gameObject;
            GameObject rightInteractions = interactionRig.transform.Find(RIGHT_INTERACTIONS_NAME).gameObject;

            PrefabReplacingSettings replaceSettings = new PrefabReplacingSettings()
            {
                changeRootNameToAssetName = false,
                logInfo = true,
                objectMatchMode = ObjectMatchMode.ByHierarchy,
                prefabOverridesOptions = PrefabOverridesOptions.KeepAllPossibleOverrides
            };

            PrefabUtility.ReplacePrefabAssetOfPrefabInstance(leftInteractions,
                comprehensiveInteractionsVariant, replaceSettings, InteractionMode.AutomatedAction);
            PrefabUtility.ReplacePrefabAssetOfPrefabInstance(rightInteractions,
                comprehensiveInteractionsVariant, replaceSettings, InteractionMode.AutomatedAction);
        }
#endif

        // Disable visuals in existing Camera Rig once ISDK visuals are present
        public static void DisableDuplicateVisuals(OVRHand hand)
        {
            if (hand.TryGetComponent<OVRSkeletonRenderer>(out var skeletonRenderer))
                skeletonRenderer.enabled = false;
            if (hand.TryGetComponent<OVRMesh>(out var mesh))
                mesh.enabled = false;
            if (hand.TryGetComponent<OVRMeshRenderer>(out var meshRenderer))
                meshRenderer.enabled = false;
            if (hand.TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer))
                skinnedMeshRenderer.enabled = false;
            Debug.Log("Duplicate hand visual components in OVRCameraRig disabled");
        }

        private void UpdateSmoothLocomotion(GameObject interactionRig)
        {
            FirstPersonLocomotor locomotor = interactionRig.GetComponentInChildren<FirstPersonLocomotor>();
            if (_smoothLocomotion)
            {
                locomotor.EnableMovement();
            }
            else
            {
                locomotor.DisableMovement();
            }
        }

        internal void AssignExistingCameraRig()
        {
            _cameraRig = FindExistingCameraRig();
        }

        internal void CreateMissingCameraRig()
        {
            GameObject cameraRig = Templates.CreateFromTemplate(
                null, OVRCameraRig, asPrefab: true);

            cameraRig.TryGetComponent(out _cameraRig);

            if (cameraRig.TryGetComponent(out OVRManager ovrManager))
            {
                //_trackingOriginType cannot be set in the Editor via the available trackingOriginType.Set
                //as that one is only for runtime
                var trackingOriginTypeField = AutoWiring.FindField("_trackingOriginType", typeof(OVRManager));
                trackingOriginTypeField.SetValue(ovrManager, OVRManager.TrackingOrigin.FloorLevel);
                ovrManager.controllerDrivenHandPosesType = OVRManager.ControllerDrivenHandPosesType.ConformingToController;
            }
        }

        public static OVRCameraRig FindExistingCameraRig()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return Object.FindObjectOfType<OVRCameraRig>();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public static OVRCameraRigRef FindExistingInteractionRig()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return Object.FindObjectOfType<OVRCameraRigRef>();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        internal override void OnGUI()
        {
            base.OnGUI();
        }
    }
}
