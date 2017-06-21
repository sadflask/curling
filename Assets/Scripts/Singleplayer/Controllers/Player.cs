using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Curling
{

    public enum Weight
    {
        One = 3720,
        Two = 3780,
        Three = 3880,
        Four = 3910,
        Five = 3940,
        Six = 3955,
        Seven = 3970,
        Eight = 3985,
        Nine = 4010,
        Ten = 4030,
        Hack = 4060,
        Board = 4110,
        Control = 4250,
        Normal = 4320,
        Peel = 4500
    }
    public enum State
    {
        Waiting,
        Ready,
        Choosing,
        Passive,
        Skipping,
        Ended
    }
}
public abstract class Player : MonoBehaviour {

    public string teamName;
    public int stoneColorIndex;
    public int playerIndex;

    public float direction;
    public int handle;
    public float weight;
    
    public Curling.State state;

    public GameController gc;
    public GameState gs;

    public Camera cam;

    public abstract void DecideOnShot();

    public void Start()
    {
        gs = gc.gState;
    }
    public void DisableCam()
    {
        cam.enabled = false;
    }
    public void EnableCam()
    {
        cam.enabled = true;
    }


    public void Throw()
    {
        ThrowStone(handle, (float)weight, direction, playerIndex);
    }
    public void ThrowStone(int handle, float weight, float direction, int playerIndex)
    {
        gc.ThrowStone(handle, weight, direction, playerIndex);
    }

    public void ChangeState(Curling.State s)
    {
        state = s;
    }
    public virtual void LoadData(ConfigData config)
    {
        stoneColorIndex = config.colourIndices[playerIndex];
        teamName = config.teamNames[playerIndex];
        gc.playersReady++;
    }

    void Update()
    {
        if (state == Curling.State.Ready)
        {
            DecideOnShot();
            ChangeState(Curling.State.Choosing);
        }
        else if (state == Curling.State.Passive)
        {
            GoToSkip();
            ChangeState(Curling.State.Skipping);
        }
    }
    public void GoToSkip()
    {
        transform.position = gc.gState.skipPosition.transform.position;
        transform.rotation = gc.gState.skipPosition.transform.rotation;
    }
}
