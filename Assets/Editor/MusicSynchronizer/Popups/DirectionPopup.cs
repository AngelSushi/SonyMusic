using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using UnityEditor;
using UnityEngine;

public class DirectionPopup : PopupWindowContent
{

    private Vector2 _scrollPosition;
    private Vector2 _popupSize;

    private MusicController _musicController;

    private int _index;
    
    public DirectionPopup(int index,MusicController musicController)
    {
        this._index = index;
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
        
        GUILayout.Label("Direction Popup",style);

        int assetsLength = AssetDatabase.FindAssets("t:texture2D", new string[] { "Assets/Resources/Arrows" }).Length;
        
        _scrollPosition =GUI.BeginScrollView(new Rect(10, 10, _popupSize.x - 1, _popupSize.y - 13), _scrollPosition, new Rect(0, 0, _popupSize.x + 50 *  (assetsLength - 2), _popupSize.y - 13));
        int index = 0;

        foreach (string objStr in AssetDatabase.FindAssets("t:texture2D", new string[] {"Assets/Resources/Arrows"}))
        {
            var path = AssetDatabase.GUIDToAssetPath( objStr );
            Texture2D texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>( path );
            
            Rect obstacleRect = new Rect(20 + index * 70, 20, 50, 50);
            GUI.DrawTexture(obstacleRect,texture2D, ScaleMode.StretchToFill, true, 0, new Color(1f,1f,1f), 0, 0);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Vector2 mousePosition = Event.current.mousePosition;

                if (mousePosition.x >= obstacleRect.x && mousePosition.x <= obstacleRect.x + obstacleRect.width)
                {
                    if (mousePosition.y >= obstacleRect.y && mousePosition.y <= obstacleRect.y + obstacleRect.height)
                    {
                        _musicController.obstacles[_index].direction = ConvertStringToEnum(texture2D.name);
                    }
                }
            }
            
            index++;
        }
        

        
        
        GUI.EndScrollView();
    }

    private DashDirection ConvertStringToEnum(string value)
    {
        switch (value)
        {
            case "UP":
                return DashDirection.UP;
            
            case "DOWN":
                return DashDirection.DOWN;
            
            case "LEFT":
                return DashDirection.LEFT;
            
            case "RIGHT":
                return DashDirection.RIGHT;
            
            case "DIAGONAL_LUP":
                return DashDirection.DIAGONAL_LUP;
            
            case "DIAGONAL_RUP":
                return DashDirection.DIAGONAL_RUP;
            
            case "DIAGONAL_LDOWN":
                return DashDirection.DIAGONAL_LDOWN;
            
            case "DIAGONAL_RDOWN":
                return DashDirection.DIAGONAL_RDOWN;
            
        }

        return DashDirection.UP;
    }
}
