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
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
    /// <summary>
    /// Define how the constraints are checked, by value or boolean
    /// </summary>
    [Serializable]
    public enum ConstraintModeCheck
    {
        Value,
        Bool
    }

    /// <summary>
    /// A struct that contains all the information for a constraint
    /// </summary>
    [Serializable]
    public struct Constraint
    {
        [SerializeField]
        public string name; /// <summary> The name of the constraint </summary>

        [SerializeField]
        public bool enabled; /// <summary> Whether the constraint is enabled or not </summary>

        [SerializeField]
        public Mask mask; /// <summary> The mask to use for the constraint </summary>

        [SerializeField] public ConstraintModeCheck modeCheck; /// <summary> The mode to use for the constraint </summary>

        [SerializeField] public float min; /// <summary> The minimum value for the constraint </summary>
        [SerializeField] public float max; /// <summary> The maximum value for the constraint </summary>
    }

    /// <summary>
    /// A struct that contains all the information needed to spawn a decoration
    /// </summary>
    public struct Candidate
    {
        public GameObject decorationPrefab; /// <summary> The prefab to spawn </summary>
        public Vector2 localPos;  /// <summary>The position of the object in local space </summary>
        public Vector2 localPosNormalized;  /// <summary> The position of the object in local space normalized </summary>
        public RaycastHit hit; /// <summary>The raycast hit </summary>
        public Vector3 anchorCompDists; /// <summary> The distance to the anchor with the given component </summary>
        public float anchorDist; /// <summary> The distance to the anchor </summary>
        public float slope; /// <summary>The slope of the surface </summary>
    }

    /// <summary>
    /// Which axis  to use
    /// </summary>
    [Flags]
    public enum Axes
    {
        X = 0x1, /// <summary>The X axis </summary>
        Y = 0x2, /// <summary>The Y axis </summary>
        Z = 0x4 /// <summary>The Z axis </summary>
    }

    /// <summary>
    /// Which distribution to use
    /// </summary>
    public enum DistributionType
    {
        GRID = 0x0, //The objects will be spawned in a grid pattern
        SIMPLEX = 0x1, //The objects will be spawned in a simplex pattern
        STAGGERED_CONCENTRIC = 0x2, //The objects will be spawned in a staggered concentric pattern
        RANDOM = 0x3 //The objects will be spawned in a random pattern
    }

    /// <summary>
    /// Which placement to use
    /// </summary>
    public enum Placement
    {
        LOCAL_PLANAR, /// <summary> The object will be spawned in the local space </summary>
        WORLD_PLANAR, /// <summary> The object will be spawned in the world space </summary>
        SPHERICAL /// <summary> The object will be spawned in the world space </summary>
    }

    /// <summary>
    /// Which hierarchy to use for the spawned object
    /// </summary>
    public enum SpawnHierarchy
    {
        ROOT, /// <summary> The object will be spawned in the root of the scene </summary>
        SCENE_DECORATOR_CHILD, /// <summary> The object will be spawned as a child of the scene decorator </summary>
        ANCHOR_CHILD, /// <summary> The object will be spawned as a child of the anchor </summary>
        TARGET_CHILD, /// <summary> The object will be spawned as a child of the target </summary>
        TARGET_COLLIDER_CHILD /// <summary> The object will be spawned as a child of the target collider </summary>
    }

    /// <summary>
    /// Which target to use for the spawned object
    /// </summary>
    [Flags]
    public enum Target
    {
        GLOBAL_MESH = 1 << 0, /// <summary> The object will be spawned on a hit on the global mesh </summary>
        RESERVED_MESH = 1 << 1, /// <summary> Not suppoerted yet, same as GLOBAL_MESH </summary>
        PHYSICS_LAYERS = 1 << 2, /// <summary> All physics layers defined will help determine the position </summary>
        CUSTOM_COLLIDERS = 1 << 3, /// <summary> Use a custom collider to determine the position </summary>
        CUSTOM_TAGS = 1 << 4, /// <summary> use only gameobjects with custom tag, needs a collider </summary>
        SCENE_ANCHORS = 1 << 5 /// <summary> use MRUK anchor Raycast, no colliders required </summary>
    }
}
