using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTagger : MonoBehaviour
{
	public float lifetime;
	
	float t;
	
	void Start()
	{
		t = lifetime;
	}
	
	void Update()
	{
		t -= Time.deltaTime;
		if(t <= 0f)
		{
			Destroy(this.gameObject);
		}
	}
	
	
}
