using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
	public SaftyArea saftyArea;
	public GameObject[] sources;
	public float[] weight;
	public float minDelay; // delay between generating two objects.
	public float taggerDelay;
	public float groupDelay;
	
	public int generateCount;
	public float beginTime; // when to begin the generation.
	public float lifeTime;
	
	int[] genid;
	float[] timeline;
	float[] taggerTimeline;
	Vector2[] genloc;
	bool[] generated;
	bool[] taggerGenerated;
	
	public float areaMulti; // things are generated within radius * areaMulti area.
	public GameObject taggerSource;
	
	public float t; // should be 0 at the beginning.
	public bool prepared = false; // whether the information is established.
	
	void GlobalInit()
	{
		/// generate all information for generating operations.
		if(weight.Length != sources.Length)
		{
			Debug.Log("WARNING: you should make every sources an explicit weight.");
		}
		
		timeline = new float[generateCount];
		taggerTimeline = new float[generateCount];
		genid = new int[generateCount];
		genloc = new Vector2[generateCount];
		generated = new bool[generateCount];
		taggerGenerated = new bool[generateCount];
		
		for(int i=0; i<generateCount; i++)
		{
			generated[i] = false;
			taggerGenerated[i] = false;
		}
		
		float steptime = groupDelay - generateCount * minDelay;
		
		/// item and location selction.
		float wsum = 0f;
		foreach(var i in weight)
			wsum += i;
		for(int i=0; i<generateCount; i++)
		{
			float cp = Random.Range(0f, wsum);
			for(int j=0; j<sources.Length; j++)	
			{
				if(cp - weight[j] <= 0f)
				{
					genid[i] = j;
					break;
				}
				cp -= weight[j];
			}
		}
		
		/// time line.
		for(int i=0; i<generateCount; i++)
		{
			timeline[i] = Random.Range(0f, steptime);
		}
		System.Array.Sort(timeline);
		for(int i=0; i<generateCount; i++)
		{
			timeline[i] += i * minDelay + beginTime;
		}
		for(int i=0; i<generateCount; i++)
		{
			taggerTimeline[i] = timeline[i] - taggerDelay;
		}
		
		/// use timeline to calculate radius, generate the loc inside.
		for(int i=0; i<generateCount; i++)
		{
			float R = (saftyArea.maxRadius - timeline[i] * saftyArea.decreasePerSec) * areaMulti;
			float r = Mathf.Sqrt(Random.Range(0f, 1f)) * R;
			float a = Random.Range(0f, 2 * Mathf.PI);
			genloc[i] = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * r;
		}
		
		prepared = true;
	}
	
	void Start()
	{
		GlobalInit();
	}
	
	void Update()
	{
		t += Time.deltaTime;
		
		if(prepared)
		{
			for(int i=0; i<generateCount; i++)
			{
				if(!taggerGenerated[i] && t >= taggerTimeline[i])
				{
                    if (Client.IsRoomServer())
                    {
                        Client.instance.SendTaggerGenerate(this.tag, genloc[i]);
                        GenerateTagger(genloc[i]);
                        taggerGenerated[i] = true;
                    }
				}
				if(!generated[i] && t >= timeline[i])
				{
                    if (Client.IsRoomServer())
                    {
                        Client.instance.SendPropGenerate(this.tag, genid[i], genloc[i]);
                        Generate(genid[i], genloc[i]);
                        generated[i] = true;
                    }

				}
			}
			if(t > lifeTime)
			{
				Destroy(this.gameObject);
			}
		}
	}

	public void GenerateTagger(Vector2 loc)
	{
		if(taggerSource != null)
		{
			var a = GameObject.Instantiate(taggerSource);
			a.transform.position = loc;
		}
	}
	
	public void Generate(int id, Vector2 loc)
	{
		var a = Instantiate(sources[id]);
		a.transform.position = loc;
	}
	
}



// public class ObjectGenerator : MonoBehaviour
// {
// 	public SaftyArea saftyArea;
// 	public GameObject[] sources;
// 	public float[] weight;
// 	float weightSum;
// 	public float areaMulti; // things are generated within radius * areaMulti area.
// 	public int timesAttempt;
// 	public GameObject taggerSource;
// 	public float taggerDelay;
// 	public float delay;
// 	public float t;
// 	public bool tagged;
// 	void Start()
// 	{
// 		foreach(var i in weight)
// 		{
// 			weightSum += i;
// 		}
// 	}
// 	public bool CanGenerate(Vector2 loc)
// 	{
// 		return true;
// 	}
// 	public Vector2 genLoc;
// 	void Update()
// 	{
// 		t -= Time.deltaTime;
		
// 		if(t <= taggerDelay && !tagged)
// 		{
// 			SetRandomGenLoc();
// 			GenerateTagger();
// 			tagged = true;
// 		}
// 		if(t <= 0f)
// 		{
// 			Generate();
// 			t += delay;
// 			tagged = false;
// 		}
// 	}
// 	void SetRandomGenLoc()
// 	{
// 		// random location inside the circle.
// 		for(int i=0; i<timesAttempt; i++)
// 		{
// 			float dist = Mathf.Sqrt(Random.Range(0f, 1f)) * saftyArea.radius * areaMulti;
// 			float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
// 			Vector2 loc = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;
// 			if(CanGenerate(loc))
// 			{
// 				// generate.
// 				genLoc = loc;
// 				break;
// 			}
// 		}
// 	}
// 	void GenerateTagger()
// 	{
// 		GenerateTagger(genLoc);
// 	}
// 	void GenerateTagger(Vector2 loc)
// 	{
// 		if(taggerSource != null)
// 		{
// 			var a = GameObject.Instantiate(taggerSource);
// 			a.transform.position = loc;
// 		}
// 	}
// 	void Generate()
// 	{
// 		// random choose an object.
// 		int id = -1;
// 		float r = Random.Range(0f, weightSum);
// 		for(int i=0; i<sources.Length; i++)
// 		{
// 			if(r - weight[i] <= 0f)
// 			{
// 				id = i;
// 				break;
// 			}
// 			r -= weight[i];
// 		}
		
// 		// generate an item.
// 		Generate(id, genLoc);
// 	}
// 	void Generate(int id, Vector2 loc)
// 	{
// 		var a = Instantiate(sources[id]);
// 		a.transform.position = loc;
// 	}
// }
