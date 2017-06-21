using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CameraFollow : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       
        transform.position = GetComponentInParent<Transform>().position;
        transform.rotation = GetComponentInParent<Transform>().rotation;
	}
}
