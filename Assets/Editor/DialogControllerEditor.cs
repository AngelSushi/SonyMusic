using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CustomEditor(typeof(DialogController))]
public class DialogControllerEditor : Editor
{

    private DialogController classTarget;
    private SerializedObject serializedClass;

    private SerializedProperty _speakerFile;
    private SerializedProperty _dialogFile;
    
    private void OnEnable()
    {
        classTarget = (DialogController)target;
        serializedClass = new SerializedObject(classTarget);

        _speakerFile = serializedClass.FindProperty("speakerFile");
        _dialogFile = serializedClass.FindProperty("dialogFile");
    }

    public override void OnInspectorGUI() {
        serializedClass.Update();
        EditorGUI.BeginChangeCheck();
        
       // this.DrawDefaultInspector();
        
        GUIStyle textStyle = GUI.skin.label;
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontStyle = FontStyle.Bold;

        DrawSpeakerPart(textStyle);
        DrawDialogPart(textStyle);


        if(EditorGUI.EndChangeCheck()) 
            serializedClass.ApplyModifiedProperties();
        
    }

    private void DrawSpeakerPart(GUIStyle textStyle) {
        EditorGUILayout.Space(15);
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Speakers",textStyle);
        if (GUILayout.Button("X",GUILayout.Width(25))) {
            if (classTarget.speakers.Count == 0) 
                Debug.Log("la liste des speakers est vide");
            else {
                if (EditorUtility.DisplayDialog("Delete all Speakers","Êtes vous sur de vouloir supprimer tous les speakers","Oui","Non")) {
                    classTarget.speakers.Clear();
                    Debug.Log("La liste des speakers a été supprimée avec succès");
                }
            }
            
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_speakerFile);

        for (int i = 0; i < classTarget.speakers.Count; i++) {
            DialogController.Speaker speaker = classTarget.speakers[i];

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(speaker.id.ToString(),textStyle);
            
            if (GUILayout.Button("X",GUILayout.Width(25))) {
                if (EditorUtility.DisplayDialog("Delete  Speaker " + i,"Êtes vous sur de vouloir supprimer le speaker n°" + i,"Oui","Non")) {
                    classTarget.speakers.RemoveAt(i);
                    Debug.Log("Le speaker n°" + i + " a été supprimé avec succès");
                }
            }


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            GUILayout.Box(speaker.speakerTex,GUILayout.Width(150),GUILayout.Height(150));
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Speaker Name");
            speaker.name = EditorGUILayout.TextField(speaker.name);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawDialogPart(GUIStyle textStyle) {
        EditorGUILayout.Space(15);

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Dialogs",textStyle);
        if (GUILayout.Button("X",GUILayout.Width(25))) {
            if (classTarget.dialogs.Count == 0) 
                Debug.Log("la liste des dialogues est vide");
            else {
                if (EditorUtility.DisplayDialog("Delete all Dialogs","Êtes vous sur de vouloir supprimer tous les dialogues","Oui","Non")) {
                    classTarget.dialogs.Clear();
                    Debug.Log("La liste des dialogues a été supprimée avec succès");
                }
            }
            
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(_dialogFile);
        EditorGUILayout.EndVertical();
    }
}
