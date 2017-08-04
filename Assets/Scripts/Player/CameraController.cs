using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Quaternion rotation;

	void Start () {
        rotation = transform.rotation;
	}
}
