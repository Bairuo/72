using UnityEngine;
using System.Collections;

public class CharactAttribute
{
    string id;
    //Remote Creat infomation
    public string prefabname;
    public float x;
    public float y;
    public float z;

    //Basic information
    public int health;
    public int damage;
    public float speed;
    //Weapon information
    public int clip;
    public int addforce;
    public float calmtime;
    //Resource information
    public string boltname;
    public string dropname;

    //Details for enemy
    public int type = 0;
    public int value = 0;
    public float probability = 0;
    public int camp = 0;

    public CharactAttribute(float _x, float _y, float _z, string _prefabname, int _health, int _damage, float _speed, int _clip, int _addforce, float _calmtime, string _boltname, string _dropname, int _type, int _value, float _probability, int _camp)
    {
        prefabname = _prefabname;
        x = _x;
        y = _y;
        z = _z;

        health = _health;
        damage = _damage;
        speed = _speed;

        clip = _clip;
        addforce = _addforce;
        calmtime = _calmtime;

        boltname = _boltname;
        dropname = _dropname;

        type = _type;
        value = _value;
        probability = _probability;
        camp = _camp;
    }

    public CharactAttribute(GameObject obj)
    {
        this.GetAttribute(obj);
    }

    public void GetAttribute(GameObject obj)
    {
        /*
        Character character = obj.GetComponent<Character>();
        prefabname = character.PrefabName;
        x = obj.transform.position.x;
        y = obj.transform.position.y;
        z = obj.transform.position.z;

        health = character.health;
        damage = character.damage;
        speed = character.speed;

        clip = character.ShootClip;
        addforce = character.ShootForce;
        calmtime = character.ClipCalmtime;

        type = character.type;
        value = character.value;
        probability = character.Probability;
        camp = character.Camp;

        boltname = character.Boltname;
        if (boltname == "" && character.Bolt.name != null)
        {
            boltname = character.Bolt.name;
        }
        dropname = character.Dropname;
        if (dropname == "" && character.Drop != null)
        {
            dropname = character.Drop.name;
        }

        if (obj.tag == "Player")
        {
            if (Client.IsUse()) id = Client.instance.playerid;
        }
        else
        {
            id = obj.GetInstanceID().ToString();
        }
        */
    }

    public ProtocolBytes GetInfoProto(string protoName)
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString(protoName);
        proto.AddString(id); //如果是玩家，id是房间内id，否则是unity创建id

        proto.AddString(prefabname);
        proto.AddFloat(x);
        proto.AddFloat(y);
        proto.AddFloat(z);

        proto.AddInt(health);
        proto.AddInt(damage);
        proto.AddFloat(speed);

        proto.AddInt(clip);
        proto.AddInt(addforce);
        proto.AddFloat(calmtime);

        proto.AddString(boltname);
        proto.AddString(dropname);

        proto.AddInt(type);
        proto.AddInt(value);
        proto.AddFloat(probability);
        proto.AddInt(camp);

        return proto;
    }
}
