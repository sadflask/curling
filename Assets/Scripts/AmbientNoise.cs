using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientNoise : MonoBehaviour {

    private AudioSource sweepNoise;
    public float sweepWaitBetween;
    private float nextSweep;
	// Use this for initialization
	void Start () {
        GetComponents<AudioSource>()[0].Play();
        sweepNoise = GetComponents<AudioSource>()[1];
        nextSweep = Time.time + sweepWaitBetween;
    }

    // Update is called once per frame
    public void Update()
    {
        if (Time.time > nextSweep)
        {
            sweepNoise.Play();
            nextSweep = Time.time + sweepWaitBetween;
        }
    }
}
