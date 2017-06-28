using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSpawner : DummySpawner
{

    // Use this for initialization
    protected override void Start()
    {
        
    }
    public Transform Engage()
    {
        return ThrowStone(poolers[0]);
    }
}
