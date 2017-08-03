using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaftyArea : MonoBehaviour
{
	public float radius;
	public float decreasePerSec;
	// public float minRadius;
	float initRadius;
	
	public SpriteRenderer rd;
	Vector2 rdInitScale;
	
	public int InvBit = 0x1;
	
	void Start()
	{
		initRadius = radius;
		rdInitScale = rd.gameObject.transform.localScale;
	}
	
	void Update()
	{
		radius -= decreasePerSec;
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
		
		float rate = radius / initRadius;
		rd.gameObject.transform.localScale = rate * rdInitScale;
	}
}
