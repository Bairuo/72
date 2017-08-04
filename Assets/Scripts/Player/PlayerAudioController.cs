using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
	public AudioSource[] collisionAudio;
	public AudioSource[] pickingAudio;
	public AudioSource[] portalAudio;
	
	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.GetComponent<PortalController>() != null)
		{
			PlayPortalAudio();
		}
		else if(col.gameObject.GetComponent<PlayerController>() != null)
		{
			PlayCollisionAudio();
		}
		else
		{
			PlayPickAudio();
		}
	}
	
	void OnColliderEnter2D(Collision2D col)
	{
		if(col.gameObject.GetComponent<PortalController>() != null)
		{
			PlayPortalAudio();
		}
		else if(col.gameObject.GetComponent<PlayerController>() != null)
		{
			PlayCollisionAudio();
		}
		else
		{
			PlayPickAudio();
		}
	}
	
	int Randint(int c)
	{
		return Mathf.FloorToInt(Random.Range(0f, c));
	}
	
	void PlayCollisionAudio()
	{
		collisionAudio[Randint(collisionAudio.Length)].Play();
	}
	
	void PlayPickAudio()
	{
		pickingAudio[Randint(pickingAudio.Length)].Play();
	}
	
	void PlayPortalAudio()
	{
		portalAudio[Randint(portalAudio.Length)].Play();
	}
	
}
