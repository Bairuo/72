using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAudioController : MonoBehaviour
{
	public float timelast;
	public float timedelay;
	public AudioSource currentAudioSource;
	public AudioSource nextAudioSource;
	
	float t;
	float cr;
	float rate;
	
	float curBeginV;
	float nxtEndV;
	
	void Start()
	{
		curBeginV = currentAudioSource.volume;
		nxtEndV = nextAudioSource.volume;
	}
	
	void Update()
	{
		t += Time.deltaTime;
		if(t >= timelast)
		{
			cr = timedelay + timelast - t;
			rate = cr / timedelay;
			nextAudioSource.enabled = true;
			
			currentAudioSource.volume = rate * curBeginV;
			nextAudioSource.volume = (1 - rate) * nxtEndV;
			if(rate > 1f)
			{
				currentAudioSource.enabled = false;
				currentAudioSource.volume = 0f;
				nextAudioSource.volume = nxtEndV;
				Destroy(this);
			}
		}
	}
	
}
