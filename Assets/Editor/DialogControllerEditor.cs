using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogController))]
public class DialogControllerEditor : Editor
{

    private DialogController classTarget;
    private SerializedObject serializedClass;
    
    private void OnEnable()
    {
        classTarget = (DialogController)target;
        serializedClass = new SerializedObject(classTarget);
    }

    public override void OnInspectorGUI() {
        serializedClass.Update();
        EditorGUI.BeginChangeCheck();
        
        this.DrawDefaultInspector();
        
        
        if(EditorGUI.EndChangeCheck()) 
            serializedClass.ApplyModifiedProperties();
        
    }
}
