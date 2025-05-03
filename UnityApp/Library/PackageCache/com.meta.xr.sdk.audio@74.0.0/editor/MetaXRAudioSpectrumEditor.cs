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
 * Filename    :   MetaXRAudioSpectrumEditor.cs
 * Content     :   Editor for audio spectra (e.g. materials)
 ***********************************************************************************/
using UnityEditor;
using UnityEngine;
using Point = Meta.XR.Acoustics.Spectrum.Point;
using Spectrum = Meta.XR.Acoustics.Spectrum;

internal sealed class MetaXRAudioSpectrumEditor
{
    internal enum AxisScale
    {
        Linear, Log, Square, Cube, SquareCentered
    }

    private static readonly Texture2D texture = EditorGUIUtility.whiteTexture;

    /// A text style that is used to draw frequency labels and tick marks.
    private static readonly GUIStyle frequencyTextStyle = new GUIStyle
    {
        alignment = TextAnchor.MiddleLeft,
        clipping = TextClipping.Overflow,
        fontSize = 8,
        fontStyle = FontStyle.Bold,
        wordWrap = false,
        normal = new GUIStyleState { textColor = new Color(0.1f, 0.1f, 0.1f) },
        focused = new GUIStyleState { textColor = new Color(0.1f, 0.1f, 0.1f) }
    };

    /// A text style that is used to draw data labels and tick marks.
    private static readonly GUIStyle dataTextStyle = new GUIStyle
    {
        alignment = TextAnchor.MiddleLeft,
        clipping = TextClipping.Overflow,
        fontSize = 8,
        fontStyle = FontStyle.Bold,
        wordWrap = false,
        normal = new GUIStyleState { textColor = Color.gray },
        focused = new GUIStyleState { textColor = Color.gray }
    };

    /// A text style that is used to draw a label for the selected point.
    private static readonly GUIStyle selectedTextStyle = new GUIStyle
    {
        alignment = TextAnchor.LowerCenter,
        clipping = TextClipping.Overflow,
        fontSize = 8,
        fontStyle = FontStyle.Bold,
        wordWrap = false,
        normal = new GUIStyleState { textColor = Color.white },
        focused = new GUIStyleState { textColor = Color.white }
    };

    private static int focus;

    private bool dragInitiated;
    private bool isDragging;

    private bool displaySpectrum = true;
    private bool displayPoints = false;

    private readonly string label;
    private readonly string tooltip;

    private readonly float rangeMin;
    private readonly float rangeMax;
    private readonly float rangeOrigin;

    private readonly AxisScale scale;

    /// The size in pixels of the right margin where axis tick mark labels are displayed.
    private const float rightMargin = 36.0f;

    /// The maximum frequency that is displayed.
    private const float frequencyMax = 20000.0f;

    /// The height of the spectrum graph in pixels
    internal float spectrumHeight = 120.0f;

    /// The size of a data point when it is selected.
    internal float selectedPointSize = 12.0f;

    /// Whether or not the user is able to add new points to the spectrum.
    internal bool canAddPoints = true;

    /// Whether or not the user is able to remove points from the spectrum.
    internal bool canRemovePoints = true;

    /// Whether or not the user can edit the frequency values of points in the spectrum.
    internal bool canEditFrequency = true;

    /// Whether or not the user can edit the data values of points in the spectrum.
    internal bool canEditData = true;

    /// Whether or not the spectrum is drawn with the area under the curve shaded.
    internal bool drawFilled = true;

    /// The color that is used to draw the spectrum curve.
    internal Color spectrumColor = AudioCurveRendering.kAudioOrange;

    /// The units to display for the data values.
    internal string dataUnits = "";

    internal static readonly string pointAddedGroupName = "Point Added";
    internal static readonly string pointRemovedGroupName = "Point Removed";
    internal static readonly string pointSelectedGroupName = "Point Selected";
    internal static readonly string pointMovedGroupName = "Point Moved";

    internal void LoadFoldoutState()
    {
        displaySpectrum = EditorPrefs.GetBool(label + "SpectrumFoldout", true);
        displayPoints = EditorPrefs.GetBool(label + "PointsFoldout", false);
    }

