using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

public class MusicController : MonoBehaviour
{

    [SerializeField] private string fileLocation;
    [SerializeField] private MusicLane[] lanes;

    public static MidiFile midiFile;

    private List<Note> allNotes;
    
    private void Start()
    {
/*        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
        allNotes = midiFile.GetNotes().ToList();

        foreach (MusicLane lane in lanes)
        {
            lane.SetTimeStamps(allNotes);
        }
  */
    }
    
    
}
