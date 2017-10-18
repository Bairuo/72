using System.Collections.Generic;
using UnityEngine;

public class BuffInvulnerable : Buff
{
    float duration;
    LinkedListNode<SubModifier> token;
    
    protected override void DatasetAdapt(BuffDataset x)
    {
        duration = x.invulnerableTime;
    }
    
    protected override void Begin()
    {
        token = this.gameObject.GetComponent<Unit>().defenceModifier.Add(0.0f, 1.0f, 10000f);
    }
    
    protected override void End()
    {
        if(token == null) return;
        this.gameObject.GetComponent<Unit>().defenceModifier.Remove(token);
    }
    
    protected override float timeLimit { get{ return duration; } }
}