    internal void SaveFoldoutState()
    {
        EditorPrefs.SetBool(label + "SpectrumFoldout", displaySpectrum);
        EditorPrefs.SetBool(label + "PointsFoldout", displayPoints);
    }

    internal static float GetRightMargin()
    {
        return rightMargin;
    }

    internal bool hasFocus(Spectrum spectrum)
    {
        return focus == spectrum.GetHashCode();
    }

    internal MetaXRAudioSpectrumEditor(string label, string tooltip, AxisScale scale, Color spectrumColor, float rangeMin = 0.0f, float rangeMax = 1.0f, float rangeOrigin = 0.0f)
    {
        this.label = label;
        this.tooltip = tooltip;
        this.scale = scale;
        this.spectrumColor = spectrumColor;
        this.rangeMin = rangeMin;
        this.rangeMax = rangeMax;
        this.rangeOrigin = rangeOrigin;
    }

    internal void Draw(Spectrum spectrum, Event e)
    {
        if (DrawFoldout())
            DrawFoldoutSpectrum(spectrum, e);
    }

    internal bool DrawFoldout()
    {
        displaySpectrum = EditorGUILayout.Foldout(displaySpectrum, new GUIContent(label, tooltip));
        return displaySpectrum;
    }

    internal void DrawFoldoutSpectrum(Spectrum spectrum, Event e)
    {
        EditorGUI.indentLevel++;

        // Note: note if UI modifes the spectrum a clone is returned
        DrawSpectrum(spectrum, e);

        displayPoints = EditorGUILayout.Foldout(displayPoints, "Points");

        if (displayPoints)
        {
            EditorGUI.indentLevel++;
            DrawPoints(spectrum);
            EditorGUI.indentLevel--;
        }

        EditorGUI.indentLevel--;
    }

    private void DrawSpectrum(Spectrum spectrum, Event e)
    {
        Rect r = EditorGUILayout.GetControlRect(true, spectrumHeight);

        r.width -= rightMargin;
        DrawDataTicks(r);
        r = AudioCurveRendering.BeginCurveFrame(r);

        DrawFrequencyTicks(r);

        if (drawFilled)
            AudioCurveRendering.DrawMinMaxFilledCurve(r, EvaluateCurveMinMaxColor(spectrum));

        AudioCurveRendering.DrawCurve(r, EvaluateCurve(spectrum), spectrumColor);

        HandleEvent(spectrum, r, e);
        if (hasFocus(spectrum))
            DrawSelected(spectrum, r);

        AudioCurveRendering.EndCurveFrame();
    }

