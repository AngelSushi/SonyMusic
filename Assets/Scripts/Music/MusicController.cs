
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;
using UnityEngine.Networking;


[System.Serializable]
public class Obstacle
{
    public Sprite sprite;
    public Color color;
    public DashDirection direction;

    public Obstacle(Sprite sprite, Color color, DashDirection direction)
    {
        this.sprite = sprite;
        this.color = color;
        this.direction = direction;
    }
}


public class MusicController : MonoBehaviour
{

    public static MusicController instance;
    [SerializeField] private string fileLocation;
    public MusicLane[] lanes;
    public AudioSource mainAudio;

    public List<Obstacle> obstacles = new List<Obstacle>();
    [HideInInspector] public int currentAllIndex;


    public GameObject emptyObstacle;
    public static MidiFile midiFile;

    public List<Note> allNotes;


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
                    LoadAndroid(www);
                }
                else
                {
                    Debug.LogError("Erreur lors du chargement du fichier MIDI : " + www.error);
                }
            }
        }
        else
        {
            LoadWindows();
        }
        
    }



    public void LoadWindows()
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
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

    private void LoadAndroid(UnityWebRequest www)
    {
        midiFile = MidiFile.Read(new MemoryStream(www.downloadHandler.data));
        allNotes = midiFile.GetNotes().ToList();

        if (obstacles.Count != allNotes.Count)
        {
            Debug.LogError("Le nombre d'obstacles pré-défini est incorrect. La taille est de " + allNotes.Count);
        }

                
        foreach (MusicLane lane in lanes)
        {
            lane.SetTimeStamps(allNotes);
        }
    }

    public static double GetAudioSourceTime()
    {
        return (double)instance.mainAudio.timeSamples / instance.mainAudio.clip.frequency;
    }


}
