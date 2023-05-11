using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour {
    public class Speaker {
        public Sprite speakerSprite;
        public string name;
        public int id;

        public Speaker(Sprite speakerSprite, string name, int id) {
            this.speakerSprite = speakerSprite;
            this.name = name;
            this.id = id;
        }
    }

    public class DialogContent {
        public int speakerID;
        public int dialogID;
        public int lastID;
        public AudioClip voice;
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

    public TextAsset csvFile;

    public List<DialogContent> contents = new List<DialogContent>();

    public void Start() {
        Load(csvFile);
    }

    public void Load(TextAsset file) {
        string[][] content = CsvParser.Parse(file.text);
        
        
        for (int i = 1; i < content.Length; i++) {
            string[] data = content[i][0].Split(";");
            contents.Add(new DialogContent(int.Parse(data[0]),int.Parse(data[1]),int.Parse(data[2]),data[4],float.Parse(data[5])));
        }
        

        Debug.Log("length " + contents.Count);
    }
}
