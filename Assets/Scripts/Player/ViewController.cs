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
    
    /// Speed limit of camera *relative* to the target point.
    public float maxSpeed;
    
    public float decelerationDistance;
    
    /// Serialize for moniting inside the inspector.
    [SerializeField] Vector2 targetPos;
    [SerializeField] float relativeSpeed;
    void FixedUpdate()
    {
        // Lock the rotation of the camera.
        this.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
        
        // target position camera moving to.
        targetPos = attachment.transform.position + attachment.transform.rotation * Vector2.up * body.velocity.magnitude;
        
        // Set the position of camera.
        // The camera is chasing the target point.
        Vector2 targetDir = targetPos - (Vector2)this.gameObject.transform.position;
        float distance = targetDir.magnitude;
        
        
        if(targetDir.magnitude < 0.001f)
        {
            this.gameObject.transform.position = targetPos;
        }
        else
        {
            Vector2 deltaPos = targetDir.normalized * Mathf.Min(1.0f, distance / decelerationDistance) * maxSpeed * Time.fixedDeltaTime;
            
            // Do not use Translate(...), it is relative position.
            this.gameObject.transform.position = this.gameObject.transform.position + (Vector3)deltaPos;
        }
    }
    
}