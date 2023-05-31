
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using SmfLite;
using UnityEngine;
using UnityEngine.Networking;
using MidiEvent = SmfLite.MidiEvent;


public static class StringExtension
{
    public static Stream ToStream(this string value, Encoding encoding)
        => new MemoryStream(encoding.GetBytes(value ?? string.Empty));

}


public class MusicController : MonoBehaviour
{

    public static MusicController instance;
    [SerializeField] private string fileLocation;
    [SerializeField] private MusicLane[] lanes;
    [SerializeField] private AudioSource mainAudio;
    [SerializeField] private int bpm;

    public List<GameObject> obstacles = new List<GameObject>();
    public int currentAllIndex;


    public static MidiFile midiFile;

    private List<Note> allNotes;

    public MidiTrackSequencer _trackSequencer;

    [Range(0,2)]
    public int index;


    private void Awake()
    {
        instance = this;
    }
    
    private void Start()
    {
        string fullPath;

        if (Application.platform == RuntimePlatform.Android)
        {
            fullPath = Path.Combine(Application.streamingAssetsPath, fileLocation);
        }
        else
        {
            fullPath = Path.Combine("file://" + Application.streamingAssetsPath, fileLocation);
        }

        Debug.Log("start");
        StartCoroutine(LoadMidiFile(fullPath));
    }

    private IEnumerator LoadMidiFile(string fullPath)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(fullPath))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    midiFile = MidiFile.Read(new MemoryStream(www.downloadHandler.data));
                    allNotes = midiFile.GetNotes().ToList();

                    if (obstacles.Count != allNotes.Count)
                    {
                        Debug.LogError("Le nombre d'obstacles pré-défini est incorrect. La taille est de " + allNotes.Count);
                    }

                
                    Debug.Log("size " + allNotes.Count);
                    foreach (MusicLane lane in lanes)
                    {
                        lane.SetTimeStamps(allNotes);
                    }
                }
                else
                {
                    Debug.LogError("Erreur lors du chargement du fichier MIDI : " + www.error);
                }
            }
        }
        else
        {
            midiFile = MidiFile.Read(fullPath);
            
        }
        
        Debug.Log("testeee");
    }
    
    
    
    /* private void Start()
    {
        Debug.Log("loo " + Application.persistentDataPath);
        string fullPath = Path.Combine(Application.streamingAssetsPath, fileLocation);

        // Charger le fichier MIDI
        Debug.Log("fullPath " + fullPath);
        midiFile = MidiFile.Read(fullPath);
        
        
        */
       // midiFile = MidiFile.Read( Application.persistentDataPath + "/Assets/" + fileLocation);
        //    allNotes = midiFile.GetNotes().ToList();

        //    if (obstacles.Count != allNotes.Count)
        //    {
        //        Debug.LogError("Le nombre d'obstacles pré-défini est incorrect. La taille est de " + allNotes.Count);
        //    }

        //    foreach (MusicLane lane in lanes)
        //    {
        //        lane.SetTimeStamps(allNotes);
        //    }
        
        
        
       /* var loadingRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, fileLocation));
        loadingRequest.SendWebRequest();
        while (!loadingRequest.isDone)
        {
            if (loadingRequest.isNetworkError || loadingRequest.isHttpError)
            {
                break;
            }
            yield return null;
        }
        
        
        MidiFileContainer song = MidiFileLoader.Load (loadingRequest.downloadHandler.data);
 
        Debug.Log("here");
       // yield return new WaitForSeconds(1f);
       
       Debug.Log("tracks length " + song.tracks.Count);

       _trackSequencer = new MidiTrackSequencer(song.tracks[1], song.division,bpm);

        _trackSequencer.Start();
        
        */

   // }

    private void Update()
    {
       /* if (_trackSequencer != null && _trackSequencer.Playing)
        {
            Debug.Log("c " + " track" + _trackSequencer);
            Debug.Log("Length " + _trackSequencer.Advance(Time.deltaTime).Count);

            foreach (MidiEvent e in _trackSequencer.Advance(0))
            {
                if (e.status != null)
                {
                    Debug.Log("e " + (int)e.data1 );
                }
            }
        }
        */
    }

    public static double GetAudioSourceTime()
    {
        return (double)instance.mainAudio.timeSamples / instance.mainAudio.clip.frequency;
    }


}
