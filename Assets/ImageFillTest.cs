using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ImageFillTest : MonoBehaviour {

    public Image image;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        image.fillAmount -= Time.deltaTime / 10;
        Debug.Log(image.fillAmount);
	}
}
