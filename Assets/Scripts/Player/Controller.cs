// Created by DK 2017/9/26

using UnityEngine;
using System.Collections;

public class Controller : NetworkBehaviour
{
    public JoyStick localJoyStick;
    public Body body;
    
    public float maxAngularVelocity;
    
    // Controller will not reply for joystick movement which locations are less than this number.
    public float controlLimit;
    
    // Each controller has a unique Name in a scene (on a server or a client).
    protected override string networkName{ get{ return this.gameObject.name + "Controller"; } }
    
    protected override void Start()
    {
        base.Start();
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
    
    void FixedUpdate()
    {
        // This filed requires synchronization from clients.
        
        // Server directly use localJoystick.
        if(Client.IsRoomServer())
        {
            DirectionControl(localJoyStick.location);
            Debug.Log("Server control." + networkName);
        }
        else
        {
            // Clients sends messages to server to synchronize.
            Send(localJoyStick.location);
            Debug.Log("Client control." + networkName);
        }
    }
    
    
    GameObject[] players;
    public override void NetworkCallback()
    {
        Vector2 loc = GetVec2();
        Debug.Log("Server receive message : " + loc + " | " + networkName);
        DirectionControl(loc);
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