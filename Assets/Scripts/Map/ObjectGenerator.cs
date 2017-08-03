using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
	public SaftyArea saftyArea;
	
	public GameObject[] sources;
	public float[] weight;
	float weightSum;
	public float areaMulti; // things are generated within radius * areaMulti area.
	public int timesAttempt;
	
	public GameObject taggerSource;
	public float taggerDelay;
	
	public float delay;
	
	public float t;
	public bool tagged;
	
	void Start()
	{
		foreach(var i in weight)
		{
			weightSum += i;
		}
	}
	
	public bool CanGenerate(Vector2 loc)
	{
		return true;
	}
	
	Vector2 genLoc;
	
	void Update()
	{
		t -= Time.deltaTime;
		
		if(t <= taggerDelay && !tagged)
		{
			SetRandomGenLoc();
			GenerateTagger();
			tagged = true;
		}
		if(t <= 0f)
		{
			Generate();
			t += delay;
			tagged = false;
		}
	}
	
	void SetRandomGenLoc()
	{
		// random location inside the circle.
		for(int i=0; i<timesAttempt; i++)
		{
			float dist = Mathf.Sqrt(Random.Range(0f, 1f)) * saftyArea.radius * areaMulti;
			float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
			Vector2 loc = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;
			if(CanGenerate(loc))
			{
				// generate.
				genLoc = loc;
				break;
			}
		}
	}
	
	void GenerateTagger()
	{
		GenerateTagger(genLoc);
	}
	void GenerateTagger(Vector2 loc)
	{
		if(taggerSource != null)
		{
			var a = GameObject.Instantiate(taggerSource);
			a.transform.position = loc;
		}
	}
	
	void Generate()
	{
		// random choose an object.
		int id = -1;
		float r = Random.Range(0f, weightSum);
		for(int i=0; i<sources.Length; i++)
		{
			if(r - weight[i] <= 0f)
			{
				id = i;
				break;
			}
			r -= weight[i];
		}
		
		// generate an item.
		Generate(id, genLoc);
	}
	void Generate(int id, Vector2 loc)
	{
		var a = Instantiate(sources[id]);
		a.transform.position = loc;
	}
}
