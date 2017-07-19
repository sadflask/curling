using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Curling
{

    public enum Weight
    {
        One = 1980,
        Two = 2020,
        Three = 2090,
        Four = 2130,
        Five = 2155,
        Six = 2175,
        Seven = 2190,
        Eight = 2205,
        Nine = 2225,
        Ten = 2250,
        Hack = 2310,
        Board = 2400,
        Control = 2500,
        Normal = 2650,
        Peel = 2800
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

    public AudioSource sweepNoise;
    public Camera cam;

    public abstract void DecideOnShot();
    public void Sweep()
    {
        sweepNoise.Play();
    }
    public void StopSweep()
    {
        sweepNoise.Stop();
    }
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
