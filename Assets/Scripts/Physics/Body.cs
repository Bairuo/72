using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

public class Body : MonoBehaviour
{
    /// Linear drag, apply for direct movement.
    [Range(0f, 1f)] public float baseDrag = 0f;
    public Modifier dragModifier = new Modifier();
    public float drag { get{ return dragModifier.GetValue(baseDrag); } }
    
    /// Side linear drag, apply for side movement.
    [Range(0f, 1f)] public float baseSideDrag = 0f;
    public Modifier sideDragModifier = new Modifier();
    public float sideDrag { get{ return sideDragModifier.GetValue(baseSideDrag); } }
    
    /// Angular drag.
    [Range(0f, 1f)] public float baseAngularDrag = 0f;
    public Modifier angularDragModifier = new Modifier();
    public float angularDrag{ get{ return angularDragModifier.GetValue(baseAngularDrag); } }
    
    /// Velocity.
    public Vector2 velocity = new Vector2(0f, 0f);
    
    /// Angular velocity in radians. Positive counter-clockwise.
    public float angularVelocity = 0f;
    
    public float baseMass;
    public Modifier massModifier = new Modifier();
    public float mass { get{ return massModifier.GetValue(baseMass); } }
    
    /// Moment of inertia, which defines by delta(AngluarVelocity) = delta(t) * MOI.
    public float baseMOI;
    public Modifier MOIModifier = new Modifier();
    public float MOI{ get{ return MOIModifier.GetValue(baseMOI); } }
    
    /// Hardness defines the Coefficient of Restitution in collision.
    /// The formula is COR = 1 - (1 - this.hardness) * (1 - other.hardness).
    /// When a hardness is larger than 1, the COR will be greater than 1 so that
    ///   the collision generates energy for colliders.
    public float baseHardness;
    public Modifier hardnessModifier = new Modifier();
    public float hardness { get{ return hardnessModifier.GetValue(baseHardness); } }
    
    /// A force that should be applied if two objects overlapped.
    public float seperationForce;
    
    /// Freezing.
    /// Velocity will not change by this script as well
    ///   if those freezing options are enabled.
    public bool freezedX = false;
    public bool freezedY = false;
    public bool freezedRotation = false;
    public bool freezedTranslation
    {
        get{ return freezedX && freezedY; }
        set{ freezedX = freezedY = value; }
    }
    public bool freezedAll
    {
        get{ return freezedTranslation && freezedRotation; }
        set{ freezedTranslation = freezedRotation = value; }
    }
    
    /// The max distance from collider polygon and object's center.
    [HideInInspector] public float farDist;
    
    LinkedList<Force> forces = new LinkedList<Force>();
    LinkedList<Torque> torques = new LinkedList<Torque>();
    
    [HideInInspector] public PolygonCollider2D col;
    
    /// Points extract from colliders.
    [HideInInspector] public Vector2[] points;
    
    /// From which path of collider shall I extract the collider points.
    public int pathID;
    
    /// callback functions are defined here.
    public delegate void CollisionCallbackType(Body other, Vector2 impulse);
    public CollisionCallbackType collisionCallback;
    
// ============================================================================================
// ============================================================================================
// ============================================================================================
    
    void Start()
    {
        collisionCallback += (Body _, Vector2 __) => { }; // Invoking a delegate must not empty.
        
        col = this.gameObject.GetComponent<PolygonCollider2D>();
        
        PhysWorld.bodies.Add(this);
        
        farDist = 0f;
        foreach(var i in col.points) farDist = Mathf.Max(farDist, i.magnitude);
        
        points = col.GetPath(pathID);
    }
    
    void OnDestroy()
    {
        PhysWorld.bodies.Remove(this);
    }
    
    void Update()
    {
    }
    
    void FixedUpdate()
    {
        
    }
    
// ============================================================================================
// ============================================================================================
// ============================================================================================
    
