using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DashDirection))]
public class DashDirectionEditor : Editor
{

    private DashDirection classTarget;
    private SerializedObject serializedClass;

    private SerializedProperty dashDirection;
    
    private void OnEnable()
    {
        classTarget = (DashDirection)target;
        serializedClass = new SerializedObject(classTarget);

        dashDirection = serializedClass.FindProperty("dashDirectionRef");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        
        EditorGUILayout.PropertyField(dashDirection);

        if (classTarget.dashDirectionRef != null)
        {
            GUILayout.Box(classTarget.dashDirectionRef,GUILayout.Width(100),GUILayout.Height(10));
        }

        
        if(EditorGUI.EndChangeCheck()) 
            serializedClass.ApplyModifiedProperties();

    }
}
