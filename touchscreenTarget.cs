using UnityEngine;
using System.Collections;

public class touchscreenTarget : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}


	public virtual void onFingerDown(int uid, Vector2 position)
	{
	}

	public virtual void onFingerUp(int uid, Vector2 position)
	{
	}

	public virtual void onFingerMove(int uid, Vector2 position, Vector2 size)
	{
	}

	// Update is called once per frame
	void Update () {
	
	}
}
