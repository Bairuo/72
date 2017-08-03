using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public int status = 0;
    public float health;
    public float speed = 1;

    public float brake = 0;     // 刹车系数

    public string PlayerID = "0";


    Vector2 last_velocity = new Vector2(0, 1); 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (PlayerID != Client.instance.playerid) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 ClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float distance = Vector2.Distance(transform.position, ClickPos);

            // 加速
            Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
            Vector2 SpeedForce = (Vector2)(ClickPos - transform.position);

            if (Vector2.Dot(velocity, SpeedForce) < 0)      // 由于没有刹车操作，自动判断刹车
            {
                GetComponent<Rigidbody2D>().velocity *= brake;
            }
            GetComponent<Rigidbody2D>().AddForce(SpeedForce * speed);

            // 旋转

            
        }
	}


    public void Turn()
    {
        //transform.rota
    }

    public void ChangeSpeed(float speed)
    {
        Client.instance.SendChangeSpeed(PlayerID, speed);
        RealChangeSpeed(speed);
    }

    public void ChangeHealth(float health)
    {
        Client.instance.SendChangeHealth(PlayerID, health);
        RealChangeHealth(health);
    }

    public void ChangeStatus(int status)
    {
        Client.instance.SendChangeStatus(PlayerID, status);
        RealChangeStatus(status);
    }


    public void RealChangeSpeed(float speed)
    {
        this.speed = speed;
    }

    public void RealChangeHealth(float health)
    {
        this.health = health;
    }

    public void RealChangeStatus(int status)
    {
        this.status = status;
    }
}
