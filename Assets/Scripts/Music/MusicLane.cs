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
    
    [SerializeField] private NoteName restriction;
    [SerializeField] private GameObject obstaclePrefab; 
    public float speed;
    private Transform[] _positions;
    
    private int _index;


    private void Start()
    {
        _positions = new Transform[transform.childCount];
        
        for (int i = 0; i < transform.childCount; i++)
        {
            _positions[i] = transform.GetChild(i);
        }
    }

    private void Update()
    {
        float distance = Vector2.Distance(_positions[0].position, _positions[1].position);
        float time = distance / speed;

        if (_index < _timeNotes.Count && MusicController.GetAudioSourceTime() >= _timeNotes[_index] - time)
        {
            Debug.Log("restriction " + restriction + " pos " + _positions[0].position);
            GameObject obstacle = Instantiate(obstaclePrefab, _positions[0].position, Quaternion.identity);
            obstacle.GetComponent<MusicObstacle>().currentLane = this;
            _index++;
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
                
                _timeNotes.Add(time);
            }
        }
        
    }
}
