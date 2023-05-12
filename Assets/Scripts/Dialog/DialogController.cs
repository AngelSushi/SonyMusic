using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogController : MonoBehaviour {
    public class Speaker {
        public Texture speakerTex;
        public string name;
        public int id;

        public Speaker(Texture speakerTex, string name, int id) {
            this.speakerTex = speakerTex;
            this.name = name;
            this.id = id;
        }
    }

    public class DialogContent {
        public int speakerID;
        public int dialogID;
        public int lastID;
        public string content;
        public float speed;
        
        public DialogContent(int speakerID, int dialogID, int lastID, string content, float speed) {
            this.speakerID = speakerID;
            this.dialogID = dialogID;
            this.lastID = lastID;
            //this.voice = voice;
            this.content = content;
            this.speed = speed;
        }
    }

    public TextAsset speakerFile;
    public TextAsset dialogFile;

    public List<DialogContent> dialogs = new List<DialogContent>();
    public List<Speaker> speakers = new List<Speaker>();

    [ContextMenu("Load")]
    private void LoadFiles() {
        Load(speakerFile,speakers);
        Load(dialogFile,dialogs);
    }
    
    public void Load(TextAsset file,List<DialogContent> loadList) {
        loadList.Clear();
        string[][] content = CsvParser.Parse(file.text);
        
        
        for (int i = 1; i < content.Length; i++) {
            loadList.Add(new DialogContent(int.Parse(content[i][0]),int.Parse(content[i][1]),int.Parse(content[i][2]),content[i][3],float.Parse(content[i][4])));
        }
        

        Debug.Log("length " + loadList.Count);
    }
    
    public void Load(TextAsset file,List<Speaker> loadList) {
        loadList.Clear();
        string[][] content = CsvParser.Parse(file.text);
        
        
        for (int i = 1; i < content.Length; i++) {
            Texture speakerTex = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Resources/Characters/" + content[i][0] + ".png");
            speakers.Add(new Speaker(speakerTex,content[i][1],int.Parse(content[i][2])));
        }
        

        Debug.Log("length " + loadList.Count);
    }
}
