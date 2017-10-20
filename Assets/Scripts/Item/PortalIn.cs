using System;
using UnityEngine;

public class PortalIn : ExNetworkBehaviour
{
    static int cnt;
    
    public GameObject target;
    
    protected override void Start()
    {
        base.Start();
        
        if(Client.IsRoomServer())
        {
            target = ExGameObjectCreator.instance.GlobalGameObjectCreate(
                "Item/PortalOut",
                "PortalOut-" + (cnt += 1),
                Util.RandPos(Vector2.zero, Component.FindObjectOfType<SaftyArea>().radius),
                0f,
                "PortalOut" + (cnt += 1));
        }
    }
    
    
    void OnTriggerEnter2D(Collider2D x)
    {
        if(!Client.IsRoomServer()) return;
        
        var v = x.gameObject.GetComponent<ExPlayerController>();
        if(v == null) return;
        
        // Teleport.
        x.gameObject.transform.position = target.gameObject.transform.position;
        // Camera is teleported too.
        if(ExPlayerController.IsMyPlayer(v))
        {
            float depth = Camera.main.gameObject.transform.position.z;
            Camera.main.gameObject.transform.position =
                new Vector3(target.gameObject.transform.position.x, target.gameObject.transform.position.y, depth);
        }
        
        // Destroy portal.
        Destroy(this.gameObject);
        Destroy(target);
    }
    
    
}