    private void DrawPoints(Spectrum spectrum)
    {
        int pointCount = spectrum.points.Count;
        int lines = pointCount > 0 ? pointCount + 2 : 1;
        float height = EditorGUIUtility.singleLineHeight * lines;
        Rect r1 = EditorGUILayout.GetControlRect(true, height);
        r1.width -= rightMargin;
        r1.height = EditorGUIUtility.singleLineHeight;

        {
            int oldCount = pointCount;
            int newCount = EditorGUI.DelayedIntField(r1, "Size", oldCount);
            r1.y += r1.height;

            if (canRemovePoints && newCount < pointCount)
            {
                spectrum.points.RemoveRange(newCount, oldCount - newCount);
                Undo.SetCurrentGroupName("Points Removed");
                GUI.changed = true;
            }
            else if (canAddPoints && newCount > oldCount)
            {
                if (newCount > spectrum.points.Capacity)
                    spectrum.points.Capacity = newCount;

                for (int i = oldCount; i < newCount; i++)
                    spectrum.points.Add(new Point(125 * (1 << i)));

                Undo.SetCurrentGroupName("Points Added");
                GUI.changed = true;
            }
        }

        pointCount = spectrum.points.Count;
        if (pointCount > 0)
        {
            Rect r2 = new Rect(r1.xMax + 9, r1.y + r1.height * 1.125f, 24, r1.height * .75f);

            r1.width /= 2;
            EditorGUI.LabelField(r1, "Frequency");
            r1.x += r1.width;
            EditorGUI.LabelField(r1, label);
            r1.x -= r1.width;
            r1.y += r1.height;

            for (int i = 0; i < pointCount; i++)
            {
                // Frequency field
                if (canEditFrequency)
                {
                    GUIStyle style = EditorStyles.textField;
                    style.alignment = TextAnchor.MiddleRight;
                    float freq = EditorGUI.FloatField(r1, Mathf.Round(spectrum.points[i].frequency), style);
                    freq = Mathf.Clamp(freq, 0.0f, frequencyMax);
                    if (freq != spectrum.points[i].frequency)
                        spectrum.points[i] = new Point(freq, spectrum.points[i].data);
                }
                else
                {
                    EditorGUI.LabelField(r1, FrequencyToString(spectrum.points[i].frequency));
                }

                // Data field
                r1.x += r1.width;
                if (canEditData)
                {
                    float data = EditorGUI.FloatField(r1, spectrum.points[i].data, EditorStyles.textField);
                    data = Mathf.Clamp(data, rangeMin, rangeMax);
                    if (data != spectrum.points[i].data)
                    {
                        Debug.Log($"Changed data from {spectrum.points[i].data} to {data}");
                        spectrum.points[i] = new Point(spectrum.points[i].frequency, data);
                    }
                }
                else
                {
                    EditorGUI.LabelField(r1, DataToString(spectrum.points[i].data));
                }
                r1.x -= r1.width;
                r1.y += r1.height;

                // Remove button
                if (canRemovePoints && GUI.Button(r2, "–"))
                {
                    RemovePointAt(spectrum, i);
                    break;
                }

                r2.y += r1.height;
            }
        }
    }

    private void DrawDataTicks(Rect r)
    {
        const int ticks = 10;
        Rect label = new Rect(r.xMax + 9, r.y - r.height / (2 * ticks), 24, r.height / ticks);
        Rect tick = new Rect(r.xMax + 2, r.y - 1, 4.5f, 2);

        for (int i = 0; i <= ticks; i++)
        {
            float value = MapData(1 - (float)i / ticks, false);

            EditorGUI.DrawRect(tick, dataTextStyle.normal.textColor);
            GUI.Label(label, value.ToString("0.000"), dataTextStyle);
            tick.y += label.height;
            label.y += label.height;
        }
    }

    private void DrawFrequencyTicks(Rect r)
    {
        Rect tick = new Rect(r.x, r.y, 1.0f, r.height);
        Rect label = new Rect(r.x, 0.5f * EditorGUIUtility.singleLineHeight, 32.0f, EditorGUIUtility.singleLineHeight);

        for (int i = 1; i < 30; i++)
        {
            float frequency;
            if (MapFrequencyTick(i, out frequency))
            {
                tick.x = MapFrequency(frequency) * r.width;
                tick.height = label.y - r.y;
                tick.width = 2.0f;
                EditorGUI.DrawRect(tick, frequencyTextStyle.normal.textColor);

                tick.y = label.yMax;
                tick.height = r.yMax - label.yMax;
                EditorGUI.DrawRect(tick, frequencyTextStyle.normal.textColor);

                label.x = tick.x - 2.0f;
                GUI.Label(label, FrequencyToTickString(frequency), frequencyTextStyle);

                tick.y = r.y;
                tick.height = r.height;
                tick.width = 1.0f;
            }
            else
            {
                tick.x = MapFrequency(frequency) * r.width;
                EditorGUI.DrawRect(tick, frequencyTextStyle.normal.textColor);
            }
        }
    }

