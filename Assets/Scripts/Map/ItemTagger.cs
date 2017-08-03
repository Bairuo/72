using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTagger : MonoBehaviour
{
	public float lifetime;
	public float flashdelay;
	
	SpriteRenderer rd;
	float mxalpha;
	float t;
	
	void Start()
	{
		rd = this.gameObject.GetComponent<SpriteRenderer>();
		t = lifetime;
		mxalpha = GetAlpha(rd);
		
	}
	
	
	void Update()
	{
		t -= Time.deltaTime;
		SetAlpha(rd,
			(Mathf.FloorToInt((lifetime - t) / flashdelay) % 2 == 0 ? 1.0f : 0.0f) +
			((lifetime - t) % flashdelay / flashdelay) * (Mathf.FloorToInt((lifetime - t) / flashdelay) % 2 == 0 ? -1.0f : 1.0f));
		
		//Debug.Log((Mathf.FloorToInt((lifetime - t) / flashdelay) % 2 == 0 ? 0.0f : 1.0f) + " " + ((lifetime - t) % flashdelay / flashdelay * (Mathf.FloorToInt((lifetime - t) / flashdelay) % 2 == 0 ? 1.0f : -1.0f)));
		
		if(t <= 0f)
		{
			Cancel();
		}
	}
	
	void Cancel()
	{
		Destroy(this.gameObject);
	}
	
	float GetAlpha(SpriteRenderer rd)
	{
		return rd.color.a;
	}
	
	void SetAlpha(SpriteRenderer rd, float v)
	{
		Color c = rd.color;
		rd.color = new Color(c.r, c.g, c.b, v);
	}
	
}
