using UnityEngine;
using System.Collections.Generic;

public struct SubModifier
{
    public float add;
    public float mult;
    public float flat;
    public SubModifier(float _add, float _mult, float _flat) { add = _add; mult = _mult; flat = _flat; }
}

public class Modifier
{
    LinkedList<SubModifier> modifier = new LinkedList<SubModifier>();
    
    public LinkedListNode<SubModifier> Add(float addToBase, float multiply = 1f, float addToFlat = 0f)
    {
        return modifier.AddFirst(new SubModifier(addToBase, multiply, addToFlat));
    }
    
    public void Remove(LinkedListNode<SubModifier> x)
    {
        modifier.Remove(x);
    }
    
    public float sumBase
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
    
    public float sumFlat
    {
        get
        {
            float x = 0f;
            foreach(var i in modifier) x += i.flat;
            return x;
        }
    }
    
    public float GetValue(float b)
    {
        return (b + sumBase) * mult + sumFlat;
    }
}