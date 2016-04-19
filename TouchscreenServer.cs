using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

public class TouchscreenServer : MonoBehaviour
{
	public int listenPort = 7000;
	private int multicastPort = 7000;
	private int directPort = 8000;
	UdpClient udpClient;
	IPEndPoint endPoint;
	Osc.Parser osc = new Osc.Parser ();
	public GameObject player;
	public static float leftBorder, rightBorder, topBorder, bottomBorder;
	public Boolean debug;
	public Hashtable touches;
	public List<touchscreenTarget> targets;
	//these will get updates on every event coming from osc
	public Boolean multicast;
	private Boolean lastBoolean;

	void Start ()
	{
		foreach (touchscreenTarget target in GameObject.FindObjectsOfType<touchscreenTarget> ())
			targets.Add ((touchscreenTarget)target);
		touches = new Hashtable ();
		lastBoolean = multicast;
		if (multicast) {
			endPoint = new IPEndPoint (IPAddress.Any, multicastPort);
			udpClient = new UdpClient (endPoint);
		} else {
			endPoint = new IPEndPoint (IPAddress.Any, directPort);
			udpClient = new UdpClient (endPoint);
		}

		endPoint = new IPEndPoint (IPAddress.Any, listenPort);
		udpClient = new UdpClient (endPoint);
	}


	void Update ()
	{

		if (multicast != lastBoolean) {
			lastBoolean = multicast;
			if (multicast)
				listenPort = multicastPort;
			else
				listenPort = directPort;
			endPoint = new IPEndPoint (IPAddress.Any, listenPort);
			udpClient = new UdpClient (endPoint);
		}						
					
		if (udpClient != null)
			while (udpClient.Available > 0) {
				osc.FeedData (udpClient.Receive (ref endPoint));
			}

		while (osc.MessageCount > 0) {
			var msg = osc.PopMessage ();

			if (msg.path == "/touchDown") {
					
				int id = (int)msg.data [0];
				Vector2 position = mapToRange (float.Parse (msg.data [1].ToString ()), float.Parse (msg.data [2].ToString ()));
				foreach (touchscreenTarget t in targets) {
					t.onFingerDown (id, position);
				}
				touches [id] = new Touch (id, position);
				if (debug)
					Debug.Log ("new touch!  ID:  " + id + "  (" + position.x + ", " + position.y + ")");
			}

			if (msg.path == "/touchUp") {
				int id = (int)msg.data [0];
				Vector2 position = mapToRange (float.Parse (msg.data [1].ToString ()), float.Parse (msg.data [2].ToString ()));
				foreach (touchscreenTarget t in targets) {
					t.onFingerUp (id, position);
				}
				touches.Remove (id);
				if (debug)
					Debug.Log ("touch up!  ID:  " + id + "  (" + position.x + ", " + position.y + ")");
			}


			if (msg.path == "/touchMove") {
				int id = (int)msg.data [0];
				Vector2 position = mapToRange (float.Parse (msg.data [1].ToString ()), float.Parse (msg.data [2].ToString ()));
				Vector2 size = new Vector2 (float.Parse (msg.data [3].ToString ()), float.Parse (msg.data [4].ToString ()));
				foreach (touchscreenTarget t in targets) {
					t.onFingerMove (id, position, size);
				}
				((Touch)touches [id]).Update (position, size);		
				if (debug)
					Debug.Log ("touch moved! ID:  " + id + "  (" + position.x + ", " + position.y + ")");

			}

			//	Debug.Log (msg);
		}
	}

	public static Vector2 mapToRange (float x, float y)
	{
		Vector2 position = new Vector2 (x, y);
		position.x = map (position.x, 0, 1, leftBorder, rightBorder);
		position.y = map (position.y, 0, 1, bottomBorder, topBorder);
		return position;
	}

	// c#
	public static float map (float s, float a1, float a2, float b1, float b2)
	{
		return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
	}
}

public class Touch
{
	public int id;
	public Vector2 position, shape;

	public Touch ()
	{
		position = new Vector2 (0, 0);
		shape = new Vector2 (0, 0);
	}

	public Touch (int _id, Vector2 _position)
	{
		id = _id;
		position = _position;
	}

	public void Update (Vector2 _position, Vector2 _shape)
	{
		position = _position;
		shape = _shape;
	}
}


