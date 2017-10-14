using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFardistTagger : MonoBehaviour
{
	public GameObject attachment;
	
	SpriteRenderer rd;
	
	void Start()
	{
		if(attachment == null)
		{
			Debug.LogError("A UnitFardisttagger must have an attachment.");
			DestroyImmediate(this);
			return;
		}
		
		rd = this.gameObject.GetComponent<SpriteRenderer>();
		if(rd == null)
		{
			Debug.LogError("A UnitFardistTagger must have a SpriteRenderer mounted to the same objdect.");
			DestroyImmediate(this);
			return;
		}
	}

	public float beginHidingRadius;
	public float endHidingRadius;
	public float radius;
	
	void FixedUpdate()
	{
		if(attachment == null)
		{
			Destroy(this);
			return;
		}
		Camera c = Camera.main;
		Vector2 dir = (attachment.transform.position - c.gameObject.transform.position).normalized;
		this.gameObject.transform.position = (Vector2)c.gameObject.transform.position + dir * radius;
		this.gameObject.transform.rotation = Quaternion.FromToRotation(Vector2.up, dir);
		
		float dist = (attachment.transform.position - c.gameObject.transform.position).magnitude;
		float alpha = 0f;
		if(dist < endHidingRadius)
		{
			alpha = 0f;
		}
		else if(dist > beginHidingRadius)
		{
			alpha = 1f;
		}
		else
		{
			alpha = (dist - endHidingRadius) / (beginHidingRadius - endHidingRadius);
		}
		rd.color = new Color(rd.color.r, rd.color.g, rd.color.b, alpha);
	}
	
}
