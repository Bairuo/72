using UnityEngine;
using System.Collections.Generic;

public struct SubModifier
{
    public float add;
    public float mult;
    public SubModifier(float _add, float _mult) { add = _add; mult = _mult; }
}

public class Modifier
{
    LinkedList<SubModifier> modifier = new LinkedList<SubModifier>();
    
    public LinkedListNode<SubModifier> Add(float add, float mult)
    {
        return modifier.AddFirst(new SubModifier(add, mult));
    }
    
    public void Remove(LinkedListNode<SubModifier> x)
    {
        modifier.Remove(x);
    }
    
    public float sum
    {
        get
        {
            float x = 0;
            foreach(var i in modifier) x += i.add;
            return x;
        }
    }
    
    public float mult
    {
        get
        {
            float x = 1f;
            foreach(var i in modifier) x *= i.mult;
            return x;
        }
    }
    
    public float GetValue(float b)
    {
        return (b + sum) * mult;
    }
}