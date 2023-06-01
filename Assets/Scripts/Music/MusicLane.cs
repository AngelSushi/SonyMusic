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
    private void Start()
    {
        _positions = new Transform[transform.childCount];
        
        for (int i = 0; i < transform.childCount; i++)
        {
            _positions[i] = transform.GetChild(i);
        }
        
        _controller = MusicController.instance;
    }

    private void Update()
    {
        float distance = Vector2.Distance(_positions[0].position, _positions[1].position);
        float time = distance / speed;

        if (_index < _timeNotes.Count && MusicController.GetAudioSourceTime() >= _timeNotes[_index] - time)
        {
            Debug.Log("spawn obstacle");

            GameObject obstacle = Instantiate(_controller.emptyObstacle, _positions[0].position, Quaternion.identity);
            
            obstacle.GetComponent<SpriteRenderer>().sprite = _controller.obstacles[_index].sprite;
            obstacle.GetComponent<SpriteRenderer>().color = _controller.obstacles[_index].color;
            obstacle.GetComponent<MusicObstacle>().dashDirection = _controller.obstacles[_index].direction;

            if (_endNotes[_index] > minLengthTime)
            {
                // ON sait que l'objet doit spawn a x secondes
                // on sait que l'objet a une vitesse de y 
                
                
                // v = d/t 
                // d = v * t
                
                float distanceToReach = (float)_endNotes[_index] * speed;
                obstacle.transform.localScale = new Vector3(distanceToReach, obstacle.transform.localScale.y, obstacle.transform.localScale.z);
            }
            
            obstacle.GetComponent<MusicObstacle>().currentLane = this;
            _index++;
            _controller.currentAllIndex++;
            
            Debug.Log(" " + _controller.currentAllIndex);
            
        }
    }

    public void SetTimeStamps(List<Note> notes) 
    {
        foreach (Note note in notes) 
        {
            Debug.Log("restriction " + note.NoteName);
            Debug.Log("file " + MusicController.midiFile.GetTempoMap());
            
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
