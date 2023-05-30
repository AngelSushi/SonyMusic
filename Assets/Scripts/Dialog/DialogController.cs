using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class DialogController : MonoBehaviour 
{
    public class Speaker 
    {
        public Texture2D speakerTex;
        public Sprite speakerSprite;
        public Texture2D backgroundTex;
        public Sprite backgroundSprite;
        public string name;
        public int id;

        public Speaker(Texture2D speakerTex,Texture2D backgroundTex, string name, int id) 
        {
            this.speakerTex = speakerTex;
            speakerSprite = Sprite.Create(speakerTex,new Rect(0,0,speakerTex.width,speakerTex.height),new Vector2(0.5f,0.5f));
            this.backgroundTex = backgroundTex;
            backgroundSprite = Sprite.Create(backgroundTex,new Rect(0,0,backgroundTex.width,backgroundTex.height),new Vector2(0.5f,0.5f));
            this.name = name;
            this.id = id;
        }
    }

    [System.Serializable]
    public class DialogContent 
    {
        public int speakerID;
        public int dialogID;
        public int nextID;
        public string content;
        public float speed;
        public UnityEvent beginAction;
        public UnityEvent endAction;
        
        public DialogContent(int speakerID, int dialogID, int nextID, string content, float speed) 
        {
            this.speakerID = speakerID;
            this.dialogID = dialogID;
            this.nextID = nextID;
            this.content = content;
            this.speed = speed;
        }
    }

    public TextAsset speakerFile;
    public TextAsset dialogFile;

    public List<DialogContent> dialogs = new List<DialogContent>();
    public List<Speaker> speakers = new List<Speaker>();

    public static DialogController instance;

    public void OnEnable() 
    {
        if (!Application.isPlaying) 
        {
            LoadFiles();
        }
    }

    private void Awake() 
    {
        LoadFiles();
        instance = this;
    }

    public DialogContent GetDialogById(int dialogID) 
    {
        return dialogs.Where(dialog => dialog.dialogID == dialogID).ToList()[0];
    }

    public Speaker GetSpeakerById(int speakerID) 
    {
        return speakers.Where(speaker => speaker.id == speakerID).ToList()[0];
    }


    [ContextMenu("Load")]
    private void LoadFiles() 
    {
        if (speakerFile != null)
        {
            Load(speakerFile,speakers);   
        }
        else
        {
            Debug.Log("speaker file is null");
        }

        if (dialogFile != null)
        {
            Load(dialogFile,dialogs);   
        }
        else
        {
            Debug.Log("dialog file is null");
        }
        
    }
    
    public void Load(TextAsset file,List<DialogContent> loadList) 
    {
        loadList.Clear();
        string[][] content = CsvParser.Parse(file.text);
        
        for (int i = 1; i < content.Length; i++) 
        {
            loadList.Add(new DialogContent(int.Parse(content[i][0]),int.Parse(content[i][1]),int.Parse(content[i][2]),content[i][3],float.Parse(content[i][4])));
        }
    }
    
    public void Load(TextAsset file,List<Speaker> loadList) 
    {
        loadList.Clear();
        string[][] content = CsvParser.Parse(file.text);
        
        for (int i = 1; i < content.Length; i++) {

            Texture2D speakerTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/Characters/" + content[i][1] + ".png");
            Texture2D backgroundTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/Backgrounds/" + content[i][2] + ".png");
            
            Debug.Log("speakerTex " + content[i][1] + " backgroundTex " +  content[i][2]);
            Debug.Log("tex " + speakerTex + " btex " + backgroundTex);
            
            loadList.Add(new Speaker(speakerTex,backgroundTex,content[i][3],int.Parse(content[i][0])));
            
            
            Debug.Log("length " + i);
        }
    }
    
    
    
    
}
