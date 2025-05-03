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

using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Linq;

namespace Oculus.Interaction.Input.Filter
{
#if !ISDK_OPENXR_HAND
    // Temporary structure used to pass data to and from native components
    [StructLayout(LayoutKind.Sequential)]

    /// <summary>
    /// Transfer struct for storing and communicating raw hand tracking data. This struct contains one full observation (i.e.,
    /// tracking frame) of raw tracking data for a single hand. Confidence information (i.e.
    /// <see cref="IHand.GetFingerIsHighConfidence(HandFinger)"/>) is not a part of this data.
    /// </summary>
    /// <remarks>
    /// Versions of this struct can be provided from a variety of sources, when available, including the Interaction SDK's OpenXR
    /// integration. This default implementation is provided for scenarios where the struct isn't defined elsewhere.
    /// </remarks>
    public struct HandData
    {
        private const int NumHandJoints = 24;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NumHandJoints * 4, ArraySubType = UnmanagedType.R4)]
        private float[] jointValues;
        private float _rootRotX;
        private float _rootRotY;
        private float _rootRotZ;
        private float _rootRotW;
        private float _rootPosX;
        private float _rootPosY;
        private float _rootPosZ;

        /// <summary>
        /// Performs first-time setup necessary before using a HandData instance. Attempting to use an instance without having
        /// initialized it will result in null accesses.
        /// </summary>
        public void Init()
        {
            jointValues = new float[NumHandJoints * 4];
        }

        /// <summary>
        /// Sets the data contained by this struct. Note that this does _not_ initialize the struct; <see cref="Init"/> must be
        /// called separately and beforehand to avoid errors.
        /// </summary>
        /// <param name="joints">The quaternions representing the tracking data meant to be stored in this struct</param>
        /// <param name="root">The pose representing the root position and orientation of the represented hand</param>
        public void SetData(Quaternion[] joints, Pose root)
        {
            Assert.AreEqual(NumHandJoints, joints.Length);
            for (int jointIndex = 0; jointIndex < NumHandJoints; jointIndex++)
            {
                Quaternion joint = joints[jointIndex];
                int jointValueIndex = jointIndex * 4;
                jointValues[jointValueIndex + 0] = joint.x;
                jointValues[jointValueIndex + 1] = joint.y;
                jointValues[jointValueIndex + 2] = joint.z;
                jointValues[jointValueIndex + 3] = joint.w;
            }
            this._rootRotX = root.rotation.x;
            this._rootRotY = root.rotation.y;
            this._rootRotZ = root.rotation.z;
            this._rootRotW = root.rotation.w;
            this._rootPosX = root.position.x;
            this._rootPosY = root.position.y;
            this._rootPosZ = root.position.z;
        }

        /// <summary>
        /// Retrieves the data contained by this struct. Note that both <see cref="Init"/> and
        /// <see cref="SetData(Quaternion[], Pose)"/> _must_ be called on this instance in order for valid data to be retrieved.
        /// </summary>
        /// <param name="joints">Output parameter; after valid invocation, this array will contain the joint rotation quaternions</param>
        /// <param name="root">Output parameter; after valid invocation, this pose will contain the root pose of the tracked hand</param>
        /// <remarks>
        /// Calling this method without having called <see cref="Init"/> will result in null access errors. Calling this method
        /// without having called <see cref="SetData(Quaternion[], Pose)"/> will yield invalid data, such as non-unit-length
        /// quaternions in <paramref name="joints"/>.
        /// </remarks>
        public void GetData(ref Quaternion[] joints, out Pose root)
        {
            Assert.AreEqual(NumHandJoints, joints.Length);
            for (int jointIndex = 0; jointIndex < NumHandJoints; jointIndex++)
            {
                int jointValueIndex = jointIndex * 4;
                joints[jointIndex].x = jointValues[jointValueIndex + 0];
                joints[jointIndex].y = jointValues[jointValueIndex + 1];
                joints[jointIndex].z = jointValues[jointValueIndex + 2];
                joints[jointIndex].w = jointValues[jointValueIndex + 3];
            }

            root = new Pose(new Vector3(_rootPosX, _rootPosY, _rootPosZ),
                new Quaternion(_rootRotX, _rootRotY, _rootRotZ, _rootRotW));
        }
    }
