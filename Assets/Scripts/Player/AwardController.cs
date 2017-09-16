using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwardController : MonoBehaviour {


    public GameObject RewardTipsController;


    void Start()
    {
        RewardTipsController = GameObject.Find("RewardTipsController");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gbj = collision.gameObject;
        if (gbj.GetComponent<SpeedUp>())
        {
            RewardTipsController.gameObject.GetComponent<RewardTipsController>().SpeedUp(this.gameObject.transform);
        }
        if (gbj.GetComponent<SlowDown>())
        {
            RewardTipsController.gameObject.GetComponent<RewardTipsController>().SpeedDown(this.gameObject.transform);
        }
        if (gbj.GetComponent<MassUp>())
        {
            RewardTipsController.gameObject.GetComponent<RewardTipsController>().WeightUp(this.gameObject.transform);
        }
        if (gbj.GetComponent<MassDown>())
        {
            RewardTipsController.gameObject.GetComponent<RewardTipsController>().WeightDown(this.gameObject.transform);
        }
    }
}
