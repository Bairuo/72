using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    GameObject camera;
    GameObject fog;
    public GameObject Cir;
    public Sprite normalCar;
    public Sprite invCar;
    public Sprite bloodCar;

    // 可更改属性
    public int SpeedLevel = 1;
    public int MassLevel = 1;
    public int status = 0;
    public float health;
    public float speed = 1;
    public float brake = 0;     // 刹车系数

    // 固定参数
    private float InitialMass = 1;
    public float brakeAngle = 60;
    public float ImpactForce = 4;

    // 记录属性
    public int ImpactTimes = 0;
    public int ItemTimes = 0;
    float SurvivalTime = 0;
    float InvTime = 0;

    // 其它变量
    public string PlayerID = "0";

    Vector3 rotate_axis = new Vector3(0, 0, 1);
    Vector2 velocity_zero = new Vector2(0, 0);
    Vector2 up = new Vector2(0, 1);
    Vector2 left = new Vector2(-1, 0);

    public Vector2 fict_velocity = new Vector2(0, 0);
    float now_angle = 0;

    // 控制变量
    bool IsDamaged = false;
    static bool HostAndDeath = false;
    float damagetimer = 0;
    float damageTime = 0.2f;

	// Use this for initialization
	void Start () {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        fog = GameObject.FindGameObjectWithTag("fog");
        InitialMass = GetComponent<Rigidbody2D>().mass;
        if (PlayerID == Client.instance.playerid)
        {
            fog.transform.SetParent(transform);
            float z = fog.transform.position.z;
            fog.transform.localPosition = new Vector3(0, 0, z);
        }
	}


    public void SetCir()
    {
        Sprite cirsprite;
        if (PlayerID == Client.instance.playerid)
        {
            cirsprite = Resources.Load<Sprite>("Player/carCir_me");
        }
        else
        {
            cirsprite = Resources.Load<Sprite>("Player/carCir_enemy");
        }

        if (cirsprite != null)
        {
            Cir.GetComponent<SpriteRenderer>().sprite = cirsprite;
        }
    }

	// Update is called once per frame
	void Update () {

        if (IsDamaged) damagetimer += Time.deltaTime;

        if (damagetimer > damageTime)
        {
            GetComponent<SpriteRenderer>().sprite = normalCar;
            damagetimer = 0;
            IsDamaged = false;
        }

        if (health > 0)
        {
            SurvivalTime += Time.deltaTime;
            if (status == 1)
            {
                InvTime += Time.deltaTime;
            }
        }

        // 光环旋转

        if (Cir != null)
        {
            Cir.transform.Rotate(rotate_axis, 30 * Time.deltaTime);
        }

        // 移动
        if (PlayerID != Client.instance.playerid) return;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 ClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Click只有房主接收（且房主不必发送），在房主客户端处理物理碰撞
            if (PlayerID != "0") Client.instance.SendPlayerClick(PlayerID, ClickPos.x, ClickPos.y, ClickPos.z);
            
            if (PlayerID == "0") AddForce(ClickPos);

        }

	}

    // 根据速度自动旋转
    void LateUpdate()
    {
        Vector2 velocity = GetComponent<Rigidbody2D>().velocity;

        if (Client.IsRoomServer()) velocity = GetComponent<Rigidbody2D>().velocity;
        else velocity = fict_velocity;

        float angle;
        if (velocity != velocity_zero) angle = Vector2.Angle(up, velocity);
        else angle = now_angle;

        if (Vector2.Dot(left, velocity) < 0)
        {
            angle = -angle;
        }


        if (velocity != velocity_zero)
        {
            now_angle = Mathf.Lerp(now_angle, angle, 4 * Time.deltaTime);
            transform.rotation = Quaternion.identity;
            transform.Rotate(rotate_axis, now_angle);
        }

        if (PlayerID == Client.instance.playerid)
        {
            camera.transform.rotation = camera.GetComponent<CameraController>().rotation;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            ImpactTimes++;
        }

        if (collision.tag == "Player" && Client.IsRoomServer())
        {
            Vector2 velocity1 = GetComponent<Rigidbody2D>().velocity;
            Vector2 velocity2 = collision.gameObject.GetComponent<Rigidbody2D>().velocity;


            if (velocity1.magnitude > velocity2.magnitude)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity += velocity1 * ImpactForce;

                if (velocity1.magnitude > 15)
                {
                    float health = collision.gameObject.GetComponent<PlayerController>().health;
                    collision.gameObject.GetComponent<PlayerController>().ChangeHealth(health - velocity1.magnitude);
                }
            }
        }

        if (collision.tag == "Item")
        {
            ItemTimes++;
        }
    }

    // 关键操作
    public void AddForce(Vector3 ClickPos)
    {
        float distance = Vector2.Distance(transform.position, ClickPos);
        
        // 加速
        Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
        Vector2 SpeedForce = (Vector2)(ClickPos - transform.position);
        float turnangle = Vector2.Angle(velocity, SpeedForce);

        // 数值处理
        brake = CalcBrake(turnangle);
        float speedK = CalcSpeedConstant(SpeedLevel);

        // 由于没有刹车操作，自动判断刹车

        if (turnangle > brakeAngle)
        {
            GetComponent<Rigidbody2D>().velocity *= brake;
        }
        //Debug.Log(SpeedForce);
        GetComponent<Rigidbody2D>().AddForce(SpeedForce * speed * speedK);
    }
    public void Dealth()
    {
        if (PlayerID != Client.instance.playerid) return;
        Client.instance.SendPlayerDestroy(PlayerID);
        Destroy(Cir);
        transform.DetachChildren();

        int playernum = GameObject.FindGameObjectsWithTag("Player").Length;     // 自己未被摧毁前，当前场景中的剩余玩家数即是排名

        RewardUIController.SetInformation(ImpactTimes, ItemTimes, SurvivalTime, InvTime, playernum);
        
        if (!Client.IsRoomServer() || Client.IsRoomServer() && playernum == 2 || Client.instance.roomnum == 1)  //房间人数只有1-测试模式时也将切换场景 
        {
            Application.LoadLevel("reward");
        }
        else
        {
            HostAndDeath = true;
        }

        this.gameObject.SetActive(false);
    }

    public void PlayerDestroy()
    {
        Client.instance.posmanager.PlayerLogoff(PlayerID);

        Destroy(this.gameObject);

        // 胜利检验
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length - 1 == 1)
        {
            //Client.instance.SendFail();
            Victory();
        }
        

    }
    private void Victory()
    {
        // 没有死亡，没有载入reward
        // 不是因为房主的原因不载入reward
        if (!HostAndDeath) RewardUIController.SetInformation(ImpactTimes, ItemTimes, SurvivalTime, InvTime, 1);
        Application.LoadLevel("reward");
    }

    public bool IsMine()
    {
        return PlayerID == Client.instance.playerid;
    }

    // 数值计算
    private float CalcBrake(float turnangle)
    {
        float brake = 0;

        if (turnangle > 160) brake = 0;
        else if (turnangle > 130) brake = 0.1f;
        else if (turnangle > 90) brake = 0.2f;
        else if (turnangle > 60) brake = 0.3f;

        return brake;
    }
    private float CalcSpeedConstant(float speedlevel)
    {
        float SpeedK = 1;

        if (speedlevel == 1)
        {
            SpeedK = 1;
        }
        else if (speedlevel == 2)
        {
            SpeedK = 1.33f;
        }
        else if (speedlevel == 3)
        {
            SpeedK = 1.66f;
        }
        else if (speedlevel == 4)
        {
            SpeedK = 2;
        }

        return SpeedK;
    }
    private float CalcMassConstant(float masslevel)
    {
        float MassK = 1;

        if (masslevel == 1)
        {
            MassK = 1;
        }
        else if (masslevel == 2)
        {
            MassK = 2;
        }
        else if (masslevel == 3)
        {
            MassK = 3;
        }
        else if (masslevel == 4)
        {
            MassK = 4;
        }

        return MassK;
    }

    // 属性更改

    public void ChangePosition(float x, float y, float z)
    {
        if (PlayerID != Client.instance.playerid) return;
        Client.instance.SendChangePosition(PlayerID, x, y, z);
        RealChangePosition(x, y, z);
    }
    public void ChangeMassLevel(int masslevel)
    {
        if (PlayerID != Client.instance.playerid) return;
        Client.instance.SendChangeMassLevel(PlayerID, masslevel);
        RealChangeMassLevel(masslevel);
    }
    public void ChangeSpeedLevel(int speedlevel)
    {
        if (PlayerID != Client.instance.playerid) return;
        Client.instance.SendChangeSpeedLevel(PlayerID, speedlevel);
        RealChangeSpeedLevel(speedlevel);
    }
    public void ChangeMass(float mass)
    {
        if (PlayerID != Client.instance.playerid) return;
        Client.instance.SendChangeMass(PlayerID, mass);
        RealChangeMass(mass);
    }
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

    public void RealChangePosition(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }
    public void RealChangeSpeedLevel(int speedlevel)
    {
        SpeedLevel = speedlevel;
    }
    public void RealChangeMassLevel(int masslevel)
    {
        MassLevel = masslevel;
        float massK = CalcMassConstant(MassLevel);
        GetComponent<Rigidbody2D>().mass = InitialMass * massK;
    }
    public void RealChangeMass(float mass)
    {
        GetComponent<Rigidbody2D>().mass = mass;
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
        IsDamaged = true;
        damagetimer = 0;
        GetComponent<SpriteRenderer>().sprite = bloodCar;
        this.health = health;
    }
    public void RealChangeStatus(int status)
    {
        if (status == 0)
        {
            GetComponent<SpriteRenderer>().sprite = normalCar;
        }
        else if (status == 1)
        {
            GetComponent<SpriteRenderer>().sprite = invCar;
        }

        this.status = status;
    }
}