#endif

    /// <summary>
    /// Smoothes hand position input data. If you want to fine-tune the smoothing, it also accepts an optional set of filter parameters
    /// (<see cref="HandFilterParameterBlock"/>). Use High if your app requires a very steady hand and can tolerate some lag. High
    /// should probably only be used in controller circumstances. Use Low if only a small amount of steadiness needs to be added to the
    /// hand. For example, to do a steady raycast from a hand.
    /// </summary>
    public class HandFilter : Hand
    {
#if !ISDK_OPENXR_HAND
        #region Oculus Library Methods and Constants
        [DllImport("InteractionSdk")]
        private static extern int isdk_DataSource_Create(int id);
        [DllImport("InteractionSdk")]
        private static extern int isdk_DataSource_Destroy(int handle);
        [DllImport("InteractionSdk")]
        private static extern int isdk_DataModifier_Create(int id, int handle);
        [DllImport("InteractionSdk")]
        private static extern int isdk_DataSource_Update(int handle);
        [DllImport("InteractionSdk")]
        private static extern int isdk_DataSource_GetData(int handle, ref HandData data);
        [DllImport("InteractionSdk")]
        private static extern int isdk_ExternalHandSource_SetData(int handle, in HandData data);
        [DllImport("InteractionSdk")]
        private static extern int isdk_DataSource_SetAttributeFloat(int handle, int attrId, float value);

        enum AttributeId
        {
            Unknown = 0,
            WristPosBeta,
            WristPosMinCutOff,
            WristRotBeta,
            WristRotMinCutOff,
            FingerRotBeta,
            FingerRotMinCutOff,
            Frequency,
            WristPosDeltaCutOff,
            WristRotDeltaCutOff,
            FingerRotDeltaCutOff,
        };

        private const int _isdkExternalHandSourceId = 2;
        private const int _isdkOneEuroHandModifierId = 1;
        private const int _isdkSuccess = 0;
        #endregion Oculus Library Methods and Constants
#endif
        #region Tuneable Values
        [Header("Settings", order = -1)]
        [Tooltip("Applies a One Euro Filter when filter parameters are provided")]
        [SerializeField, Optional]
        private HandFilterParameterBlock _filterParameters = null;
        #endregion Tuneable Values

#if ISDK_OPENXR_HAND
        private readonly IOneEuroFilter<Quaternion> _rootRotFilter = OneEuroFilter.CreateQuaternion();
        private readonly IOneEuroFilter<Vector3> _rootPosFilter = OneEuroFilter.CreateVector3();
        private readonly IOneEuroFilter<Vector3>[] _jointPosFilter = new IOneEuroFilter<Vector3>[Constants.NUM_HAND_JOINTS];
        private readonly IOneEuroFilter<Quaternion>[] _jointRotFilter = new IOneEuroFilter<Quaternion>[Constants.NUM_HAND_JOINTS];
#else
        private int _dataSourceHandle = -1;
        private int _handModifierHandle = -1;
        private const string _logPrefix = "[Oculus.Interaction]";
        private bool _hasFlaggedError = false;
        private HandData _handData = new HandData();
#endif

        protected virtual void Awake()
        {
#if ISDK_OPENXR_HAND
            for (var i = 0; i < Constants.NUM_HAND_JOINTS; i++)
            {
                _jointPosFilter[i] = OneEuroFilter.CreateVector3();
                _jointRotFilter[i] = OneEuroFilter.CreateQuaternion();

            }
#else
            _handData.Init();
            _dataSourceHandle = isdk_DataSource_Create(_isdkExternalHandSourceId);
            this.AssertIsTrue(_dataSourceHandle >= 0, $"{_logPrefix} Unable to allocate external hand data source!");

            _handModifierHandle = isdk_DataModifier_Create(_isdkOneEuroHandModifierId, _dataSourceHandle);
            this.AssertIsTrue(_handModifierHandle >= 0, $"{_logPrefix} Unable to allocate one euro hand data modifier!");
#endif
        }


#if !ISDK_OPENXR_HAND
        protected virtual void OnDestroy()
        {
            int result = -1;

            //Release the filter and source
            result = isdk_DataSource_Destroy(_handModifierHandle);
            this.AssertIsTrue(_isdkSuccess == result, $"{nameof(_handModifierHandle)} destroy was unsuccessful. ");
            result = isdk_DataSource_Destroy(_dataSourceHandle);
            this.AssertIsTrue(_isdkSuccess == result, $"{nameof(_dataSourceHandle)} destroy was unsuccessful. ");
        }
#endif

        protected override void Apply(HandDataAsset handDataAsset)
        {
            base.Apply(handDataAsset);

            if (!handDataAsset.IsTracked)
            {
                return;
            }

            if (UpdateFilterParameters() && UpdateHandData(handDataAsset))
            {
                return;
            }
#if !ISDK_OPENXR_HAND

            if (_hasFlaggedError)
                return;

            _hasFlaggedError = true;
            Debug.LogError("Unable to send value to filter, InteractionSDK plugin may be missing or corrupted");
#endif
        }

        protected bool UpdateFilterParameters()
        {
            if (_filterParameters == null)
                return true;
#if ISDK_OPENXR_HAND

            _rootRotFilter.SetProperties(_filterParameters.wristRotationParameters);
            _rootPosFilter.SetProperties(_filterParameters.wristPositionParameters);
            for (var i = 0; i < Constants.NUM_HAND_JOINTS; i++)
            {
                _jointRotFilter[i].SetProperties(_filterParameters.fingerRotationParameters);
            }
#else
            int result = -1;

            // wrist position
            result = isdk_DataSource_SetAttributeFloat(
                _handModifierHandle, (int)AttributeId.WristPosBeta,
                _filterParameters.wristPositionParameters.Beta);
            if (result != _isdkSuccess)
            {
                return false;
            }

            result = isdk_DataSource_SetAttributeFloat(
                _handModifierHandle, (int)AttributeId.WristPosMinCutOff,
                _filterParameters.wristPositionParameters.MinCutoff);
            if (result != _isdkSuccess)
            {
                return false;
            }

            result = isdk_DataSource_SetAttributeFloat(
                _handModifierHandle, (int)AttributeId.WristPosDeltaCutOff,
                _filterParameters.wristPositionParameters.DCutoff);
            if (result != _isdkSuccess)
            {
                return false;
            }


            // wrist rotation
            result = isdk_DataSource_SetAttributeFloat(
                _handModifierHandle, (int)AttributeId.WristRotBeta,
                _filterParameters.wristRotationParameters.Beta);
            if (result != _isdkSuccess)
            {
                return false;
            }

            result = isdk_DataSource_SetAttributeFloat(
                _handModifierHandle, (int)AttributeId.WristRotMinCutOff,
                _filterParameters.wristRotationParameters.MinCutoff);
            if (result != _isdkSuccess)
            {
                return false;
            }

            result = isdk_DataSource_SetAttributeFloat(
                _handModifierHandle, (int)AttributeId.WristRotDeltaCutOff,
                _filterParameters.wristRotationParameters.DCutoff);
            if (result != _isdkSuccess)
            {
                return false;
            }

            // finger rotation
            result = isdk_DataSource_SetAttributeFloat(
                _handModifierHandle, (int)AttributeId.FingerRotBeta,
                _filterParameters.fingerRotationParameters.Beta);
            if (result != _isdkSuccess)
            {
                return false;
            }

            result = isdk_DataSource_SetAttributeFloat(
                _handModifierHandle, (int)AttributeId.FingerRotMinCutOff,
                _filterParameters.fingerRotationParameters.MinCutoff);
            if (result != _isdkSuccess)
            {
                return false;
            }

            result = isdk_DataSource_SetAttributeFloat(
                _handModifierHandle, (int)AttributeId.FingerRotDeltaCutOff,
                _filterParameters.fingerRotationParameters.DCutoff);
            if (result != _isdkSuccess)
            {
                return false;
            }

            // frequency
            result = isdk_DataSource_SetAttributeFloat(
                _handModifierHandle, (int)AttributeId.Frequency,
                _filterParameters.frequency);
            if (result != _isdkSuccess)
            {
                return false;
            }
#endif
            return true;
        }

        private ShadowHand _shadowHand = new ShadowHand();

        protected bool UpdateHandData(HandDataAsset handDataAsset)
        {
            // null parameters implies don't filter
            if (_filterParameters == null)
                return true;
#if ISDK_OPENXR_HAND
            var dt = 1.0f / _filterParameters.frequency;

            var pose = handDataAsset.Root;

            _shadowHand.FromJoints(handDataAsset.JointPoses.ToList(), false);

            handDataAsset.Root = new Pose(
                _rootPosFilter.Step(pose.position, dt),
                _rootRotFilter.Step(pose.rotation, dt)
                );

            // Update rotations to filtered values
            for (var i = 0; i < Constants.NUM_HAND_JOINTS; i++)
            {
                var jointId = (HandJointId)i;
                var jointPose = _shadowHand.GetLocalPose(jointId);
                var filterRot = _jointRotFilter[i].Step(
                    jointPose.rotation, dt
                );
                jointPose.rotation = filterRot;
                _shadowHand.SetLocalPose(jointId, jointPose);
            }

            handDataAsset.JointPoses = _shadowHand.GetWorldPoses();

            // Legacy local rotations
            for (int i = 0; i < Constants.NUM_HAND_JOINTS; i++)
            {
                int parent = (int)HandJointUtils.JointParentList[i];
#pragma warning disable 0618
                handDataAsset.Joints[i] = parent < 0 ? Quaternion.identity :
                    Quaternion.Inverse(handDataAsset.JointPoses[parent].rotation) *
                    handDataAsset.JointPoses[i].rotation;
#pragma warning restore 0618
            }
#else
            // pipe data asset into temp struct
            _handData.SetData(handDataAsset.Joints, handDataAsset.Root);

            // Send it
            int result = isdk_ExternalHandSource_SetData(_dataSourceHandle, _handData);
            if (result != _isdkSuccess)
            {
                return false;
            }

            // Update
            result = isdk_DataSource_Update(_handModifierHandle);
            if (result != _isdkSuccess)
            {
                return false;
            }

            // Get result
            result = isdk_DataSource_GetData(_handModifierHandle, ref _handData);
            if (result != _isdkSuccess)
            {
                return false;
            }

            // Copy results into our hand data asset
            _handData.GetData(ref handDataAsset.Joints, out handDataAsset.Root);
#endif
            return true;
        }
    }
}
