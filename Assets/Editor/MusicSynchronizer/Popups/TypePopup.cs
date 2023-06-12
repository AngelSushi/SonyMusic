using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using UnityEditor;
using UnityEngine;

public class TypePopup : PopupWindowContent
{

    private Vector2 _scrollPosition;
    private Vector2 _popupSize;

    private MusicSynchronizer.ObstacleUI _targetObstacle;
    private MusicController _musicController;
    
    public TypePopup(MusicSynchronizer.ObstacleUI targetObstacle,MusicController musicController)
    {
        this._targetObstacle = targetObstacle;
        this._musicController = musicController;
    }
    
    public override Vector2 GetWindowSize()
    {
        _popupSize = new Vector2(200, 100);
        return _popupSize;
    }
    
    public override void OnGUI(Rect rect)
    {

        GUIStyle style = EditorStyles.boldLabel;
        style.alignment = TextAnchor.UpperCenter;
        
        GUILayout.Label("Type Popup",style);

        int assetsLength = AssetDatabase.FindAssets("t:texture2D", new string[] { "Assets/Resources/Obstacles" }).Length;
        
        _scrollPosition =GUI.BeginScrollView(new Rect(10, 10, _popupSize.x - 1, _popupSize.y - 13), _scrollPosition, new Rect(0, 0, _popupSize.x + 50 *  (assetsLength - 2), _popupSize.y - 13));
        int index = 0;
        
        foreach (string objStr in AssetDatabase.FindAssets("t:texture2D", new string[] {"Assets/Resources/Obstacles"}))
        {
            var path = AssetDatabase.GUIDToAssetPath( objStr );
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>( path);
            
            Texture2D obstacleTexture = tex;
            Rect obstacleRect = new Rect(20 + index * 70, 20, 50, 50);
            GUI.DrawTexture(obstacleRect,obstacleTexture/*, ScaleMode.StretchToFill, true, 0, renderer.color, 0, 0*/);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Vector2 mousePosition = Event.current.mousePosition;

                if (mousePosition.x >= obstacleRect.x && mousePosition.x <= obstacleRect.x + obstacleRect.width)
                {
                    if (mousePosition.y >= obstacleRect.y && mousePosition.y <= obstacleRect.y + obstacleRect.height)
                    {
                        Sprite modelSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Obstacles/" + tex.name + ".png");
                        _musicController.obstacles[_targetObstacle.obstacleIndex].sprite = modelSprite;
                        _musicController.obstacles[_targetObstacle.obstacleIndex].color = Color.white;
                    }
                }
            }
            
            index++;

        }
        
        GUI.EndScrollView();
    }
}
