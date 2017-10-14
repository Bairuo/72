// Created by DK 2017/10/8

using UnityEngine;

/// View controller.
/// Mounted on a camera.
/// Set the attachment gameobject to follow.
/// And track the speed.
public class ViewController : MonoBehaviour
{
    public GameObject attachment;
    
    Body body;
    
    void Start()
    {
        if(attachment == null)
        {
            Debug.Log("WARNING: a viewController should have an attachment object.");
            Destroy(this);
            return;
        }
        
        if(body == null) body = attachment.GetComponent<Body>();
        
        if(this.gameObject.GetComponent<Camera>() == null)
        {
            Debug.Log("WARNING: a view controller should be mounted on a camera.");
        }
    }
    
    public float maxSpeed;
    public float speedMult;
    public float targetMult;
    [SerializeField] Vector2 targetPos;
    void FixedUpdate()
    {
        Vector2 curpos = attachment.transform.position;
        Quaternion currot = attachment.transform.rotation;
        
        targetPos = (Vector2)(currot * Vector2.up) * body.velocity.magnitude * targetMult + curpos;
        
        Vector2 relPos = (Vector2)this.gameObject.transform.position - targetPos;
        
        Vector2 nextRelPos = relPos * Mathf.Pow(speedMult, Time.fixedDeltaTime);
        
        
        if(Vector2.Distance(relPos, nextRelPos) > maxSpeed * Time.deltaTime)
        {
            nextRelPos = relPos.normalized * (relPos.magnitude - maxSpeed * Time.deltaTime);
        }
        
        
        Vector2 position = targetPos + nextRelPos;
        float depth = this.gameObject.transform.position.z;
        this.gameObject.transform.position = new Vector3(position.x, position.y, depth);
    }
    
}