    private void DrawSelected(Spectrum spectrum, Rect r)
    {
        if (spectrum.points.Count > spectrum.selection)
        {
            Point point = spectrum.points[spectrum.selection];

            // Draw a circle for the selected point.
            Vector2 position = MapPointPosition(r, point);
            Vector2 pointSize = new Vector2(selectedPointSize, selectedPointSize);
            Rect pointRect = new Rect(position - pointSize * 0.5f, pointSize);

            // White circle with black outline
#if UNITY_5
            GUI.DrawTexture(pointRect, texture, ScaleMode.StretchToFill, false, 0);
            GUI.DrawTexture(pointRect, texture, ScaleMode.StretchToFill, false, 0);
#else
            GUI.DrawTexture(pointRect, texture, ScaleMode.StretchToFill, false, 0, Color.white, 0, selectedPointSize);
            GUI.DrawTexture(pointRect, texture, ScaleMode.StretchToFill, false, 0, Color.black, 2, selectedPointSize);
#endif

            // Draw a label above the point with the current values.
            const float labelPadding = 2.0f;
            Vector2 labelSize = new Vector2(30.0f, EditorGUIUtility.singleLineHeight);
            Rect labelRect = new Rect(position -
                new Vector2(labelSize.x * 0.5f, labelSize.y + selectedPointSize * 0.5f + labelPadding),
                labelSize);
            GUI.Label(labelRect, FrequencyToString(point.frequency) + "\n" + DataToString(point.data), selectedTextStyle);
        }
    }