    public void StepTranslation(float timestep)
    {
        // ===================== Accelleration ========================
        
        Force sf = new Force();
        foreach(Force i in forces)
        {
            sf.value += i.value * Calc.RelativeCut(i.timelast, timestep);
            i.timelast -= timestep;
        }
        var x = forces.First;
        while(x != null)
        {
            var y = x.Next;
            if(x.Value.timelast <= 0f) forces.Remove(x);
            x = y; 
        }
        Vector2 acce = this.gameObject.transform.rotation * sf.value / mass;
        
        // Velocity relative to world.
        velocity += acce * timestep;
        if(freezedX) velocity.x = 0f;
        if(freezedY) velocity.y = 0f;
        
        // ========================== Drag ============================
        
        Quaternion currot = this.gameObject.transform.rotation;
        Vector2 vForward = Calc.Projection(velocity, currot * Vector2.up);
        Vector2 vSide = Calc.Projection(velocity, currot * Vector2.right);
        vForward *= Mathf.Pow(1 - drag, timestep);
        vSide *= Mathf.Pow(1 - sideDrag, timestep);
        velocity = vForward + vSide;
        if(freezedX) velocity.x = 0f;
        if(freezedY) velocity.y = 0f;
        
        // ==================== Move Simulation =======================
        
        Vector2 deltapos = velocity * timestep - 0.5f * acce * timestep * timestep;
        
        this.gameObject.transform.position = (Vector2)this.gameObject.transform.position + deltapos;
    }
    
    public void StepRotation(float timestep)
    {
        // ===================== Accelleration ========================
        
        Torque sq = new Torque();
        foreach(var i in torques)
        {
            sq.value += i.value * Calc.RelativeCut(i.timelast, timestep);
            i.timelast -= timestep;
        }
        var x = torques.First;
        while(x != null)
        {
            var y = x.Next;
            if(x.Value.timelast <= 0f) torques.Remove(x);
            x = y; 
        }
        float angacce = sq.value / MOI;
        angularVelocity += angacce * timestep;
        
        if(freezedRotation) angularVelocity = 0f;
        
        // ========================== Drag ============================
        
        angularVelocity *= Mathf.Pow(1 - angularDrag, timestep);
        
        if(freezedRotation) angularVelocity = 0f;
        
        // ==================== Move Simulation =======================
        
        float deltadir = angularVelocity - 0.5f * angacce * timestep * timestep;
        if((deltadir > 0 && angularVelocity < 0) || (deltadir < 0 && angularVelocity > 0))
            deltadir = 0f;
        
        if(freezedRotation) deltadir = 0f;
        this.gameObject.transform.Rotate(0f, 0f, deltadir);
        
    }
    
    public void StepMovement(float timestep)
    {
        StepTranslation(timestep);
        StepRotation(timestep);
    }
    
    /// @force RELATIVE direction.
    /// @loc RELATIVE location where the force is applied.
    /// @time time the force applied.
    /// The force that applied will change direction with game object's rotation.
    public ForceToken AddForce(Vector2 force, Vector2 loc, float time)
    {
        return new ForceToken(
            forces.AddFirst(new Force(force, time)), 
            torques.AddFirst(new Torque(Calc.CrossMultiply(loc, force), time)));
    }
    
    public bool RemoveForce(ForceToken token)
    {
        forces.Remove(token.forceToken);
        torques.Remove(token.torqueToken);
        return true;
    }
    
    /// @torque
    /// @ time time the torque applied.
    public TorqueToken AddTorque(float torque, float time)
    {
        return new TorqueToken(torques.AddFirst(new Torque(torque, time)));
    }
    
    public bool RemoveTorque(TorqueToken token)
    {
        torques.Remove(token.token);
        return true;
    }
}


public class ForceToken
{
    public LinkedListNode<Force> forceToken;
    public LinkedListNode<Torque> torqueToken;
    public ForceToken(LinkedListNode<Force> _FT, LinkedListNode<Torque> _TT)
    {
        forceToken = _FT;
        torqueToken = _TT;
    }
}

public class TorqueToken
{
    public LinkedListNode<Torque> token;
    public TorqueToken(LinkedListNode<Torque> _T)
    {
        token = _T;
    }
}