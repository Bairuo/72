using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hpbar1 : MonoBehaviour
{
    public Image front;
    public GameObject speedLevelController;
    public GameObject massLevelController;
    Unit host = null;

	void Start ()
    {
        front.fillAmount = 1;
	}
	
	void Update ()
    {
        if(!host)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
            if(objs != null)
            {
                foreach(GameObject obj in objs)
                {
                    if(obj.GetComponent<ExNetworkBehaviour>().netObject.NetID == "Player-" + Client.instance.playerid)
                    {
                        host = obj.GetComponent<Unit>();
                        front.fillAmount = host.health / host.maxHealth;
                        break;
                    }
                }
            }
        }
        else
        {
            front.fillAmount = host.health / host.maxHealth;
            
            int A = host.gameObject.GetComponents<BuffMassUp>().Length;
            int B = host.gameObject.GetComponents<BuffSpeedUp>().Length;
            
            
            massLevelController.GetComponent<LevelController>().level = A;
            speedLevelController.GetComponent<LevelController>().level = B;
        }
    }
}
