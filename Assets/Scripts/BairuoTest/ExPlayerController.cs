// Created by DK 2017/9/26

using UnityEngine;
using System.Collections;


/// [!]WARNING: Network integrated!
/// Controller will find which Player Object belongs to this client/server.
public class ExPlayerController : ExNetworkBehaviour
{
    Body body;
    
    public JoyStick localJoyStick;
    public float maxAngularVelocity;
    
    // Controller will not reply for joystick movement which locations are less than this number.
    public float controlLimit;
    
    // variable to synchronize.
    public Vector2 joystickPosition;
    
    [SerializeField] int round = 0;
    
    
    protected override void Start()
    {
        base.Start();
        
        body = this.gameObject.GetComponent<Body>();
        
        
        AddProtocol("St", Pos, null, null, PosSync,typeof(Vector2), typeof(float));
        AddProtocol("Tar", null, Tar, TarSync, TarSync, typeof(Vector2));
        
        if(IsMyPlayer(this))
        {
            localJoyStick = Component.FindObjectOfType<JoyStick>();
        }
        else
        {
            localJoyStick = null;
        }
    }
    
    object[] Pos()
    {
        return new object[]{
            (Vector2)this.gameObject.transform.position,
            Calc.RotationAngleZ(this.gameObject.transform.rotation)};
    }
    
    void PosSync(object[] info)
    {
        this.gameObject.transform.position = (Vector2)info[0];
        this.gameObject.transform.rotation = Calc.GetQuaternion((float)info[1]);
    }
    
    object[] Tar(){ return new object[]{joystickPosition}; }
    void TarSync(object[] info){ joystickPosition = (Vector2)info[0]; }
    object[] Ang(){ return new object[]{Calc.RotationAngleZ(this.gameObject.transform.rotation)}; }
    
    void FixedUpdate()
    {
        // This filed requires synchronization from server.
        if(localJoyStick != null) joystickPosition = localJoyStick.location;
        
        Send("St");
        Send("Tar");
        
        round++;
        
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
    
    
    
    public static bool IsPlayer(ExNetworkBehaviour x)
    {
        return x.netObject.NetID.StartsWith("Player-");
    }
    
    public static bool IsMyPlayer(ExNetworkBehaviour x)
    {
        return x.netObject.NetID.StartsWith("Player-" + Client.instance.playerid);
    }
}