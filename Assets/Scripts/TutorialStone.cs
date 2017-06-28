using UnityEngine;

public class TutorialStone : DummyStone {

    protected override void OnEnable()
    {
        isCurling = true;
        //Give the stone a random weight and line and handle.
        weight = 3.965f;
        rotation = 1.2f;
        handle = -1;
        transform.rotation = Quaternion.Euler(0, rotation, 0);
        velocity = weight * transform.forward;
    }
}
