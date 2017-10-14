using System.Collections.Generic;
using UnityEngine;

public class BuffSpeedUp : Buff
{
    /// Values might be changed if BUffDataset exists.
    public float massBonusMult = 1.2f;
    public float speedBonusMult = 1.5f;
    
    LinkedListNode<SubModifier> massToken;
    LinkedListNode<SubModifier> speedToken;
    
    protected override void DatasetAdapt(BuffDataset x)
    {
        massBonusMult = x.speedupMassMult;
        speedBonusMult = x.speedupThrustMult;
    }
    
    protected override void Begin()
    {
        Unit unit = this.gameObject.GetComponent<Unit>();
        if(unit == null)
        {
            Destroy(this);
            return;
        }
        
        Thrust thrust = this.gameObject.GetComponent<Thrust>();
        if(thrust == null)
        {
            Destroy(this);
            return;
        }
        
        massToken = unit.massModifier.Add(0f, massBonusMult);
        speedToken = thrust.thrustModifier.Add(0f, speedBonusMult);
    }
    
    protected override void End()
    {
        if(massToken == null || speedToken == null) return;
        this.gameObject.GetComponent<Unit>().massModifier.Remove(massToken);
        this.gameObject.GetComponent<Thrust>().thrustModifier.Remove(speedToken);
    }
    
    protected override float timeLimit { get{ return 1e20f; } }
}