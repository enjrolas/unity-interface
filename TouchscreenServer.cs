using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;

public class TouchscreenServer : MonoBehaviour
{
	public int listenPort = 7000;
	UdpClient udpClient; 	
	IPEndPoint endPoint;
	Osc.Parser osc = new Osc.Parser ();
	public GameObject player;
	public float leftBorder, rightBorder, topBorder, bottomBorder;
	public Boolean debug;
	Hashtable touches;
	void Start ()
	{
		touches = new Hashtable ();
		endPoint = new IPEndPoint (IPAddress.Any, listenPort);
		udpClient = new UdpClient (endPoint);
	}

	void Update ()
	{
		while (udpClient.Available > 0) {
			osc.FeedData (udpClient.Receive (ref endPoint));
		}

		while (osc.MessageCount > 0) {
			var msg = osc.PopMessage ();

			if (msg.path == "/newTouch") {
				int id = (int)msg.data [0];
//				touches[id]=new Touch(
			}

			if (msg.path == "/touchUp") {
				int id = (int)msg.data [0];

			}


			if(msg.path=="/touch")
			{
				float[] position = new float[2];
				position[1]=float.Parse(msg.data [1].ToString());
				position[0]=1-float.Parse(msg.data[2].ToString());
				if (debug) {
					Debug.Log (position [1] + " " + position [0]);
				}
				position [1] = map (position [1], 0, 1, leftBorder, rightBorder);
				position [0] = map (position [0], 0, 1, bottomBorder, topBorder);
				player.SendMessage ("OnPositionMessage", position);
			}

			//	Debug.Log (msg);
		}
	}
	// c#
	float map(float s, float a1, float a2, float b1, float b2)
	{
		return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}
}

public class Touch{
						public int id;
						public float x, y, diameter;
						public Touch()
						{
		x = 0;
		y = 0;
		diameter = 0;
						}
						public Touch(int _id, float _x, float _y)
						{
		x = _x;
		y = _y;
		id = _id;
		diameter = 1;
						}
						public void Update(float _x, float _y, float _diameter)
						{
		x = _x;
		y = _y;
		diameter = _diameter;
						}
}

