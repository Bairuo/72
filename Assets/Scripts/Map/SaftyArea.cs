using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaftyArea : MonoBehaviour
{
	public float radius;
	public float maxRadius;
	public float circleWidth;
	public float decreasePerSec;
	// public float minRadius;
	float initRadius;
	
	SpriteRenderer rd;
	Material rmt;
		
	public int InvBit = 0x1;
	
	void Start()
	{
		radius = maxRadius;
		rd = this.gameObject.GetComponent<SpriteRenderer>();
		rmt = rd.material;
		//rmt.SetFloat("_InnerRadius", radius - circleWidth);
		//rmt.SetFloat("_OutterRadius", radius);
	}
	
	void Update()
	{
		radius -= decreasePerSec * Time.deltaTime;
		// drawing precess...
		rmt.SetFloat("_InnerRadius", radius - circleWidth);
		rmt.SetFloat("_OuterRadius", radius);
		if(radius < 0.0f) radius = 0.0f;
		
		
		foreach(var i in GameObject.FindGameObjectsWithTag("Player"))
		{
			if(Vector2.Distance(i.transform.position, this.gameObject.transform.position) <= radius)
			{
				var ctr = i.GetComponent<PlayerController>();
				if((ctr.status & InvBit) == 0)
				{
					float h = ctr.health;
					ctr.ChangeHealth(h - Time.deltaTime * decreasePerSec);
				}
			}
		}
	}
}
