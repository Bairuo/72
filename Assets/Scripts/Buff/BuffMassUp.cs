using System.Collections.Generic;

public class BuffMassUp : Buff
{
    public const float bonus = 8f;
    
    LinkedListNode<SubModifier> token;
    
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