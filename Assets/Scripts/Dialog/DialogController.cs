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

    public void ChangeScene(string sceneName)
    {
        GameManager.instance.ChangeSceneWithAnim(sceneName);
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

        int speakerID = -1;
        int dialogID = -1;
        int nextID = -1;
        float speed = 0f;
        
        for (int i = 1; i < content.Length; i++) 
        {
            
            int.TryParse(content[i][0],out speakerID);
            int.TryParse(content[i][1], out dialogID);
            int.TryParse(content[i][2], out nextID);
            float.TryParse(content[i][4],out speed);
            
            Debug.Log("speakerID " + speakerID);
            
            loadList.Add(new DialogContent(speakerID,dialogID,nextID,content[i][3],speed));
        }
    }
    
    public void Load(TextAsset file,List<Speaker> loadList) 
    {
        loadList.Clear();
        string[][] content = CsvParser.Parse(file.text);
        
        for (int i = 1; i < content.Length; i++) {

            Texture2D speakerTex = Resources.Load<Texture2D>("Characters/" + content[i][1] + "");
            Texture2D backgroundTex = Resources.Load<Texture2D>("Backgrounds/" + content[i][2] + "");

            loadList.Add(new Speaker(speakerTex,backgroundTex,content[i][3],int.Parse(content[i][0])));
        
        }
    }
    
    
    
    
}
