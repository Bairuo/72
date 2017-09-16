using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardTipsController : MonoBehaviour {

    public GameObject su;
    public GameObject sd;
    public GameObject wu;
    public GameObject wd;

    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpeedUp(Transform transform)
    {
        GameObject.Instantiate(su, this.gameObject.transform);
    }

    public void SpeedDown(Transform transform)
    {
        GameObject.Instantiate(su, this.gameObject.transform);
    }


    public void WeightUp(Transform transform)
    {
        GameObject.Instantiate(wu, this.gameObject.transform);
    }

    public void WeightDown(Transform transform)
    {
        GameObject.Instantiate(wd, this.gameObject.transform);
    }
}
