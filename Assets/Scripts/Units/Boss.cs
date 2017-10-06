using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float baseDamagePerSecond;
    public readonly Modifier damageModifier = new Modifier();
    
    public float exitTime;
    
    private void OnCollisionStay2D(Collision2D x)
    {
        var unit = x.collider.gameObject.GetComponent<Unit>(); // get Unit component from *this* colllider.
        if(unit == null) return;
        
        // A collision will execute in fixed update circle of physics.
        unit.health -= damageModifier.GetValue(baseDamagePerSecond) * Time.fixedDeltaTime;
    }
    
    [SerializeField] float t;
    void Start()
    {
        t = 0f;
    }
    
    void Update()
    {
        if(t > exitTime) Destroy(this.gameObject);
    }
    
    void OnDestroy()
    {
        // do nothing now...
        // should add some FX here...
    }
}
