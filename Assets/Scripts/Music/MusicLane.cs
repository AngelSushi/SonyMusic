using System;
using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using Unity.VisualScripting;
using UnityEngine;
using Note = Melanchall.DryWetMidi.Interaction.Note;

public class MusicLane : MonoBehaviour
{

    private List<double> _timeNotes = new List<double>();
    private List<double> _endNotes = new List<double>();
    
    [SerializeField] private NoteName restriction; 
    public float speed;
    private Transform[] _positions;
    
    private int _index;
    
    [SerializeField] [Tooltip("The minimum length of an obstacle to be resize ")]private float minLengthTime;

    private MusicController _controller;
    public ObstaclePool lanePool;
    
    
    private void Start()
    {
        _positions = new Transform[transform.childCount];
        
        for (int i = 0; i < transform.childCount; i++)
        {
            _positions[i] = transform.GetChild(i);
        }
        
        _controller = MusicController.instance;
        lanePool = GetComponent<ObstaclePool>();
    }

    private void Update()
    {
        float distance = Vector2.Distance(_positions[0].position, _positions[1].position);
        float time = distance / speed;
        

        if (_index < _timeNotes.Count && MusicController.GetAudioSourceTime() >= _timeNotes[_index] - time)
        {
            GameObject obstacle = lanePool.pool.Get();
            obstacle.transform.parent = transform;
            
            obstacle.GetComponent<SpriteRenderer>().sprite = _controller.obstacles[_controller.currentAllIndex].sprite;
            obstacle.GetComponent<SpriteRenderer>().color = _controller.obstacles[_controller.currentAllIndex].color;
            
            obstacle.GetComponent<MusicObstacle>().dashDirection = _controller.obstacles[_controller.currentAllIndex].direction;
            obstacle.GetComponent<MusicObstacle>().currentLane = this;
            
            _index++;
            _controller.currentAllIndex++;
        }
    }

    public void SetTimeStamps(List<Note> notes) 
    {
        foreach (Note note in notes) 
        {
            Debug.Log("restriction " + note.NoteName);
            
            if (note.NoteName == restriction) 
            {
                var convertTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, MusicController.midiFile.GetTempoMap());
                double time = (double)convertTime.Minutes * 60f + convertTime.Seconds + (double)convertTime.Milliseconds / 1000f;
                
                
                var convertLengthTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Length, MusicController.midiFile.GetTempoMap());
                double lengthTime = (double)convertLengthTime.Minutes * 60f + convertLengthTime.Seconds + (double)convertLengthTime.Milliseconds / 1000f;

                _timeNotes.Add(time);
                _endNotes.Add(lengthTime);
                
            }
        }
        
    }
    
}
