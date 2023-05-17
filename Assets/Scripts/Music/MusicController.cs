using System;
using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using UnityEngine;

public class MusicController : MonoBehaviour
{

    [SerializeField] private string fileLocation;
    
    private void Start()
    {
        MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
        allNotes = midiFile.GetNotes().ToList();

        foreach (NoteLane lane in lanes)
            lane.SetTimeStamps(allNotes);
    }
}
