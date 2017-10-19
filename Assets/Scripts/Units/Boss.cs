// Created bY DK 2017/10/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float baseDamagePerSecond;
    public readonly Modifier damageModifier = new Modifier();
    
    public float exitTime;
    
    [SerializeField] float stepTime = 0f;
    public float minTime;
    public float maxTime;
    public float speed;
    public float minDeltaAngle;
    public float maxDeltaAngle;
    public float deltaAngle;
    [SerializeField] Vector2 velocity;
    public SaftyArea sa;
    
    /// [!]Notice: this value may not be the same as it on the server.
    ///     All damage receive operation is invalid if this is a client.
    private void OnCollisionStay2D(Collision2D x)
    {
        var unit = x.collider.gameObject.GetComponent<Unit>(); // get Unit component from *this* colllider.
        if(unit == null) return;
        
        // A collision will execute in fixed update circle of physics.
        unit.health -= damageModifier.GetValue(baseDamagePerSecond) * Time.fixedDeltaTime;
    }
    
    [SerializeField] float t = 0f;
    void Start()
    {
        // Client.instance.posmanager.PlayerRegister(this.gameObject);
    }
    
    
    void Update()
    {
        t += Time.deltaTime;
        if(t > exitTime) Destroy(this.gameObject);
        
    }
    
    void FixedUpdate()
    {
        if(!Client.IsRoomServer()) return; // position is set to the posmanager.
        
        if(sa == null) sa = Component.FindObjectOfType<SaftyArea>();
        // boss movement definition:
        // random choose a direction and a time.
        // turn alongside this direction for the time.
        if(sa != null)
        {
            stepTime -= Time.deltaTime;
            if(stepTime < 0f)
            {
                stepTime += Random.Range(minTime, maxTime);
                deltaAngle = Random.Range(minDeltaAngle, maxDeltaAngle);
            }
            velocity = Calc.ApplyRotationAngle(deltaAngle * Time.deltaTime, velocity);
            this.gameObject.transform.position = (Vector2)this.gameObject.transform.position + velocity * Time.deltaTime;
            if(this.gameObject.transform.position.magnitude >= sa.radius)
            {
                velocity = -this.gameObject.transform.position.normalized * speed;
            }
        }
    }
    
    void OnDestroy()
    {
        // do nothing now...
        // should add some FX here...
    }
}
