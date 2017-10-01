using UnityEngine;

public class Buff : MonoBehaviour
{
    float t;
    
    void Start()
    {
        // DO NOT use begin in this section
        // since this script will execute immediately
        // in AddComponent().
    }
    
    bool applied = false;
    
    void FixedUpdate()
    {
        if(!applied)
        {
            Begin();
            applied = true;
        }
        
        Process();
        t += Time.fixedDeltaTime; 
        if(t > timeLimit) Destroy(this);
    }
    
    void OnDestroy()
    {
        End();
    }
    
    protected virtual void Begin() { }
    protected virtual void End() { }
    protected virtual void Process() { }
    protected virtual float timeLimit { get{ return 0f; } }
}

