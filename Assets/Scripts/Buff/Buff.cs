using UnityEngine;

public abstract class Buff : ExNetworkComponent
{
    float t;
    
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
    
    protected override void OnDestroy()
    {
        End();
        base.OnDestroy();
    }
    
    /// this function will run if a dataset is found.
    protected virtual void DatasetAdapt(BuffDataset x){ }
    
    protected virtual void Begin() { }
    protected virtual void End() { }
    protected virtual void Process() { }
    protected abstract float timeLimit{ get; }
}

