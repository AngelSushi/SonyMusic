using System;
using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MusicSynchronizer : EditorWindow
{

    private Vector2 _scrollPosition;
    private bool _hasLoadMidiFile;
    private Rect _popupRect;
    
    public class ObstacleUI
    {
        public Rect position;
        public int obstacleIndex;

        public ObstacleUI(Rect position,int obstacleIndex)
        {
            this.position = position;
            this.obstacleIndex = obstacleIndex;
        }
    }


    private void OnEnable()
    {
        if (!_hasLoadMidiFile && _musicController != null)
        {
            _hasLoadMidiFile = true;
            _musicController.LoadWindows();
                
            if (_musicController.obstacles.Count == 0)
            {
                for (int i = 0; i < _musicController.allNotes.Count; i++)
                {      
                    GameObject emptyObstacle = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Obstacles/Empty.prefab");
                    _musicController.obstacles.Add(new Obstacle(emptyObstacle.GetComponent<SpriteRenderer>().sprite,emptyObstacle.GetComponent<SpriteRenderer>().color,i % 2 == 0 ? DashDirection.DOWN : DashDirection.LEFT));
                }
            }
                
        }
    }

    [MenuItem("Window/Audio/MusicSynchronizer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MusicSynchronizer));
    }

    private MusicController _musicController;

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0,0,position.width,position.height),Resources.Load<Texture2D>("background"));
        
        _musicController = (MusicController)EditorGUILayout.ObjectField("Music Controller",_musicController,typeof(MusicController));
        
        if (_musicController != null)
        {
            if (!_hasLoadMidiFile || _musicController.allNotes == null)
            {
                _hasLoadMidiFile = true;
                _musicController.LoadWindows();
                
                if (_musicController.obstacles.Count == 0)
                {
                    for (int i = 0; i < _musicController.allNotes.Count; i++)
                    {
                        GameObject emptyObstacle = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Obstacles/Empty.prefab");
                        _musicController.obstacles.Add(new Obstacle(emptyObstacle.GetComponent<SpriteRenderer>().sprite,emptyObstacle.GetComponent<SpriteRenderer>().color,i % 2 == 0 ? DashDirection.DOWN : DashDirection.LEFT));
                    }
                }
                
            }
            
            _scrollPosition =GUI.BeginScrollView(new Rect(10, 10, position.width - 1, position.height - 13), _scrollPosition, new Rect(0, 0, position.width + ((_musicController.mainAudio.clip.length - 16) * 150), position.height - 13));

            Vector2 begin = new Vector2(0, 10);
            for (int i = 0; i < _musicController.mainAudio.clip.length; i++)
            {
                GUI.Label(new Rect(begin.x,begin.y,30,15),"" + i);
                GUI.DrawTexture(new Rect(begin.x + 4,begin.y + 18,1,position.height - 2),EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, true, 0, new Color(1f,1f,1f,0.3f), 0, 0);
                begin.x += 150;
            }

            float yOffset = (position.height - 50) / _musicController.lanes.Length;
            
            List<NoteName> names = new List<NoteName>();
            List<ObstacleUI> obstaclesUi = new List<ObstacleUI>();

    
            
            if (_musicController.allNotes != null && _musicController.allNotes.Count > 0)
            {
                for (int i = 0; i < _musicController.allNotes.Count; i++)
                {
                    if (!names.Contains(_musicController.allNotes[i].NoteName))
                    {
                        names.Add(_musicController.allNotes[i].NoteName);
                    }
                }
            }
            
            names.Sort();
            names.Reverse();

            for (int i = 0; i < _musicController.allNotes.Count; i++)
            {
                int nameIndex = names.IndexOf(_musicController.allNotes[i].NoteName);
                
                var convertTime = TimeConverter.ConvertTo<MetricTimeSpan>(_musicController.allNotes[i].Time, MusicController.midiFile.GetTempoMap());
                double time = (double)convertTime.Minutes * 60f + convertTime.Seconds + (double)convertTime.Milliseconds / 1000f;

                
                Rect obstaclePosition = new Rect((float)time * 150, 150 + yOffset * nameIndex, 20, 20);
                GUI.DrawTexture(obstaclePosition,_musicController.obstacles[i].sprite.texture, ScaleMode.StretchToFill, true, 0, _musicController.obstacles[i].color, 0, 0);
                
                obstaclesUi.Add(new ObstacleUI(obstaclePosition,i));
                
                Texture2D directionTex = Resources.Load<Texture2D>("Arrows/" + _musicController.obstacles[i].direction.ToString().ToUpper() + "");
                obstaclePosition.y += 30;
                GUI.DrawTexture(obstaclePosition,directionTex);
                
                
            }

            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    
                    Vector2 mousePosition = Event.current.mousePosition;
                    if (ObstacleOnMousePosition(obstaclesUi, mousePosition) != null)
                    {
                        _popupRect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 200, 100);
                        PopupWindow.Show(_popupRect,new TypePopup(ObstacleOnMousePosition(obstaclesUi,mousePosition),_musicController)); // A modifier avec un out 
                    }
                }
                else if (Event.current.button == 1)
                {
                    Vector2 mousePosition = Event.current.mousePosition;
                    if (ObstacleOnMousePosition(obstaclesUi,mousePosition) != null)
                    {
                        _popupRect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 200, 100);

                        
                        PopupWindow.Show(_popupRect,new DirectionPopup(ObstacleOnMousePosition(obstaclesUi, mousePosition).obstacleIndex,_musicController)); // A modifier avec un out 
                    }
                }
            }
            
            EditorUtility.SetDirty(_musicController);
        }
        
        
        
        GUI.EndScrollView();

        
        
        

    }

    public override void SaveChanges()
    {
        base.SaveChanges();
    }

    private ObstacleUI ObstacleOnMousePosition(List<ObstacleUI> checkList,Vector2 mousePos)
    {
        foreach (ObstacleUI obstacleUI in checkList)
        {
            if (mousePos.x >= obstacleUI.position.x && mousePos.x <= obstacleUI.position.x + obstacleUI.position.width)
            {
                if (mousePos.y >= obstacleUI.position.y && mousePos.y <= obstacleUI.position.y + obstacleUI.position.height)
                {
                    return obstacleUI;
                }
            }
        }

        return null;
    }
}
