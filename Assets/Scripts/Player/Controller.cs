// Created by DK 2017/9/26

using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    public JoyStick localJoyStick;
    public Body body;
    
    void Start()
    {
        if(localJoyStick == null) localJoyStick = GameObject.Find("JoyStick").GetComponent<JoyStick>();
        if(localJoyStick == null)
        {
            Debug.Log("WARNING: cannot find a joystick for script Controller.");
            Destroy(this);
            return;
        }
        
        body = this.gameObject.GetComponent<Body>();
        if(body == null)
        {
            Debug.Log("WARNING: cannot find a body component for script Controller.");
            Destroy(this);
            return;
        }
        
        
        
    }
    
    void Update()
    {
        
    }
    
}