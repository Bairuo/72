using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardUIController : MonoBehaviour {
    public Text text1;
    public Text text2;
    public Text text3;
    public Text text4;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void output(int colTimes, int itemTimes, float time, float invTime, int num)
    {
        text1.text = colTimes.ToString();
        text2.text = itemTimes.ToString();
        text3.text = time.ToString();
        text4.text = invTime.ToString();
    }
}
