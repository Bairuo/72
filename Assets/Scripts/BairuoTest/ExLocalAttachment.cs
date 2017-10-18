using UnityEngine;


public class ExLocalAttachment : MonoBehaviour
{
    
    
    /// A tagger to tag whether the game object is found.
    /// Once it has been found, the joysticks and other global things are attached to this Segment.
    bool _attachmentInited = false;
    
    public bool attachmentInited { get{ return _attachmentInited; } }
    
    
    protected virtual void Begin(GameObject x)
    {
        // default do nothing...
    }
    
    protected virtual void Update()
    {
        if(!_attachmentInited)
        {
            GameObject[] x = GameObject.FindGameObjectsWithTag("Player");
            foreach(var i in x)
            {
                ExNetworkBehaviour n = i.GetComponent<ExNetworkBehaviour>();
                if(n == null || !n.prepared || n.netObject.NetID != "Player-" + Client.instance.playerid) continue;
                Begin(i); // Will set the gameobject attach to this GamObject.
                _attachmentInited = true;
            }
        }
    }
    
    
    
}