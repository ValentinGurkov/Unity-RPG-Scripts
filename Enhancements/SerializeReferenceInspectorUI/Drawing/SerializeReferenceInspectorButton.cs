using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SerializeReferenceInspectorButton
{
    /// Must be drawn before DefaultProperty in order to receive input
    public static void DrawSelectionButtonForManagedReference(this SerializedProperty property,
        Rect position, IEnumerable<Func<Type, bool>> filters = null)
    {
        //var backgroundColor = new Color(0f, 0.7f, 0.7f, 1f);
        var backgroundColor = new Color(0.1f, 0.45f, 0.8f, 1f);
        //var backgroundColor = GUI.backgroundColor;

        Rect buttonPosition = position;
        buttonPosition.x += EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
        buttonPosition.width =
            position.width - EditorGUIUtility.labelWidth - 1 * EditorGUIUtility.standardVerticalSpacing;
        buttonPosition.height = EditorGUIUtility.singleLineHeight;

        int storedIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        Color storedColor = GUI.backgroundColor;
        GUI.backgroundColor = backgroundColor;


        (string AssemblyName, string ClassName) names =
            SerializeReferenceTypeNameUtility.GetSplitNamesFromTypename(property.managedReferenceFullTypename);
        string className = string.IsNullOrEmpty(names.ClassName) ? "Null (Assign)" : names.ClassName;
        string assemblyName = names.AssemblyName;
        if (GUI.Button(buttonPosition, new GUIContent(className, className + "  ( " + assemblyName + " )")))
            property.ShowContextMenuForManagedReference(filters);

        GUI.backgroundColor = storedColor;
        EditorGUI.indentLevel = storedIndent;
    }
}