using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    /// Linear drag, apply for direct movement.
    [Range(0f, 1f)] public float drag = 0f;
    
    /// Side linear drag, apply for side movement.
    [Range(0f, 1f)] public float sideDrag = 0f;
    
    /// Angular drag.
    [Range(0f, 1f)] public float angularDrag = 0f;
    
    /// Velocity.
    public Vector2 velocity = new Vector2(0f, 0f);
    
    /// Angular velocity in radians. Positive counter-clockwise.
    public float angularVelocity = 0f;
    
    public float mass;
    
    /// Moment of inertia, which defines by delta(AngluarVelocity) = delta(t) * MOI.
    public float MOI;
    
    /// Hardness defines the Coefficient of Restitution in collision.
    /// The formula is COR = 1 - (1 - this.hardness) * (1 - other.hardness).
    /// When a hardness is larger than 1, the COR will be greater than 1 so that
    ///   the collision generates energy for colliders.
    public float hardness;
    
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
    
    List<Force> forces = new List<Force>();
    List<Torque> torques = new List<Torque>();
    
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
        for(int i = forces.Count - 1; i >= 0; i--)
        {
            Force cf = forces[i];
            sf.value += cf.value * Calc.RelativeCut(cf.timelast, timestep);
            cf.timelast -= timestep;
            forces[i] = cf;
        }
        forces.RemoveAll((Force x) => { return x.timelast <= 0f; });
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
        if(freezedRotation) return; // [!]notice: the angular velocity are now not changing inside as well.
        
        // ===================== Accelleration ========================
        
        Torque sq = new Torque();
        for(int i = torques.Count - 1; i >= 0; i--)
        {
            Torque cq = torques[i];
            sq.value += cq.value * Calc.RelativeCut(cq.timelast, timestep);
            cq.timelast -= timestep;
            torques[i] = cq;
        }
        torques.RemoveAll((Torque x) => { return x.timelast <= 0f; });
        
        float angacce = sq.value / MOI;
        angularVelocity += angacce * timestep;
        
        // ========================== Drag ============================
        
        angularVelocity *= Mathf.Pow(1 - angularDrag, timestep);
        
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
    public void AddForce(Vector2 force, Vector2 loc, float time)
    {
        forces.Add(new Force(force, time));
        torques.Add(new Torque(Calc.CrossMultiply(loc, force), time));
    }
    
    /// @torque
    /// @ time time the torque applied.
    public void AddTorque(float torque, float time)
    {
        torques.Add(new Torque(torque, time));
    }
}


