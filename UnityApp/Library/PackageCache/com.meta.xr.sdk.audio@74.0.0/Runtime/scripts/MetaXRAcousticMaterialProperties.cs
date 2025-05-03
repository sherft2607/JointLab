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

/************************************************************************************
 * Filename    :   MetaXRAcousticMaterialProperties.cs
 * Content     :   Acoustic material properties object stores the acoustic properties
 ***********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Meta.XR.Acoustics
{
    //***********************************************************************
    // Public Fields
    /// \brief This class defines all the properties which describe an Acoustic Material
    ///
    /// The properties of a material include absorption, transmission, and scattering.
    ///
    /// \see MaterialProperty
    [Serializable]
    public class MaterialData
    {
        /// \brief  The fraction of sound arriving at a surface that is absorbed by the material.
        /// This controls how long it takes for the reverb to decay, with higher absorption leading to shorter reverberation times.
        /// This is the Sabine absorption coefficient, which is the absorption averaged over all angles of incidence.
        /// The absorption coefficient is the opposite of the reflection coefficient (1 - absorption). The default absorption is 0.1.
        [SerializeField]
        internal Spectrum absorption = new Spectrum();
        /// \brief The fraction of sound arriving at a surface that is transmitted through the material.
        /// This value is in the range 0 to 1, where 0 indicates a material that is acoustically opaque, and 1 indicates a material that is acoustically transparent.
        /// To preserve energy in the simulation, the following condition must hold: (1 - absorption + transmission) <= 1.
        ///  If this condition is not met, the transmission and absorption coefficients will be modified to enforce energy conservation.
        /// The default transmission is 0. Note that increasing the transmission coefficient has the effect of reducing the reverberation time because it allows some of the sound to escape the geometry.
        [SerializeField]
        internal Spectrum transmission = new Spectrum();
        /// \brief The fraction of sound arriving at a surface that is scattered. The scattering coefficient describes how rough or smooth the surface is for sound of a given frequency.
        /// A value of 0 indicates a perfectly mirror-like (specular) reflection, while a value of 1 indicates a perfectly diffuse/matte reflection. The default value is 0.5.
        /// The impact of the scattering coefficient on the audio is subtle, and it primarily affects the early reflections.
        [SerializeField]
        internal Spectrum scattering = new Spectrum();
        [SerializeField]
        internal Color color = Color.yellow;

        internal void Clone(MaterialData other)
        {
            color = other.color;
            absorption.Clone(other.absorption);
            transmission.Clone(other.transmission);
            scattering.Clone(other.scattering);
        }

        internal bool IsEmpty => absorption.points.Count == 0 && transmission.points.Count == 0 && scattering.points.Count == 0;
    }

    internal interface IMaterialDataProvider
    {
        Meta.XR.Acoustics.MaterialData Data { get; }
        string name { get; }
    }


    [Serializable]
    internal sealed class Spectrum : IEnumerable<Spectrum.Point>
    {
        [Serializable]
        internal struct Point : IComparable<Point>
        {
            [SerializeField]
            internal float frequency;
            [SerializeField]
            internal float data;

            internal Point(float frequency = 0, float data = 0)
            {
                this.frequency = frequency;
                this.data = data;
            }
            public int CompareTo(Point other)
            {
                return frequency.CompareTo(other.frequency);
            }

            public static implicit operator Point(Vector2 v)
            {
                return new Point(v.x, v.y);
            }

            public static implicit operator Vector2(Point point)
            {
                return new Vector2(point.frequency, point.data);
            }

            public override string ToString()
            {
                return $"({frequency}Hz, {data:0.00})";
            }
        }

        [SerializeField]
        internal int selection = int.MaxValue;
        [SerializeField]
        internal List<Point> points = new List<Point>();

        IEnumerator<Point> IEnumerable<Point>.GetEnumerator() => points.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => points.GetEnumerator();

        internal void Add(float frequency, float data) => points.Add(new Point(frequency, data));
        internal Spectrum(Spectrum other = null)
        {
            if (other != null)
                Clone(other);
        }

        internal void Clone(Spectrum other)
        {
            if (this == other)
                return;

            selection = other.selection;
            points = new List<Point>(other.points);
        }

        internal void Sort()
        {
            if (points.Count != 0)
            {
                Point selectedPoint = points[selection];
                points.Sort();
                selection = points.IndexOf(selectedPoint);
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (Point p in points)
                s.Append($"[{p.frequency}, {p.data}] ");

            return s.ToString();
        }

        internal float this[float f]
        {
            get
            {
                if (points.Count > 0)
                {
                    Point lower = new Point(float.MinValue);
                    Point upper = new Point(float.MaxValue);

                    foreach (Point point in points)
                    {
                        if (point.frequency < f)
                        {
                            if (point.frequency > lower.frequency)
                                lower = point;
                        }
                        else
                        {
                            if (point.frequency < upper.frequency)
                                upper = point;
                        }
                    }

                    if (lower.frequency == float.MinValue)
                        lower.data = points.OrderBy(p => p.frequency).First().data;
                    if (upper.frequency == float.MaxValue)
                        upper.data = points.OrderBy(p => p.frequency).Last().data;

                    return Mathf.Lerp(lower.data, upper.data, (f - lower.frequency) / (upper.frequency - lower.frequency));
                }

                return 0f;
            }
        }
    }
}

[CreateAssetMenu(menuName = "MetaXRAudio/Acoustic Material Properties")]
internal class MetaXRAcousticMaterialProperties : ScriptableObject, Meta.XR.Acoustics.IMaterialDataProvider
{
    internal enum BuiltinPreset
    {
        Custom,
        AcousticTile,
        Brick,
        BrickPainted,
        Cardboard,
        Carpet,
        CarpetHeavy,
        CarpetHeavyPadded,
        CeramicTile,
        Concrete,
        ConcreteRough,
        ConcreteBlock,
        ConcreteBlockPainted,
        Curtain,
        Foliage,
        Glass,
        GlassHeavy,
        Grass,
        Gravel,
        GypsumBoard,
        Marble,
        Mud,
        PlasterOnBrick,
        PlasterOnConcreteBlock,
        Rubber,
        Soil,
        SoundProof,
        Snow,
        Steel,
        Stone,
        Vent,
        Water,
        WoodThin,
        WoodThick,
        WoodFloor,
        WoodOnConcrete,
        MetaDefault
    }


    [SerializeField]
    private Meta.XR.Acoustics.MaterialData data = new Meta.XR.Acoustics.MaterialData();
    public Meta.XR.Acoustics.MaterialData Data => data;

    [SerializeField]
    private BuiltinPreset preset = BuiltinPreset.Custom;
    internal BuiltinPreset Preset
    {
        get => preset;
        set
        {
            if (value != BuiltinPreset.Custom)
            {
                SetPreset(value, ref data);
            }
            preset = value;
        }
    }

    //***********************************************************************

    internal static void SetPreset(BuiltinPreset builtinPreset, ref Meta.XR.Acoustics.MaterialData data)
    {
        switch (builtinPreset)
        {
            case BuiltinPreset.AcousticTile: AcousticTile(ref data); break;
            case BuiltinPreset.Brick: Brick(ref data); break;
            case BuiltinPreset.BrickPainted: BrickPainted(ref data); break;
            case BuiltinPreset.Cardboard: Cardboard(ref data); break;
            case BuiltinPreset.Carpet: Carpet(ref data); break;
            case BuiltinPreset.CarpetHeavy: CarpetHeavy(ref data); break;
            case BuiltinPreset.CarpetHeavyPadded: CarpetHeavyPadded(ref data); break;
            case BuiltinPreset.CeramicTile: CeramicTile(ref data); break;
            case BuiltinPreset.Concrete: Concrete(ref data); break;
            case BuiltinPreset.ConcreteRough: ConcreteRough(ref data); break;
            case BuiltinPreset.ConcreteBlock: ConcreteBlock(ref data); break;
            case BuiltinPreset.ConcreteBlockPainted: ConcreteBlockPainted(ref data); break;
            case BuiltinPreset.Curtain: Curtain(ref data); break;
            case BuiltinPreset.Foliage: Foliage(ref data); break;
            case BuiltinPreset.Glass: Glass(ref data); break;
            case BuiltinPreset.GlassHeavy: GlassHeavy(ref data); break;
            case BuiltinPreset.Grass: Grass(ref data); break;
            case BuiltinPreset.Gravel: Gravel(ref data); break;
            case BuiltinPreset.GypsumBoard: GypsumBoard(ref data); break;
            case BuiltinPreset.Marble: Marble(ref data); break;
            case BuiltinPreset.Mud: Mud(ref data); break;
            case BuiltinPreset.PlasterOnBrick: PlasterOnBrick(ref data); break;
            case BuiltinPreset.PlasterOnConcreteBlock: PlasterOnConcreteBlock(ref data); break;
            case BuiltinPreset.Rubber: Rubber(ref data); break;
            case BuiltinPreset.Soil: Soil(ref data); break;
            case BuiltinPreset.SoundProof: SoundProof(ref data); break;
            case BuiltinPreset.Snow: Snow(ref data); break;
            case BuiltinPreset.Steel: Steel(ref data); break;
            case BuiltinPreset.Stone: Stone(ref data); break;
            case BuiltinPreset.Vent: Vent(ref data); break;
            case BuiltinPreset.Water: Water(ref data); break;
            case BuiltinPreset.WoodThin: WoodThin(ref data); break;
            case BuiltinPreset.WoodThick: WoodThick(ref data); break;
            case BuiltinPreset.WoodFloor: WoodFloor(ref data); break;
            case BuiltinPreset.WoodOnConcrete: WoodOnConcrete(ref data); break;
            case BuiltinPreset.MetaDefault: MetaDefault(ref data); break;
            case BuiltinPreset.Custom:
            default:
                Debug.LogError("no preset specified");
                break;
        }
    }

    //***********************************************************************

    private static void AcousticTile(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            { 125f, 0.50f }, {250f, 0.70f }, {500f, 0.60f }, {1000f, 0.70f }, { 2000f, 0.70f }, { 4000f, 0.50f } };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.15f}, {500f, 0.20f}, {1000f, 0.20f}, {2000f, 0.25f}, {4000f, 0.30f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.05f}, {250f, 0.04f}, {500f, 0.03f}, {1000f, 0.02f}, {2000f, 0.005f}, {4000f, 0.002f} };
    }

    private static void Brick(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.02f}, {250f, 0.02f}, {500f, 0.03f}, {1000f, 0.04f}, {2000f, 0.05f}, {4000f, 0.07f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.20f}, {250f, 0.25f}, {500f, 0.30f}, {1000f, 0.35f}, {2000f, 0.40f}, {4000f, 0.45f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.025f}, {250f, 0.019f}, {500f, 0.01f}, {1000f, 0.0045f}, {2000f, 0.0018f}, {4000f, 0.00089f} };
    }

    private static void BrickPainted(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.01f}, {250f, 0.01f },  {500f, 0.02f}, {1000f, 0.02f}, {2000f, 0.02f}, {4000f, 0.03f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.15f}, {250f, 0.15f}, {500f, 0.20f}, {1000f, 0.20f}, {2000f, 0.20f}, {4000f, 0.25f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.025f}, {250f, 0.019f}, {500f, 0.01f}, {1000f, 0.0045f}, {2000f, 0.0018f}, {4000f, 0.00089f} };
    }

    private static void Cardboard(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {400f, 0.41f}, {500f, 0.607f}, {630f, 0.773f}, {800f, 0.669f}, {1000f, 0.685f}, {1250f, 0.806f}, {1600f, 0.579f}, {2000f, 0.792f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.12f}, {500f, 0.14f}, {1000f, 0.16f}, {2000f, 0.18f}, {4000f, 0.20f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {400f, 0.082f}, {500f, 0.121f}, {630f, 0.155f}, {800f, 0.134f}, {1000f, 0.137f}, {1250f, 0.161f}, {1600f, 0.116f}, {2000f, 0.158f} };
    }

    private static void Carpet(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.01f}, {250f, 0.05f}, {500f, 0.10f}, {1000f, 0.20f}, {2000f, 0.45f}, {4000f, 0.65f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.10f}, {500f, 0.15f}, {1000f, 0.20f}, {2000f, 0.30f}, {4000f, 0.45f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.004f}, {250f, 0.0079f}, {500f, 0.0056f}, {1000f, 0.0016f}, {2000f, 0.0014f}, {4000f, 0.0005f} };
    }

    private static void CarpetHeavy(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.02f}, {250f, 0.06f}, {500f, 0.14f}, {1000f, 0.37f}, {2000f, 0.48f}, {4000f, 0.63f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.15f}, {500f, 0.20f}, {1000f, 0.25f}, {2000f, 0.35f },  {4000f, 0.50f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.004f}, {250f, 0.0079f}, {500f, 0.0056f}, {1000f, 0.0016f}, {2000f, 0.0014f}, {4000f, 0.0005f} };
    }

    private static void CarpetHeavyPadded(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.08f}, {250f, 0.24f}, {500f, 0.57f}, {1000f, 0.69f}, {2000f, 0.71f}, {4000f, 0.73f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.15f}, {500f, 0.20f}, {1000f, 0.25f}, {2000f, 0.35f}, {4000f, 0.50f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.004f}, {250f, 0.0079f}, {500f, 0.0056f}, {1000f, 0.0016f}, {2000f, 0.0014f}, {4000f, 0.0005f} };
    }

    private static void CeramicTile(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.01f}, {250f, 0.01f}, {500f, 0.01f}, {1000f, 0.01f}, {2000f, 0.02f}, {4000f, 0.02f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.12f}, {500f, 0.14f}, {1000f, 0.16f}, {2000f, 0.18f}, {4000f, 0.20f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.004f}, {250f, 0.0079f}, {500f, 0.0056f}, {1000f, 0.0016f}, {2000f, 0.0014f}, {4000f, 0.0005f} };
    }

    private static void Concrete(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.01f}, {250f, 0.01f}, {500f, 0.02f}, {1000f, 0.02f}, {2000f, 0.02f}, {4000f, 0.02f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.11f}, {500f, 0.12f}, {1000f, 0.13f}, {2000f, 0.14f}, {4000f, 0.15f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.004f}, {250f, 0.0079f}, {500f, 0.0056f}, {1000f, 0.0016f}, {2000f, 0.0014f}, {4000f, 0.0005f} };
    }

    private static void ConcreteRough(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.01f}, {250f, 0.02f}, {500f, 0.04f}, {1000f, 0.06f}, {2000f, 0.08f}, {4000f, 0.10f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.12f}, {500f, 0.15f}, {1000f, 0.20f}, {2000f, 0.25f}, {4000f, 0.30f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.004f}, {250f, 0.0079f}, {500f, 0.0056f}, {1000f, 0.0016f}, {2000f, 0.0014f}, {4000f, 0.0005f} };
    }

    private static void ConcreteBlock(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.36f}, {250f, 0.44f}, {500f, 0.31f}, {1000f, 0.29f}, {2000f, 0.39f}, {4000f, 0.21f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.12f}, {500f, 0.15f}, {1000f, 0.20f}, {2000f, 0.30f}, {4000f, 0.40f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.02f}, {250f, 0.01f}, {500f, 0.0063f}, {1000f, 0.0035f}, {2000f, 0.00011f}, {4000f, 0.00063f} };
    }

    private static void ConcreteBlockPainted(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.05f}, {500f, 0.06f}, {1000f, 0.07f}, {2000f, 0.09f}, {4000f, 0.08f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.11f}, {500f, 0.13f}, {1000f, 0.15f}, {2000f, 0.16f}, {4000f, 0.20f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.02f}, {250f, 0.01f}, {500f, 0.0063f}, {1000f, 0.0035f}, {2000f, 0.00011f}, {4000f, 0.00063f} };
    }

    private static void Curtain(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.07f}, {250f, 0.31f}, {500f, 0.49f}, {1000f, 0.75f}, {2000f, 0.70f}, {4000f, 0.60f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.15f}, {500f, 0.2f}, {1000f, 0.3f}, {2000f, 0.4f}, {4000f, 0.5f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.42f}, {250f, 0.39f}, {500f, 0.21f}, {1000f, 0.14f}, {2000f, 0.079f}, {4000f, 0.045f} };
    }

    private static void Foliage(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.03f}, {250f, 0.06f}, {500f, 0.11f}, {1000f, 0.17f}, {2000f, 0.27f}, {4000f, 0.31f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.20f}, {250f, 0.3f}, {500f, 0.4f}, {1000f, 0.5f}, {2000f, 0.7f}, {4000f, 0.8f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.9f}, {250f, 0.9f}, {500f, 0.9f}, {1000f, 0.8f}, {2000f, 0.5f}, {4000f, 0.3f} };
    }

    private static void Glass(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.35f}, {250f, 0.25f}, {500f, 0.18f}, {1000f, 0.12f}, {2000f, 0.07f}, {4000f, 0.05f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.05f}, {250f, 0.05f}, {500f, 0.05f}, {1000f, 0.05f}, {2000f, 0.05f}, {4000f, 0.05f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.125f}, {250f, 0.089f}, {500f, 0.05f}, {1000f, 0.028f}, {2000f, 0.022f}, {4000f, 0.079f} };
    }

    private static void GlassHeavy(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.18f },  {250f, 0.06f}, {500f, 0.04f },  {1000f, 0.03f}, {2000f, 0.02f}, {4000f, 0.02f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.05f}, {250f, 0.05f}, {500f, 0.05f}, {1000f, 0.05f}, {2000f, 0.05f}, {4000f, 0.05f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.056f}, {250f, 0.039f}, {500f, 0.028f}, {1000f, 0.02f}, {2000f, 0.032f}, {4000f, 0.014f} };
    }

    private static void Grass(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.11f}, {250f, 0.26f}, {500f, 0.60f}, {1000f, 0.69f}, {2000f, 0.92f}, {4000f, 0.99f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.30f}, {250f, 0.30f}, {500f, 0.40f}, {1000f, 0.50f}, {2000f, 0.60f}, {4000f, 0.70f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum();
    }

    private static void Gravel(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.25f}, {250f, 0.60f}, {500f, 0.65f}, {1000f, 0.70f}, {2000f, 0.75f}, {4000f, 0.80f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.20f}, {250f, 0.30f}, {500f, 0.40f}, {1000f, 0.50f}, {2000f, 0.60f}, {4000f, 0.70f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum();
    }

    private static void GypsumBoard(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.29f}, {250f, 0.10f}, {500f, 0.05f}, {1000f, 0.04f}, {2000f, 0.07f}, {4000f, 0.09f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.11f}, {500f, 0.12f}, {1000f, 0.13f}, {2000f, 0.14f}, {4000f, 0.15f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.035f}, {250f, 0.0125f}, {500f, 0.0056f}, {1000f, 0.0025f}, {2000f, 0.0013f}, {4000f, 0.0032f} };
    }

    private static void Marble(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.01f}, {250f, 0.01f}, {500f, 0.01f}, {1000f, 0.01f}, {2000f, 0.02f}, {4000f, 0.02f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.10f}, {500f, 0.10f}, {1000f, 0.10f}, {2000f, 0.10f}, {4000f, 0.10f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.004f}, {250f, 0.0079f}, {500f, 0.0056f}, {1000f, 0.0016f}, {2000f, 0.0014f}, {4000f, 0.0005f} };
    }

    private static void Mud(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.15f}, {250f, 0.25f}, {500f, 0.30f}, {1000f, 0.25f}, {2000f, 0.20f}, {4000f, 0.15f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.20f}, {500f, 0.25f}, {1000f, 0.40f}, {2000f, 0.55f}, {4000f, 0.70f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum();
    }

    private static void PlasterOnBrick(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.01f}, {250f, 0.02f}, {500f, 0.02f}, {1000f, 0.03f}, {2000f, 0.04f}, {4000f, 0.05f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.20f}, {250f, 0.25f}, {500f, 0.30f}, {1000f, 0.35f}, {2000f, 0.40f}, {4000f, 0.45f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.025f}, {250f, 0.019f}, {500f, 0.01f}, {1000f, 0.0045f}, {2000f, 0.0018f}, {4000f, 0.00089f} };
    }

    private static void PlasterOnConcreteBlock(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.12f}, {250f, 0.09f}, {500f, 0.07f}, {1000f, 0.05f}, {2000f, 0.05f}, {4000f, 0.04f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.20f}, {250f, 0.25f}, {500f, 0.30f}, {1000f, 0.35f}, {2000f, 0.40f },  {4000f, 0.45f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.02f}, {250f, 0.01f}, {500f, 0.0063f}, {1000f, 0.0035f}, {2000f, 0.00011f}, {4000f, 0.00063f} };
    }

    private static void Rubber(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.05f}, {250f, 0.05f}, {500f, 0.1f}, {1000f, 0.1f}, {2000f, 0.05f}, {4000f, 0.05f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.10f}, {500f, 0.10f}, {1000f, 0.10f}, {2000f, 0.15f },  {4000f, 0.20f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.01f}, {250f, 0.01f}, {500f, 0.02f}, {1000f, 0.02f}, {2000f, 0.01f}, {4000f, 0.01f} };
    }

    private static void Soil(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.15f}, {250f, 0.25f}, {500f, 0.40f}, {1000f, 0.55f}, {2000f, 0.60f}, {4000f, 0.60f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.20f}, {500f, 0.25f}, {1000f, 0.40f}, {2000f, 0.55f}, {4000f, 0.70f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum();
    }

    private static void SoundProof(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum { { 1000f, 1.0f } };
        data.scattering = new Meta.XR.Acoustics.Spectrum { { 1000f, 0.0f } };
        data.transmission = new Meta.XR.Acoustics.Spectrum();
    }

    private static void Snow(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.45f}, {250f, 0.75f}, {500f, 0.90f}, {1000f, 0.95f}, {2000f, 0.95f}, {4000f, 0.95f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.20f}, {250f, 0.30f}, {500f, 0.40f}, {1000f, 0.50f}, {2000f, 0.60f}, {4000f, 0.75f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum();
    }

    private static void Steel(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.05f}, {250f, 0.10f}, {500f, 0.10f}, {1000f, 0.10f}, {2000f, 0.07f}, {4000f, 0.02f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.10f}, {500f, 0.10f}, {1000f, 0.10f}, {2000f, 0.10f}, {4000f, 0.10f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.25f}, {250f, 0.2f}, {500f, 0.17f}, {1000f, 0.089f}, {2000f, 0.089f}, {4000f, 0.0056f} };
    }

    private static void Stone(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.02f}, {500f, 0.02f}, {2000f, 0.05f}, {4000f, 0.05f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.10f}, {500f, 0.15f}, {1000f, 0.20f}, {2000f, 0.25f}, {4000f, 0.30f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.004f}, {250f, 0.0079f}, {500f, 0.0056f}, {1000f, 0.00016f}, {2000f, 0.0014f}, {4000f, 0.0005f} };
    }

    private static void Vent(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {63.5f, 0.15f}, {125f, 0.15f}, {250f, 0.20f}, {500f, 0.50f}, {1000f, 0.35f}, {2000f, 0.30f}, {4000f, 0.20f}, {8000f, 0.20f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {63.5f, 0.10f}, {125f, 0.10f}, {250f, 0.10f}, {500f, 0.15f}, {1000f, 0.30f}, {2000f, 0.40f}, {4000f, 0.50f}, {8000f, 0.50f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {63.5f, 0.135f}, {125f, 0.135f}, {250f, 0.18f}, {500f, 0.45f}, {1000f, 0.315f}, {2000f, 0.27f}, {4000f, 0.18f}, {8000f, 0.18f} };
    }

    private static void Water(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.01f}, {250f, 0.01f}, {500f, 0.01f}, {1000f, 0.02f}, {2000f, 0.02f}, {4000f, 0.03f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.10f}, {500f, 0.10f}, {1000f, 0.07f}, {2000f, 0.05f}, {4000f, 0.05f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.03f}, {250f, 0.03f}, {500f, 0.03f}, {1000f, 0.02f}, {2000f, 0.015f}, {4000f, 0.01f} };
    }

    private static void WoodThin(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.42f}, {250f, 0.21f}, {500f, 0.10f}, {1000f, 0.08f}, {2000f, 0.06f}, {4000f, 0.06f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.10f}, {500f, 0.10f}, {1000f, 0.10f}, {2000f, 0.10f}, {4000f, 0.15f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.2f}, {250f, 0.125f}, {500f, 0.079f}, {1000f, 0.1f}, {2000f, 0.089f}, {4000f, 0.05f} };
    }

    private static void WoodThick(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.19f}, {250f, 0.14f}, {500f, 0.09f}, {1000f, 0.06f}, {2000f, 0.06f}, {4000f, 0.05f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.10f}, {500f, 0.10f}, {1000f, 0.10f}, {2000f, 0.10f}, {4000f, 0.15f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.035f}, {250f, 0.028f}, {500f, 0.028f}, {1000f, 0.028f}, {2000f, 0.011f}, {4000f, 0.0071f} };
    }

    private static void WoodFloor(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.15f}, {250f, 0.11f}, {500f, 0.10f}, {1000f, 0.07f}, {2000f, 0.06f}, {4000f, 0.07f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.10f}, {500f, 0.10f}, {1000f, 0.10f}, {2000f, 0.10f}, {4000f, 0.15f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.071f}, {250f, 0.025f}, {500f, 0.0158f}, {1000f, 0.0056f}, {2000f, 0.0035f}, {4000f, 0.0016f} };
    }

    private static void WoodOnConcrete(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.04f },  {250f, 0.04f}, {500f, 0.07f}, {1000f, 0.06f },  {2000f, 0.06f}, {4000f, 0.07f} };

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.10f}, {250f, 0.10f}, {500f, 0.10f}, {1000f, 0.10f}, {2000f, 0.10f}, {4000f, 0.15f} };

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {125f, 0.004f}, {250f, 0.0079f}, {500f, 0.0056f}, {1000f, 0.0016f}, {2000f, 0.0014f}, {4000f, 0.0005f} };
    }

    private static void MetaDefault(ref Meta.XR.Acoustics.MaterialData data)
    {
        data.absorption = new Meta.XR.Acoustics.Spectrum{
            {1000f, 0.1f }};

        data.scattering = new Meta.XR.Acoustics.Spectrum{
            {1000f, 0.5f}};

        data.transmission = new Meta.XR.Acoustics.Spectrum{
            {1000f, 0.0f}};
    }
}
