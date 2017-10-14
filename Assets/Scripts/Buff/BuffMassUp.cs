using System.Collections.Generic;
using UnityEngine;


public class BuffMassUp : Buff
{
    /// Value might be changed if BuffDataset exists.
    public float bonus = 8f;
    
    LinkedListNode<SubModifier> token;
    
    protected override void DatasetAdapt(BuffDataset x)
    {
        bonus = x.massupMassAdd;
    }
    
    protected override void Begin()
    {
        Unit unit = this.gameObject.GetComponent<Unit>();
        if(unit == null)
        {
            Destroy(this);
            return;
        }
        
        token = unit.massModifier.Add(bonus);
    }
    
    protected override void End()
    {
        if(token == null) return;
        this.gameObject.GetComponent<Unit>().massModifier.Remove(token);
    }
    
    protected override float timeLimit { get{ return 1e20f; } }
}