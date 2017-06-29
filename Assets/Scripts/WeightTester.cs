using UnityEngine;

public class WeightTester : DummyStone
{
    public float w;
    protected override void OnEnable()
    {
        isCurling = true;
        //Give the stone a weight and line and handle.
        weight = w; 
        rotation = 1.6f;
        handle = -1;
        transform.rotation = Quaternion.Euler(0, rotation, 0);
        velocity = weight * transform.forward;
    }
}