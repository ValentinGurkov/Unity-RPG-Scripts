#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

//created by Unity forum user TextusGames
public static class SerializeReferenceInspectorUI {
    public static void ShowContextMenuForManagedReferenceOnMouseMiddleButton(this SerializedProperty property, Rect position) {
        var e = Event.current;
        if (e.type != EventType.MouseDown || !position.Contains(e.mousePosition) || e.button != 2)
            return;

        ShowContextMenuForManagedReference(property);
    }

    /// Must be drawn before DefaultProperty in order to receive input
    public static void DrawSelectionButtonForManagedReference(this SerializedProperty property, Rect position) {
        //var backgroundColor = new Color(0f, 0.7f, 0.7f, 1f); 
        var backgroundColor = new Color(0.1f, 0.45f, 0.8f, 1f);
        //var backgroundColor = GUI.backgroundColor;     

        var buttonPosition = position;
        buttonPosition.x += EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
        buttonPosition.width = position.width - EditorGUIUtility.labelWidth - 1 * EditorGUIUtility.standardVerticalSpacing;
        buttonPosition.height = EditorGUIUtility.singleLineHeight;

        var storedIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        var storedColor = GUI.backgroundColor;
        GUI.backgroundColor = backgroundColor;

        var names = SerializeReferenceInspectorUtility.GetSplitNamesFromTypename(property.managedReferenceFullTypename);
        var className = string.IsNullOrEmpty(names.ClassName) ? "Null (Assign)" : names.ClassName;
        var assemblyName = names.AssemblyName;
        if (GUI.Button(buttonPosition, new GUIContent(className, className + "  ( " + assemblyName + " )"))) {
            property.ShowContextMenuForManagedReference();
        }

        GUI.backgroundColor = storedColor;
        EditorGUI.indentLevel = storedIndent;
    }

    public static void ShowContextMenuForManagedReference(this SerializedProperty property) {
        var context = new GenericMenu();
        FillContextMenu();
        context.ShowAsContext();

        void FillContextMenu() {
            context.AddItem(new GUIContent("Null"), false, MakeNull);
            var realPropertyType = SerializeReferenceInspectorUtility.GetRealTypeFromTypename(property.managedReferenceFieldTypename);
            if (realPropertyType == null) {
                Debug.LogError("Can not get type from");
                return;
            }

            var types = TypeCache.GetTypesDerivedFrom(realPropertyType);
            foreach (var currentType in types) {
                // Skips unity engine Objects (because they are not serialized by SerializeReference)
                if (currentType.IsSubclassOf(typeof(UnityEngine.Object)))
                    continue;
                if (currentType.IsAbstract)
                    continue;

                AddContextMenu(currentType);
            }

            void MakeNull() {
                property.serializedObject.Update();
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo(); // undo is bugged
            }

            void AddContextMenu(Type type) {
                var assemblyName = type.Assembly.ToString().Split('(', ',') [0];
                var entryName = type + "  ( " + assemblyName + " )";
                context.AddItem(new GUIContent(entryName), false, AssignNewInstanceOfType, type);
            }

            void AssignNewInstanceOfType(object typeAsObject) {
                var type = (Type) typeAsObject;
                var instance = Activator.CreateInstance(type);
                property.serializedObject.Update();
                property.managedReferenceValue = instance;
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo(); // undo is bugged
            }
        }
    }
}

#endif
