using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardUIController : MonoBehaviour {
    public Sprite[] sprites;
    public GameObject st;
    public Text text1;
    public Text text2;
    public Text text3;
    public Text text4;

    public static int colTime = 0;
    public static int itemTimes = 0;
    public static float survivalTime = 0;
    public static float invTime = 0;
    public static int Order = 1;

    public static void SetInformation(int coltime, int itemtime, float survivaltime, float invtime, int order)
    {
        colTime = coltime;
        itemTimes = itemtime;
        survivalTime = survivaltime;
        invTime = invtime;
        Order = order;
    }

    // Use this for initialization
    void Start () {
        text1.text = colTime.ToString();
        text2.text = itemTimes.ToString();
        text3.text = survivalTime.ToString("F2") + " s";
        text4.text = invTime.ToString("F2") + " s";

        if (Order == 0) Order = 1;

        st.GetComponent<Image>().sprite = sprites[Order - 1];

        ResetConnection();
	}

    void ResetConnection()
    {
        if (Client.IsUse())
            Client.instance.Close();
        if (ServerNet.IsUse())
            ServerNet.instance.Close();
    }

    public void EnterNetStart()
    {
        Application.LoadLevel("NetStart");
    }
}
