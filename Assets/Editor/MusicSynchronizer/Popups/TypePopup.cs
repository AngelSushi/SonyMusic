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

        int assetsLength = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Prefab/Obstacles" }).Length;
        
        _scrollPosition =GUI.BeginScrollView(new Rect(10, 10, _popupSize.x - 1, _popupSize.y - 13), _scrollPosition, new Rect(0, 0, _popupSize.x + 50 *  (assetsLength - 2), _popupSize.y - 13));
        int index = 0;
        
        foreach (string objStr in AssetDatabase.FindAssets("t:prefab", new string[] {"Assets/Prefab/Obstacles"}))
        {
            var path = AssetDatabase.GUIDToAssetPath( objStr );
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>( path );

            if (go.name == "Empty")
            {
                continue;
            }
            
            SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();

            if (renderer != null)
            {
                Texture2D obstacleTexture = renderer.sprite.texture;
                Rect obstacleRect = new Rect(20 + index * 70, 20, 50, 50);
                GUI.DrawTexture(obstacleRect,obstacleTexture, ScaleMode.StretchToFill, true, 0, renderer.color, 0, 0);

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    Vector2 mousePosition = Event.current.mousePosition;

                    if (mousePosition.x >= obstacleRect.x && mousePosition.x <= obstacleRect.x + obstacleRect.width)
                    {
                        if (mousePosition.y >= obstacleRect.y && mousePosition.y <= obstacleRect.y + obstacleRect.height)
                        {
                            Sprite modelSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Square.png");
                            Sprite newSprite = modelSprite;
                            
                            _musicController.obstacles[_targetObstacle.obstacleIndex].sprite = newSprite;
                            _musicController.obstacles[_targetObstacle.obstacleIndex].color = renderer.color;
                        }
                    }
                }
                
                index++;
            }
        

        }
        
        GUI.EndScrollView();
    }
}
