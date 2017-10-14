using UnityEngine;

public abstract class Buff : MonoBehaviour
{
    float t;
    
    void Start()
    {
        // DO NOT use begin in this section
        // since this script will execute immediately
        // in AddComponent().
    }
    
    bool applied = false;
    
    GameObject dataset
    {
        get
        {
            return GameObject.FindGameObjectWithTag("BuffDataset");
        }
    }
    
    void FixedUpdate()
    {
        if(!applied)
        {
            if(dataset != null)
            {
                DatasetAdapt(dataset.GetComponent<BuffDataset>());
            }
            
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
    
    /// this function will run if a dataset is found.
    protected virtual void DatasetAdapt(BuffDataset x){ }
    
    protected virtual void Begin() { }
    protected virtual void End() { }
    protected virtual void Process() { }
    protected abstract float timeLimit{ get; }
}