    private void HandleEvent(Spectrum spectrum, Rect r, Event e)
    {
        Vector2 position = e.mousePosition;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (r.Contains(position))
                {
                    if (e.clickCount == 2)
                    {
                        if (canAddPoints)
                        {
                            spectrum.selection = spectrum.points.Count;
                            spectrum.points.Add(MapMouseEvent(r, position));
                            spectrum.Sort();
                            Undo.SetCurrentGroupName(pointAddedGroupName);
                            GUI.changed = true;
                        }
                    }
                    else
                    {
                        int selection = spectrum.selection;
                        float minDistance = float.MaxValue;

                        for (int i = 0; i < spectrum.points.Count; i++)
                        {
                            float distance = Vector2.Distance(MapPointPosition(r, spectrum.points[i]), position);
                            if (distance < minDistance)
                            {
                                selection = i;
                                minDistance = distance;
                            }
                        }

                        if (selection != spectrum.selection)
                        {
                            spectrum.selection = selection;
                            Undo.SetCurrentGroupName(pointSelectedGroupName);
                            GUI.changed = true;
                        }
                    }

                    focus = spectrum.GetHashCode();
                    dragInitiated = true;
                }
                else
                {
                    isDragging = false;
                    focus = 0;
                }

                e.Use();
                break;

            case EventType.MouseDrag:
                if (dragInitiated)
                {
                    dragInitiated = false;
                    isDragging = true;
                }
                if (isDragging && spectrum.selection < spectrum.points.Count)
                {
                    Point newPoint = MapMouseEvent(r, position);

                    if (!canEditFrequency)
                        newPoint.frequency = spectrum.points[spectrum.selection].frequency;

                    if (!canEditData)
                        newPoint.data = spectrum.points[spectrum.selection].data;

#if META_XR_ACOUSTIC_INFO
                    Debug.Log($"point moved: {spectrum.points[spectrum.selection]} -> {newPoint}");
#endif
                    spectrum.points[spectrum.selection] = newPoint;

                    e.Use();
                }
                break;

            case EventType.Ignore:
            case EventType.MouseUp:
                dragInitiated = false;
                if (isDragging)
                {
                    isDragging = false;
                    Undo.SetCurrentGroupName(pointMovedGroupName);
                    GUI.changed = true;
                    spectrum.Sort();
                    e.Use();
                }
                break;

            case EventType.KeyDown:
                switch (e.keyCode)
                {
                    case KeyCode.Delete:
                    case KeyCode.Backspace:
                        if (canRemovePoints && spectrum.selection < spectrum.points.Count)
                        {
                            RemovePointAt(spectrum, spectrum.selection);
                            e.Use();
                        }
                        break;
                }
                break;

        }
    }

    private void RemovePointAt(Spectrum spectrum, int index)
    {
        spectrum.points.RemoveAt(index);

        if (spectrum.selection == index)
            spectrum.selection = spectrum.points.Count;

        Undo.SetCurrentGroupName(pointRemovedGroupName);
        GUI.changed = true;
    }

    private AudioCurveRendering.AudioCurveEvaluator EvaluateCurve(Spectrum spectrum)
    {
        return (float f) => 2 * MapData(spectrum[MapFrequency(f, false)]) - 1;
    }

    private AudioCurveRendering.AudioMinMaxCurveAndColorEvaluator EvaluateCurveMinMaxColor(Spectrum spectrum)
    {
        return (float f, out Color color, out float minValue, out float maxValue) =>
        {
            float y = 2.0f * MapData(spectrum[MapFrequency(f, false)]) - 1.0f;
            float c = 2.0f * (rangeOrigin - rangeMin) / (rangeMax - rangeMin) - 1.0f;
            minValue = Mathf.Min(y, c);
            maxValue = Mathf.Max(y, c);
            color = spectrumColor;
            color.a = 0.3f;
        };
    }

    private Vector2 MapPointPosition(Rect r, Point point) => new Vector2
    {
        x = r.x + r.width * MapFrequency(point.frequency),
        y = r.y + r.height * (1 - MapData(point.data))
    };

    private Point MapMouseEvent(Rect r, Vector2 v) => new Point
    {
        frequency = v.x < r.xMin ? 0.0f : v.x > r.xMax ? frequencyMax : MapFrequency((v.x - r.x) / r.width, false),
        data = v.y < r.yMin ? rangeMax : v.y > r.yMax ? rangeMin : MapData(1 - (v.y - r.y) / r.height, false)
    };

    private float Square(float f)
    {
        return f * f;
    }

    private float MapData(float f, bool forward = true)
    {
        float rangeSize = rangeMax - rangeMin;
        if (forward)
            f = (f - rangeMin) / rangeSize;

        switch (scale)
        {
            case AxisScale.Log:
                f = forward ? f < 1e-3f ? 0 : 1 + (Mathf.Log10(f) / 3) : Mathf.Pow(10, -3 * (1 - f));
                break;

            case AxisScale.Square:
                f = forward ? Mathf.Sqrt(f) : f * f;
                break;

            case AxisScale.SquareCentered:
                if (forward)
                {
                    f = f > 0.5f ?
                        0.5f + 0.5f * Mathf.Sqrt((f - 0.5f) * 2.0f) :
                        0.5f - 0.5f * Mathf.Sqrt((0.5f - f) * 2.0f);
                }
                else
                {
                    f = f > 0.5f ?
                        0.5f + 0.5f * Square((f - 0.5f) * 2.0f) :
                        0.5f - 0.5f * Square((0.5f - f) * 2.0f);
                }
                break;

            case AxisScale.Cube:
                f = forward ? Mathf.Pow(f, 1.0f / 3.0f) : f * f * f;
                break;

            default:
            case AxisScale.Linear:
                break;
        }

        if (!forward)
            f = f * rangeSize + rangeMin;

        return f;
    }

    private static bool MapFrequencyTick(int i, out float frequency)
    {
        int power = i / 9 + 1;
        int multiplier = i % 9 + 1;

        frequency = multiplier * Mathf.Pow(10, power);

        return multiplier == 1;
    }

    private static float MapFrequency(float f, bool forward = true)
    {
        if (forward)
            return f < 10.0f ? 0.0f : Mathf.Log(f / 10.0f, frequencyMax / 10.0f);
        else
            return 10.0f * Mathf.Pow(frequencyMax / 10.0f, f);
    }

    /// Return a formated string with units for the specified frequency in hertz.
    private static string FrequencyToString(float frequency)
    {
        if (frequency < 1000)
            return string.Format("{0:F1} Hz", frequency);
        else
            return string.Format("{0:F2} kHz", frequency * 0.001f);
    }

    /// Return a formated tick label string with units for the specified frequency in hertz.
    private static string FrequencyToTickString(float frequency)
    {
        if (frequency < 1000)
            return string.Format("{0:F0} Hz", frequency);
        else
            return string.Format("{0:F0} kHz", frequency * 0.001f);
    }

    /// Return a formated string for the specified data value.
    private string DataToString(float data)
    {
        return string.Format("{0:F3}", data) + dataUnits;
    }
}
