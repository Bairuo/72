using System.Collections.Generic;
using UnityEngine;


public class BuffAddDefence : Buff
{
    /// Value might be changed if BuffDataset exists.
    public float bonus = 8f;
    
    LinkedListNode<SubModifier> token;
    
    protected override void DatasetAdapt(BuffDataset x)
    {
        bonus = x.defenceAdd;
    }
    
    protected override void Begin()
    {
        Unit unit = this.gameObject.GetComponent<Unit>();
        if(unit == null)
        {
            Destroy(this);
            return;
        }
        
        token = unit.defenceModifier.Add(0f, bonus, 0f);
    }
    
    protected override void End()
    {
        if(token == null) return;
        this.gameObject.GetComponent<Unit>().defenceModifier.Remove(token);
    }
    
    protected override float timeLimit { get{ return 1e20f; } }
}