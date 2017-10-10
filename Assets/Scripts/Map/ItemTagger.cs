using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// A real item is created by this GameObject.
/// a prefab is logged inside this object.
/// Deliver the prefab information (name or id) in the ObjectGenerator for synchronization.
public class ItemTagger : MonoBehaviour
{
	public float lifetime;
	public float flashdelay;
	
	public GameObject prefebToCreate = null;
	
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
		
		if(t <= 0f)
		{
			Cancel();
		}
	}
	
	void Cancel()
	{
		if(prefebToCreate != null)
		{
			Instantiate(prefebToCreate, this.gameObject.transform.position, this.gameObject.transform.rotation);
		}
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
