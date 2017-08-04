using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {
    public Image lv1;
    public Image lv2;
    public Image lv3;
    public Image lv4;
    public int level;
	// Use this for initialization
	void Start () {
        level = 1;
	}
	
	// Update is called once per frame
	void Update()
    {
        switch (level)
        {
            case 1:
                LoadLevel1();
                break;
            case 2:
                LoadLevel2();
                break;
            case 3:
                LoadLevel3();
                break;
            case 4:
                LoadLevel4();
                break;

        }
                
	}

    public void LoadLevel1()
    {
        lv1.color = new Color(1, 1, 1, 0.8f);
        lv2.color = new Color(1, 1, 1, 0.5f);
        lv3.color = new Color(1, 1, 1, 0.3f);
        lv4.color = new Color(1, 1, 1, 0.2f);
    }

    public void LoadLevel2()
    {
        lv1.color = new Color(1, 1, 1, 0.8f);
        lv2.color = new Color(1, 1, 1, 0.8f);
        lv3.color = new Color(1, 1, 1, 0.3f);
        lv4.color = new Color(1, 1, 1, 0.2f);
    }

    public void LoadLevel3()
    {
        lv1.color = new Color(1, 1, 1, 0.8f);
        lv2.color = new Color(1, 1, 1, 0.8f);
        lv3.color = new Color(1, 1, 1, 0.8f);
        lv4.color = new Color(1, 1, 1, 0.2f);
    }

    public void LoadLevel4()
    {
        lv1.color = new Color(1, 1, 1, 0.8f);
        lv2.color = new Color(1, 1, 1, 0.8f);
        lv3.color = new Color(1, 1, 1, 0.8f);
        lv4.color = new Color(1, 1, 1, 0.8f);
    }

}
