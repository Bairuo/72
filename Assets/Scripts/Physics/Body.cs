using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    /// Linear drag.
    public float drag = 0f;
    
    /// Angular drag.
    public float angularDrag = 0f;
    
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
    
    public bool freezed = false;
    
    /// The max distance from collider polygon and object's center.
    public float farDist;
    
    List<Force> forces = new List<Force>();
    List<Torque> torques = new List<Torque>();
    
    [HideInInspector] public PolygonCollider2D col;
    
    /// Colliders that touches this object in the last physics step.
    List<Body> collides;
    
    void Start()
    {
        PhysWorld.bodies.Add(this);
        
        col = this.gameObject.GetComponent<PolygonCollider2D>();
        
        farDist = 0f;
        foreach(var i in col.points) farDist = Mathf.Max(farDist, i.magnitude);
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
    
    float Cutoff(float curv, float topv) { return (curv - topv) / topv; }
    
    public void StepMovement(float timestep)
    {
        // ============== Acceleration Control ================
        
        Torque sq = new Torque();
        Force sf = new Force();
        for(int i = forces.Count - 1; i >= 0; i--)
        {
            Force cf = forces[i];
            sf.value += cf.value * Cutoff(cf.timelast, timestep);
            cf.timelast -= timestep;
            forces[i] = cf;
        }
        forces.RemoveAll((Force x) => { return x.timelast <= 0f; });
        
        for(int i = torques.Count - 1; i >= 0; i--)
        {
            Torque cq = torques[i];
            sq.value += cq.value * Cutoff(cq.timelast, timestep);
            cq.timelast -= timestep;
            torques[i] = cq;
        }
        torques.RemoveAll((Torque x) => { return x.timelast <= 0f; });
        
        Vector2 acce = this.gameObject.transform.rotation * sf.value / mass;
        float angacce = sq.value / MOI;
        
        velocity += acce * timestep;
        angularVelocity += angacce * timestep;
        
        // ============= Drag Velocity Simulation =============
        
        velocity *= Mathf.Pow(1 - drag, timestep);
        angularVelocity *= Mathf.Pow(1 - angularDrag, timestep);
        
        // ================ Move Simulation ===================
        
        Vector2 deltapos = velocity * timestep - 0.5f * acce * timestep * timestep;
        float deltadir = angularVelocity - 0.5f * angacce * timestep * timestep;
        if((deltadir > 0 && angularVelocity < 0) || (deltadir < 0 && angularVelocity > 0))
            deltadir = 0f;
        
        if(freezed)
        {
            velocity = Vector2.zero;
            angularVelocity = 0f;
        }
        else
        {
            this.gameObject.transform.position = (Vector2)this.gameObject.transform.position + deltapos;
            this.gameObject.transform.Rotate(0f, 0f, deltadir);
        }
    }
    
    int colCount;
    public GameObject taggerSource;
    GameObject[] taggers = new GameObject[15]; // [!]DEBUG REQUEST. REMOVE WHEN NOT DEBUGGING!!! 
    Vector2[] isc = new Vector2[15]; // intersection points location in world space.
    
    // Call this function when current collider first collides with another.
    public bool CollisionCall(Body ox, float timestep)
    {
        // single force calculation:
        //   calculate the force within this gameobject's script.
        //   for the other collider, not changing anything though.
        // the time this pair of forces applied are equal to two colliders.
        // force depends on :
        // speed, contact conditions, and hardness.
        
        var cx = ox.col;
        colCount = Calc.GetIntersectionPoints(col, cx, ref isc);
        
        // ============== DEBUG : CONTACTS ==============
        
        if(taggers[0] == null)
        {
            var col = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            for(int i = taggers.Length - 1; i >= 0; i--)
            {
                taggers[i] = Instantiate(taggerSource, Vector2.one * 99999f, new Quaternion(0f, 0f, 0f, 1f));
                taggers[i].name = this.gameObject.name + ":" + i;
                Destroy(taggers[i].GetComponent<ItemTagger>());
                taggers[i].GetComponent<SpriteRenderer>().color = col;
            }
        }
        
        for(int i=0; i<colCount; i++)
        {
            taggers[i].transform.position = isc[i];
        }
        
        if(colCount >= 2)
        {
            taggers[0].transform.rotation = 
            taggers[1].transform.rotation = 
                Quaternion.Euler(0f, 0f, Calc.Angle(Vector2.up, Calc.Rot90(new Segment(isc[0], isc[1]).dir.normalized)));
        }
        
        for(int i = colCount; i < taggers.Length; i++) taggers[i].transform.position = Vector2.one * 99999f;
        
        if(colCount == 0) return false;
        
        // ============ Collision Response ==============
        // Consider forces applied to itself.
        // Not affect the other collider.
        var ob = cx.gameObject.GetComponent<Body>();
        if(ob != null)
        {
            // Coefficient of Restitution.
            // defined as - deltaV before / deltaV after.
            // determined by material properties (like hardness).
            float e = this.hardness * ob.hardness;
            if(e < 0f) e = 0f;
            // intersection line.
            Segment L = new Segment(isc[0], isc[1]);
            // collision point. Center of intersection line.
            Vector2 mid = L.Interpolate(0.5f);
            // relative position of collision point to mass center.
            Vector2 rc = mid - (Vector2)this.gameObject.transform.position;
            Vector2 rp = mid - (Vector2)ob.gameObject.transform.position;
            // relative velocity.
            Vector2 rv = velocity - ob.velocity + Calc.Rot90(rc) * this.angularVelocity - Calc.Rot90(rp) * ob.angularVelocity;
            // normal of collision. Whatever direction is.
            Vector2 f = Calc.Rot90(L.dir.normalized);
            if(Calc.DotMultiply(f, rc) < 0f) f = -f;
            // Impulse.
            Vector2 I = f;
            I *= - (1f + e) * Calc.DotMultiply(rv, f) / (
                (1f / this.mass + 1f / ob.mass) +
                Calc.DotMultiply(f, Vector3.Cross(Vector3.Cross((Vector3)rc, f) / this.MOI, rc)) +
                Calc.DotMultiply(f, Vector3.Cross(Vector3.Cross((Vector3)rp, f) / ob.MOI, rp)));
            
            if(Vector2.Distance(isc[0], isc[1]) > 1.0f || I.magnitude < 100f) // seperate two objects for no reason.
            {
                Vector2 dip = ob.gameObject.transform.position - this.gameObject.transform.position;
                this.velocity -= dip.normalized * Mathf.Pow(0.2f, timestep);
                this.angularVelocity *= Mathf.Pow(0.1f, timestep);
                ob.velocity += dip.normalized * Mathf.Pow(0.2f, timestep);
                ob.angularVelocity *= Mathf.Pow(0.1f, timestep);
            }
            else
            {
                this.velocity += I / this.mass;
                this.angularVelocity += Calc.CrossMultiply(rc, I) / this.MOI;
                this.angularVelocity = Calc.RangeCut(this.angularVelocity, -2.0f * Mathf.PI, 2.0f * Mathf.PI);
                
                ob.velocity += -I / ob.mass;
                ob.angularVelocity += Calc.CrossMultiply(rp, -I) / ob.MOI;
                ob.angularVelocity = Calc.RangeCut(ob.angularVelocity, -2.0f * Mathf.PI, 2.0f * Mathf.PI);
            }
        }
        
        return true;
    }
    
    
    /// @force relative direction.
    /// @loc RELATIVE location where the force is applied.
    /// @time time the force applied.
    public void AddForce(Vector2 force, Vector2 loc, float time)
    {
        Quaternion qx = this.gameObject.transform.rotation;
        Vector2 rotedForce = qx * force;
        forces.Add(new Force(rotedForce, time));
        torques.Add(new Torque(Calc.CrossMultiply(loc, force), time));
    }
    
    /// @torque
    /// @ time time the torque applied.
    public void AddTorque(float torque, float time)
    {
        torques.Add(new Torque(torque, time));
    }
}


