using System;
using UnityEngine;
using System.Collections.Generic;

public class ExItemManager : MonoBehaviour
{
    
    int itc = 0; // item count.
    
    [Serializable]
    public class Data
    {
        public string path;
        public string name;
    }
    
    [SerializeField] public Data[] info;
    public SaftyArea saftyArea;
        
    public float generateDelay;
    
    float t = 0f;
    void Update()
    {
        if(!Client.IsRoomServer()) return; // clients do not deal with generating.
        
        // Generating rule:
        // Every item has a corresponding tagger.
        // Generate the tagger and the tagger will deal generating a true object.
        
        // Create an object that has a per 12s.
        t -= Time.deltaTime;
        if(t < 0f)
        {
            t += generateDelay;
            
            int g = Mathf.FloorToInt(UnityEngine.Random.Range(0f, info.Length));
            Data i = info[g];
            ExGameObjectCreator.instance.GlobalGameObjectCreate(
                i.path,
                i.name,
                Util.RandPos(Vector2.zero, saftyArea.radius * 1.2f),
                0f,
                i.name + (itc += 1));
        }
    }
    
}