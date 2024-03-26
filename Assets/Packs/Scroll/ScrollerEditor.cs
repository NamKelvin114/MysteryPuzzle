#if UNITY_EDITOR
using UnityEngine.UI;
using UnityEditor.AnimatedValues;
using UnityEditor;
using UnityEngine;
using Lance.UI;

namespace Lance.Editor
{
    [CustomEditor(typeof(Scroller), true)]
    [CanEditMultipleObjects]
    public class ScrollerEditor : UnityEditor.Editor
    {
        private SerializedProperty _content;
        private SerializedProperty _movementType;
        private SerializedProperty _elasticity;
        private SerializedProperty _inertia;
        private SerializedProperty _decelerationRate;
        private SerializedProperty _scrollSensitivity;
        private SerializedProperty _viewport;
        private SerializedProperty _onValueChanged;

        //inherited
        private SerializedProperty _protoTypeCell;
        private SerializedProperty _selfInitialize;
        private SerializedProperty _direction;
        private SerializedProperty _type;
        private SerializedProperty _minPoolCoverage;
        private SerializedProperty _minPoolSize;
        private SerializedProperty _recyclingThreshold;

        private AnimBool _mShowElasticity;
        private AnimBool _mShowDecelerationRate;

        private Scroller _script;
        protected virtual void OnEnable()
        {
            _script = (Scroller)target;

            _content = serializedObject.FindProperty("m_Content");
            _movementType = serializedObject.FindProperty("m_MovementType");
            _elasticity = serializedObject.FindProperty("m_Elasticity");
            _inertia = serializedObject.FindProperty("m_Inertia");
            _decelerationRate = serializedObject.FindProperty("m_DecelerationRate");
            _scrollSensitivity = serializedObject.FindProperty("m_ScrollSensitivity");
            _viewport = serializedObject.FindProperty("m_Viewport");
            _onValueChanged = serializedObject.FindProperty("m_OnValueChanged");

            //Inherited
            _protoTypeCell = serializedObject.FindProperty("prototypeCell");
            _minPoolCoverage = serializedObject.FindProperty("minPoolCoverage");
            _minPoolSize = serializedObject.FindProperty("minPoolSize");
            _recyclingThreshold = serializedObject.FindProperty("recyclingThreshold");
            _selfInitialize = serializedObject.FindProperty("selfInitialize");
            _direction = serializedObject.FindProperty("direction");
            _type = serializedObject.FindProperty("isGrid");

            _mShowElasticity = new AnimBool(Repaint);
            _mShowDecelerationRate = new AnimBool(Repaint);
            SetAnimBools(true);
        }

        protected virtual void OnDisable()
        {
            _mShowElasticity.valueChanged.RemoveListener(Repaint);
            _mShowDecelerationRate.valueChanged.RemoveListener(Repaint);
        }

        private void SetAnimBools(bool instant)
        {
            SetAnimBool(_mShowElasticity, !_movementType.hasMultipleDifferentValues && _movementType.enumValueIndex == (int)ScrollRect.MovementType.Elastic, instant);
            SetAnimBool(_mShowDecelerationRate, !_inertia.hasMultipleDifferentValues && _inertia.boolValue, instant);
        }

        private void SetAnimBool(AnimBool a, bool value, bool instant)
        {
            if (instant)
                a.value = value;
            else
                a.target = value;
        }

        public override void OnInspectorGUI()
        {
            SetAnimBools(false); 
            serializedObject.Update();
          
            EditorGUILayout.PropertyField(_direction);
            EditorGUILayout.PropertyField(_type, new GUIContent("Grid"));
            if (_type.boolValue)
            {
                string title = _direction.enumValueIndex == (int)EScrollDirection.Vertical ? "Coloumns" : "Rows";
               _script.Segments =  EditorGUILayout.IntField(title, _script.Segments);
            }

            EditorGUILayout.PropertyField(_selfInitialize);
            EditorGUILayout.PropertyField(_viewport);
            EditorGUILayout.PropertyField(_content);
            GUI.backgroundColor = new Color(1f, 0.76f, 0.44f);
            EditorGUILayout.PropertyField(_protoTypeCell);
            EditorGUILayout.PropertyField(_minPoolCoverage);
            EditorGUILayout.PropertyField(_minPoolSize);
            EditorGUILayout.PropertyField(_recyclingThreshold);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_movementType);
            if (EditorGUILayout.BeginFadeGroup(_mShowElasticity.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_elasticity);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(_inertia);
            if (EditorGUILayout.BeginFadeGroup(_mShowDecelerationRate.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_decelerationRate);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(_scrollSensitivity);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_onValueChanged);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif

