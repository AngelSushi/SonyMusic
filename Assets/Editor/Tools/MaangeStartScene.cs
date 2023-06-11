using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class ManageStartScene
{

    private static int _select;
    private static string[] _scenes = new string[] { "MainMenu", "PlayerSelection", "NameSelection", "Dialog", "Gameplay" };
    static ManageStartScene()
    {
        ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
    }

    private static void OnRightToolbarGUI()
    {
        _select = EditorGUILayout.Popup("Launch Scene", _select, _scenes,GUILayout.Width(250));

        if (GUILayout.Button("O",GUILayout.Width(40)))
        {
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene("Assets/Scenes/" + _scenes[_select]  + ".unity");
        }
    }
}
