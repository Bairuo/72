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

    // 关键操作
    public void Turn()
    {
        //transform.rota
    }
    public void Dealth()
    {
        if (PlayerID != Client.instance.playerid) return;
        transform.DetachChildren();

        this.gameObject.SetActive(false);
    }

    public void PlayerDestroy()
    {
        Client.instance.posmanager.PlayerLogoff(PlayerID);
        Destroy(this.gameObject);
    }


    // 属性更改
    public void ChangeBrake(float brake)
    {
        if (PlayerID != Client.instance.playerid) return;
        Client.instance.SendChangeBrake(PlayerID, brake);
        RealChangeBrake(brake);
    }
    public void ChangeSpeed(float speed)
    {
        if (PlayerID != Client.instance.playerid) return;
        Client.instance.SendChangeSpeed(PlayerID, speed);
        RealChangeSpeed(speed);
    }
    public void ChangeHealth(float health)      // 生命（参数）小于零时会触发死亡
    {
        if (PlayerID != Client.instance.playerid) return;
        if (health > 0)
        {
            Client.instance.SendChangeHealth(PlayerID, health);
            RealChangeHealth(health);
        }
        else
        {
            Dealth();
        }
        
    }
    public void ChangeStatus(int status)
    {
        if (PlayerID != Client.instance.playerid) return;
        Client.instance.SendChangeStatus(PlayerID, status);
        RealChangeStatus(status);
    }

    public void RealChangeBrake(float brake)
    {
        this.brake = brake;
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
