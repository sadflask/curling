using UnityEngine;

public class Sweeper : MonoBehaviour {

    public GameController gc;
    public Player p;
    public bool isBeingSwept;

	// Use this for initialization
	void Start () {
        isBeingSwept = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(0))
        {
            //Requests to sweep the stone if it is not already being swept
            if (!isBeingSwept)
            {
                isBeingSwept = gc.SweepStone(true, p);
            }
        } else
        {
            //Requests to stop sweeping if it is being swept
            if (isBeingSwept)
            {
                isBeingSwept = gc.SweepStone(false, p);
            }
        }
	}
}
