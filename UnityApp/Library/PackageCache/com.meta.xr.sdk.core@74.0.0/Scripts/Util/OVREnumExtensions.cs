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

public static class OVREnumExtensions
{
    public static bool IsHand(this OVRSkeleton.SkeletonType skeletonType)
    {
        if (skeletonType == OVRSkeleton.SkeletonType.HandLeft || skeletonType == OVRSkeleton.SkeletonType.HandRight)
        {
            return true;
        }
        if (skeletonType == OVRSkeleton.SkeletonType.XRHandLeft || skeletonType == OVRSkeleton.SkeletonType.XRHandRight)
        {
            return true;
        }
        return false;
    }

    public static bool IsOpenXRHandSkeleton(this OVRSkeleton.SkeletonType skeletonType)
    {
        return skeletonType == OVRSkeleton.SkeletonType.XRHandLeft ||
               skeletonType == OVRSkeleton.SkeletonType.XRHandRight;
    }

    public static bool IsOVRHandSkeleton(this OVRSkeleton.SkeletonType skeletonType)
    {
        return skeletonType == OVRSkeleton.SkeletonType.HandLeft ||
               skeletonType == OVRSkeleton.SkeletonType.HandRight;
    }

    public static bool IsLeft(this OVRSkeleton.SkeletonType type)
    {
        if (type == OVRSkeleton.SkeletonType.HandLeft)
        {
            return true;
        }
        if (type == OVRSkeleton.SkeletonType.XRHandLeft)
        {
            return true;
        }
        return false;
    }

    public static OVRHand.Hand AsHandType(this OVRSkeleton.SkeletonType skeletonType)
    {
        switch (skeletonType)
        {
            case OVRSkeleton.SkeletonType.HandLeft:
            case OVRSkeleton.SkeletonType.XRHandLeft:
                return OVRHand.Hand.HandLeft;
            case OVRSkeleton.SkeletonType.HandRight:
            case OVRSkeleton.SkeletonType.XRHandRight:
                return OVRHand.Hand.HandRight;
            default:
                return OVRHand.Hand.None;
        }
    }


    [Obsolete("Use the overload which takes an " + nameof(OVRHandSkeletonVersion) + "instead.")]
    public static OVRSkeleton.SkeletonType AsSkeletonType(this OVRHand.Hand hand)
    {
        switch (hand)
        {
            case OVRHand.Hand.HandLeft:
                return OVRSkeleton.SkeletonType.HandLeft;
            case OVRHand.Hand.HandRight:
                return OVRSkeleton.SkeletonType.HandRight;
            default:
                return OVRSkeleton.SkeletonType.None;
        }
    }

    public static OVRSkeleton.SkeletonType AsSkeletonType(this OVRHand.Hand hand,
        OVRHandSkeletonVersion version)
    {
        switch (hand)
        {
            case OVRHand.Hand.HandLeft:
                return version == OVRHandSkeletonVersion.OVR ?
                    OVRSkeleton.SkeletonType.HandLeft : OVRSkeleton.SkeletonType.XRHandLeft;
            case OVRHand.Hand.HandRight:
                return version == OVRHandSkeletonVersion.OVR ?
                    OVRSkeleton.SkeletonType.HandRight : OVRSkeleton.SkeletonType.XRHandRight;
            default:
                return OVRSkeleton.SkeletonType.None;
        }
    }

    [Obsolete("Use the overload which takes an " + nameof(OVRHandSkeletonVersion) + "instead.")]
    public static OVRMesh.MeshType AsMeshType(this OVRHand.Hand hand)
    {
        switch (hand)
        {
            case OVRHand.Hand.HandLeft:
                return OVRMesh.MeshType.HandLeft;
            case OVRHand.Hand.HandRight:
                return OVRMesh.MeshType.HandRight;
            default:
                return OVRMesh.MeshType.None;
        }
    }


    public static bool IsOpenXRHandMesh(this OVRMesh.MeshType meshType)
    {
        return meshType == OVRMesh.MeshType.XRHandLeft ||
               meshType == OVRMesh.MeshType.XRHandRight;
    }

    public static bool IsOVRHandMesh(this OVRMesh.MeshType meshType)
    {
        return meshType == OVRMesh.MeshType.HandLeft ||
               meshType == OVRMesh.MeshType.HandRight;
    }

    public static OVRMesh.MeshType AsMeshType(this OVRHand.Hand hand,
    OVRHandSkeletonVersion version)
    {
        switch (hand)
        {
            case OVRHand.Hand.HandLeft:
                return version == OVRHandSkeletonVersion.OVR ?
                    OVRMesh.MeshType.HandLeft : OVRMesh.MeshType.XRHandLeft;
            case OVRHand.Hand.HandRight:
                return version == OVRHandSkeletonVersion.OVR ?
                    OVRMesh.MeshType.HandRight : OVRMesh.MeshType.XRHandRight;
            default:
                return OVRMesh.MeshType.None;
        }
    }

    public static bool IsLeft(this OVRMesh.MeshType type)
    {
        if (type == OVRMesh.MeshType.HandLeft)
        {
            return true;
        }

        if (type == OVRMesh.MeshType.XRHandLeft)
        {
            return true;
        }

        return false;
    }

    public static bool IsHand(this OVRMesh.MeshType meshType)
    {
        if (meshType == OVRMesh.MeshType.HandLeft || meshType == OVRMesh.MeshType.HandRight)
        {
            return true;
        }
        if (meshType == OVRMesh.MeshType.XRHandLeft || meshType == OVRMesh.MeshType.XRHandRight)
        {
            return true;
        }

        return false;
    }

    public static OVRHand.Hand AsHandType(this OVRMesh.MeshType meshType)
    {
        switch (meshType)
        {
            case OVRMesh.MeshType.HandLeft:
            case OVRMesh.MeshType.XRHandLeft:
                return OVRHand.Hand.HandLeft;
            case OVRMesh.MeshType.HandRight:
            case OVRMesh.MeshType.XRHandRight:
                return OVRHand.Hand.HandRight;
            default:
                return OVRHand.Hand.None;
        }
    }
}
