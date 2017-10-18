using UnityEngine;
using System;


[Serializable]
public class ItemGenerationInfo
{
    public GameObject source;
    public float deltaTime;
}



/// Global item generator.
public class ItemGenerator : ExNetworkBehaviour
{
    [SerializeField] public ItemGenerationInfo[] info;
    
    
    void Update()
    {
        
    }
    
}