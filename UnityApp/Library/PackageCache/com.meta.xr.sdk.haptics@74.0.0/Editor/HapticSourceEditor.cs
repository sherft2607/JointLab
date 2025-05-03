// (c) Meta Platforms, Inc. and affiliates. Confidential and proprietary.

using UnityEngine;
using UnityEditor;

namespace Oculus.Haptics.Editor
{
    /// <summary>
    /// A custom inspector for <see cref="HapticSource"/>. Provides custom controls
    /// for its properties and enables undo, multi-object editing and prefab overrides
    /// for all of its controls.
    /// </summary>
    ///
    /// <remarks>
    /// <c>HapticSource</c> has a number of serializable private fields which
    /// <c>HapticSourceEditor</c> uses as the backing data for its controls via
    /// serialized properties.
    /// </remarks>
    [CustomEditor(typeof(HapticSource))]
    [CanEditMultipleObjects]
    public class HapticSourceEditor : UnityEditor.Editor
    {
        /// These are used to bind to the serialized properties on <c>HapticSource</c> while
        /// providing undo, multi-object editing and prefab overrides.
        private SerializedProperty _clip, _controller, _loop, _amplitude, _frequencyShift, _priority;

        /// <summary>
        /// Binds serialized properties to the serializable fields on <c>HapticSource</c>.
        /// </summary>
        void OnEnable()
        {
            _clip = serializedObject.FindProperty("_clip");
            _controller = serializedObject.FindProperty("_controller");
            _loop = serializedObject.FindProperty("_loop");
            _amplitude = serializedObject.FindProperty("_amplitude");
            _frequencyShift = serializedObject.FindProperty("_frequencyShift");
            _priority = serializedObject.FindProperty("_priority");
        }

        /// <summary>
        /// Builds a control for each property.
        /// </summary>
        ///
        /// <remarks>
        /// A mixture of default and custom controls are used.
        /// </remarks>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_clip, new GUIContent("Clip"));
            EditorGUILayout.PropertyField(_controller, new GUIContent("Controller"));
            EditorGUILayout.PropertyField(_loop, new GUIContent("Loop"));
            FloatSlider(_amplitude, 0.0f, 5.0f, new GUIContent("Amplitude Scaling"));
            FloatSlider(_frequencyShift, -1.0f, 1.0f, new GUIContent("Frequency Shift"));
            IntSlider(_priority, 0, 255, new GUIContent("Priority"), "High", "Low");
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Builds a slider for a float property.
        /// </summary>
        ///
        /// <param name="property">The serialized property for which this slider will be built.</param>
        /// <param name="leftValue">The float value on the left of the slider.</param>
        /// <param name="rightValue">The float value on the right of the slider.</param>
        /// <param name="label">The label that should be displayed next to the slider.</param>
        private void FloatSlider(SerializedProperty property, float leftValue, float rightValue,
                                 GUIContent label)
        {
            Rect position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            label = EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();

            float newValue = EditorGUI.Slider(position, label.text, property.floatValue, leftValue, rightValue);

            // Only assign the value back if it was actually changed by the user.
            // Otherwise a single value will be assigned to all objects when multi-object editing,
            // even when the user didn't touch the control.
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = newValue;
            }

            EditorGUI.EndProperty();

            SliderLabels(position, leftValue.ToString(), rightValue.ToString());
        }

        /// <summary>
        /// Builds a slider for an int property.
        /// </summary>
        ///
        /// <param name="property">The serialized property for which this slider will be built.</param>
        /// <param name="leftValue">The int value on the left of the slider.</param>
        /// <param name="rightValue">The int value on the right of the slider.</param>
        /// <param name="label">The label that should be displayed next to the slider.</param>
        /// <param name="labelLeft">The label for the left value of the slider.</param>
        /// <param name="labelRight">The label for the right value of the slider.</param>
        private void IntSlider(SerializedProperty property, int leftValue, int rightValue,
                               GUIContent label, string labelLeft, string labelRight)
        {
            Rect position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            label = EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();

            int newValue = EditorGUI.IntSlider(position, label.text, property.intValue, leftValue, rightValue);

            // See comment in <c>floatValue</c> method.
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }

            EditorGUI.EndProperty();

            SliderLabels(position, labelLeft, labelRight);
        }

        /// <summary>
        /// Creates the labels for a slider.
        /// </summary>
        ///
        /// <remarks>
        /// This can be used for a slider of any data type.
        /// </remarks>
        ///
        /// <param name="position">The position of the slider for which these labels are being added.</param>
        /// <param name="leftValue">The label for the left value of the slider.</param>
        /// <param name="rightValue">The label for the right value of the slider.</param>
        void SliderLabels(Rect position, string leftValue, string rightValue)
        {
            // Move to next line.
            position.y += EditorGUIUtility.singleLineHeight;

            // Subtract the label.
            position.x += EditorGUIUtility.labelWidth;
            position.width -= EditorGUIUtility.labelWidth;

            // Subtract the text field width thats drawn with slider.
            position.width -= EditorGUIUtility.fieldWidth;

            GUIStyle style = GUI.skin.label;
            TextAnchor defaultAlignment = GUI.skin.label.alignment;
            style.alignment = TextAnchor.UpperLeft; EditorGUI.LabelField(position, leftValue, style);
            style.alignment = TextAnchor.UpperRight; EditorGUI.LabelField(position, rightValue, style);
            GUI.skin.label.alignment = defaultAlignment;

            // Allow space for the labels
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
    }
}
