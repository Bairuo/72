using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hpbar1 : MonoBehaviour {

    public Image front;
    public GameObject speedLevelController;
    public GameObject massLevelController;
    private GameObject hostPlayer;

	// Use this for initialization
	void Start () {
        front.fillAmount = 1;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!hostPlayer)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
            if(objs != null)
            {
                foreach(GameObject obj in objs)
                {
                    if(obj.GetComponent<PlayerController>().IsMine())
                    {
                        hostPlayer = obj;
                        front.fillAmount = hostPlayer.GetComponent<PlayerController>().health / 100;
                        break;
                    }
                }
            }
        }
        else
        {
            front.fillAmount = hostPlayer.GetComponent<PlayerController>().health / 100;
            speedLevelController.GetComponent<LevelController>().level = hostPlayer.GetComponent<PlayerController>().SpeedLevel;
            massLevelController.GetComponent<LevelController>().level = hostPlayer.GetComponent<PlayerController>().MassLevel;
        }
    }
}
