using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

public class MusicController : MonoBehaviour
{

    public static MusicController instance;
    [SerializeField] private string fileLocation;
    [SerializeField] private MusicLane[] lanes;
    [SerializeField] private AudioSource mainAudio;
    
    public static MidiFile midiFile;

    private List<Note> allNotes;


    private void Awake()
    {
        instance = this;
    }
    
    private void Start()
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
        allNotes = midiFile.GetNotes().ToList();

        foreach (MusicLane lane in lanes)
        {
            lane.SetTimeStamps(allNotes);
        }
  
    }
    
    public static double GetAudioSourceTime() {
        return (double)instance.mainAudio.timeSamples / instance.mainAudio.clip.frequency;
    }
    
    
}
