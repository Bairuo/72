using System.Collections.Generic;

public class BuffSpeedUp : Buff
{
    public const float massBonusMult = 1.1f;
    public const float speedBonusMult = 1.2f;
    
    LinkedListNode<SubModifier> massToken;
    LinkedListNode<SubModifier> speedToken;
    
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