using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{
	public float outerRadius;
	public float innerRadius;
	
	public float rec;
	public float t;
	
	public float begina;
	SpriteRenderer rd;
	Material mat;
	
	void Start()
	{
		rd = this.gameObject.GetComponent<SpriteRenderer>();
		mat = rd.material;
		begina = GetAlpha(rd);
	}
	
	void MakeVisible(float time)
	{
		rec = t = time;
	}
	
	void Update()
	{
		t -= Time.deltaTime;
		
		if(t <= 0f)
		{
			t = 0f;
			rec = 0f;
		}
		
		// drawing porcess...
		mat.SetFloat("_OuterRadius", outerRadius);
		mat.SetFloat("_InnerRadius", innerRadius);
		mat.SetFloat("_locx", this.gameObject.transform.position.x);
		mat.SetFloat("_locy", this.gameObject.transform.position.y);
		mat.SetVector("_Color", rd.color);
		
		if(t == 0f)
		{
			SetAlpha(rd, 1.0f);
		}
		else
		{
			SetAlpha(rd, ((rec - t) / rec) * begina);
		}
		
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
