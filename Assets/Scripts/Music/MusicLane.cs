using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using Unity.VisualScripting;
using UnityEngine;
using Note = Melanchall.DryWetMidi.Interaction.Note;

public class MusicLane : MonoBehaviour
{

    private List<Note> _laneNotes = new List<Note>();
    private List<double> _timeNotes = new List<double>();
    
    [SerializeField] private NoteName restriction;

    public void SetTimeStamps(List<Note> notes) 
    {
        foreach (Note note in notes) 
        {
            if (note.NoteName == restriction) 
            {
                var convertTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, MusicController.midiFile.GetTempoMap());
                double time = (double)convertTime.Minutes * 60f + convertTime.Seconds + (double)convertTime.Milliseconds / 1000f;
                
                _timeNotes.Add(time);
                _laneNotes.Add(note);
            }
        }
        
    }
}
