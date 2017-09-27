// Created by DK 2017/9/26

using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    public JoyStick localJoyStick;
    public Body body;
    
    public float maxAngularVelocity;
    
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
        
        body.freezedRotation = true;
    }
    
    void Update()
    {
        Vector2 dir = localJoyStick.direction;
        
        if(Calc.eq(dir.x, 0f) && Calc.eq(dir.y, 0f) || localJoyStick.location.magnitude < 10f)
        {
            // do nothing if there's no action from the joystick.
            return;
        }
        
        float dira = Calc.Angle(Vector2.up, dir);
        float cura = Calc.RotationAngleZ(body.gameObject.transform.rotation);
        
        float deltadir = Calc.Angle(cura, dira);
        deltadir = Mathf.Sign(deltadir) * Mathf.Min(Mathf.Abs(deltadir), maxAngularVelocity * Time.deltaTime);
        
        body.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, (cura + deltadir) * Mathf.Rad2Deg);
        body.angularVelocity = deltadir / Time.deltaTime;
    }
    
}