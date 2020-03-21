#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializeReferenceMenuAttribute))]
public class SerializeReferenceMenuAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        IEnumerable<Func<Type, bool>> typeRestrictions =
            SerializedReferenceUIBuiltInTypeRestrictions.GetAllBuiltInTypeRestrictions(fieldInfo);
        property.ShowContextMenuForManagedReferenceOnMouseMiddleButton(position, typeRestrictions);

        EditorGUI.PropertyField(position, property, true);
        EditorGUI.EndProperty();
    }
}
#endif