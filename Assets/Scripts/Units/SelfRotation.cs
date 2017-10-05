using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotation : MonoBehaviour
{
    // Positive counter-clockwise, negative counter-clockwise.
    public float rotationSpeed;
    
    void LateUpdate()
    {
    }
    
    void FixedUpdate()
    {
       this.gameObject.transform.Rotate(0f, 0f, rotationSpeed * Mathf.Rad2Deg *Time.fixedDeltaTime);
    }    
}
