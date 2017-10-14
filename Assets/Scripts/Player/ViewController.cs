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
    
    /// The multiple of velocity direction to move the target position.
    /// Linear multiple.
    public float multLinear;
    /// Square multiple.
    public float multSqr;
    
    /// The multiply that move close to the target point per second.
    [Range(0f, 1f)] public float multMove;
    
    /// The multiple of speed limit of camera depends on attachment's speed.
    public float multSpeedLimit;
    
    /// The instance distance of moving to target point per second.
    /// Depreated.
    // public float flatMove;
    
    [SerializeField] Vector2 targetPos;
    void Update()
    {
        // Lock the rotation of the camera.
        this.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
        
        // Get the target position of camera, tracking the attachment,
        //   add a forward amount depends on velocity.
        Vector2 curv = body.velocity;
        float speed = curv.magnitude;
        float extDist = speed * multLinear + speed * speed * multSqr;
        Vector2 curdir = attachment.transform.rotation * Vector2.up;
        Vector2 curpos = attachment.transform.position;
        
        targetPos = curdir * extDist + curpos;
        
        // move the camera to the target point with a consequentive speed.
        Vector2 campos = this.gameObject.transform.position;
        float depth = this.gameObject.transform.position.z;
        Vector2 deltaPos = targetPos - campos;
        float speedLimit = multSpeedLimit * curv.magnitude;
        if(deltaPos.magnitude > speedLimit * Time.deltaTime) deltaPos = deltaPos.normalized * speedLimit * Time.deltaTime;
        this.gameObject.transform.position = new Vector3(deltaPos.x + campos.x, deltaPos.y + campos.y, depth);
    }
    
}