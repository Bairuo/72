// Created by DK 2017/9/26

using UnityEngine;
using System.Collections;

/// Provides a linked skill, provides a forward speed for a specific time.
public class BurnDriveDevice : MonoBehaviour
{
    static int cnt;
    
    public Body body;
    
    public float massAdd;
    public float massMult;
    
    public float thrustAdd;
    public float thrustMult;
    
    public float applyTime;
    
    public float cooldown;
    
    void Start()
    {
        if(body == null) body = this.gameObject.GetComponent<Body>();
        if(body == null)
        {
            Debug.Log("WARNING: missing body component required by BurnDriveDevice.");
            Destroy(this);
            return;
        }
        
        t = 0;
    }
    
    void Update()
    {
        // DEBUG SECTION =>
        if(Input.GetKey(KeyCode.G)) TryBurnDrive();
        // <= DEBUG SECTION.
    }
    
    [SerializeField] float t;
    void FixedUpdate()
    {
        t -= Time.fixedDeltaTime;
        if(t < 0f) t = 0f;
    }
    
    void TryBurnDrive()
    {
        if(t <= 0f)
        {
            BurnDriveApply();
        }
    }
    
    void BurnDriveApply()
    {
        var c = ExComponentGenerator.instance.CreateComponentAt("BuffBurndrive", this.gameObject, "Burndrive" + (cnt += 1)) as Burndrive;
        c.massAdd = massAdd;
        c.massMult = massMult;
        c.thrustAdd = thrustAdd;
        c.thrustMult = thrustMult;
        c.time = applyTime;
        
        t += cooldown;
    }
}