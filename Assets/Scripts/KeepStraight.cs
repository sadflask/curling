using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepStraight : MonoBehaviour {

    private Transform t;
    public Transform parentToAlignTo;

	// Use this for initialization
	void Start () {
        t = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        t.LookAt(parentToAlignTo.GetComponent<Stone>().velocity);
        t.RotateAround(t.position, t.right, -90f);
	}
}
