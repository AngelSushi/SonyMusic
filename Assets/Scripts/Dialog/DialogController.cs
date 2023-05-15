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
        public string name;
        public int id;

        public Speaker(Texture2D speakerTex, string name, int id) 
        {
            this.speakerTex = speakerTex;
            this.speakerSprite = Sprite.Create(speakerTex,new Rect(0,0,speakerTex.width,speakerTex.height),new Vector2(0.5f,0.5f));
            this.name = name;
            this.id = id;
        }
    }

    public class DialogContent 
    {
        public int speakerID;
        public int dialogID;
        public int nextID;
        public string content;
        public float speed;
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
        if(speakers.Count == 0 && dialogs.Count == 0)
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
        Load(speakerFile,speakers);
        Load(dialogFile,dialogs);
        
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
            Texture2D speakerTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/Characters/" + content[i][0] + ".png");

            speakers.Add(new Speaker(speakerTex,content[i][1],int.Parse(content[i][2])));
        }
    }
    
    
    
    
}
