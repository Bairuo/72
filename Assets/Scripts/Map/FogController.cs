using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : ExNetworkBehaviour
{
	public float visiableTime;
	
	public float outerRadius;
	public float innerRadius;
	
	public float rec;
	public float t;
	
	public float begina;
	SpriteRenderer rd;
	Material mat;
	
	public GameObject attachment;
	
	protected override void Start()
	{
		base.Start();
		
		AddProtocol("OpenView", OpenViewMessage, null, OpenViewReceive, OpenViewReceive, typeof(string));
	}
	
	public void MakeVisible()
	{
		rec = t = visiableTime;
	}
	
	protected virtual void FixedUpdate()
	{
		if(!attachment)
		{
			var x = GameObject.FindGameObjectsWithTag("Player");
			foreach(var i in x)
			{
				if(ExPlayerController.IsMyPlayer(i.GetComponent<ExNetworkBehaviour>())) attachment = i;
			}
			if(attachment)
			{
				rd = this.gameObject.GetComponent<SpriteRenderer>();
				mat = rd.material;
				begina = GetAlpha(rd);
			}
		}
		if(!attachment) return;
		
		t -= Time.fixedDeltaTime;
		
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
		
		this.gameObject.transform.position = attachment.gameObject.transform.position;
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
	
	public void SendMakeVisible(string id)
	{
		tempMessage = id;
		Send("OpenView");
	}
	
	string tempMessage;
	object[] OpenViewMessage()
	{
		return new object[]{tempMessage};
	}
	void OpenViewReceive(object[] info)
	{
		string id = info[0] as string;
		if(Client.instance.playerid == id)
		{
			MakeVisible();
		}
	}
}
