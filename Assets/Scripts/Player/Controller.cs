// Created by DK 2017/9/26

using UnityEngine;
using System.Collections;


/// [!]WARNING: Network integrated!
/// Controller will find which Player Object belongs to this client/server.
public class Controller : ExLocalAttachment
{
    public JoyStick localJoyStick;
    public Body body;
    
    public float maxAngularVelocity;
    
    // Controller will not reply for joystick movement which locations are less than this number.
    public float controlLimit;
    
    public Vector2 joystickPosition;
    
    protected override void Begin(GameObject x)
    {
        if(localJoyStick == null) localJoyStick = GameObject.Find("JoyStick").GetComponent<JoyStick>();
        if(localJoyStick == null)
        {
            Debug.Log("WARNING: cannot find a joystick for script Controller.");
            Destroy(this);
            return;
        }
        
        body = x.GetComponent<Body>();
        if(body == null)
        {
            Debug.Log("WARNING: cannot find a body component for script Controller.");
            Destroy(this);
            return;
        }
    }
    
    void FixedUpdate()
    {
        if(!attachmentInited) return;
        
        
        // This filed requires synchronization from server.
        if(Client.IsRoomServer()) joystickPosition = localJoyStick.location;
        else
        {
            
        }
        DirectionControl(joystickPosition);
    }
    
    void DirectionControl(Vector2 targetDirection)
    {
        if(targetDirection.magnitude < controlLimit)
        {
            // do nothing if there's no action from the joystick.
            return;
        }
        
        float dira = Calc.Angle(Vector2.up, targetDirection);
        float cura = Calc.RotationAngleZ(body.gameObject.transform.rotation);
        
        float deltadir = Calc.Angle(cura, dira);
        deltadir = Mathf.Sign(deltadir) * Mathf.Min(Mathf.Abs(deltadir), maxAngularVelocity * Time.fixedDeltaTime);
        
        body.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, (cura + deltadir) * Mathf.Rad2Deg);
        body.angularVelocity = deltadir / Time.fixedDeltaTime;
    }
    
}