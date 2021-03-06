﻿using System;
using UnityEngine;
using Curling;

public class NormalAI : AI {

    public override void DecideOnShot()
    {
       GameStateEnum gse = DetermineState();

        int score = (int.Parse(gc.ui.firstPlayerTotal.text) - int.Parse(gc.ui.secondPlayerTotal.text));
        //If the stone is the first stone
        if ( gs.stonesThrown == 0)
        {
            //If less than 2 up throw a guard
            if (score < 2)
            {
                weight = (float)Weight.Two;
                direction = 1.3f;
                handle = -1;
            }
            //If 2-3 up throw a draw
            else if (score < 4)
            {
                Draw(1.3f);
            }
            //If more than 4 up throw it through
            else
            {
                weight = (float)Weight.Control;
                direction = 0.8f;
                handle = -1;
            }
            //If the stone is the second stone
        }
        else
        {
            switch (gse)
            {
                case GameStateEnum.EmptyHouseOpen:
                    Draw(1.3f);
                    break;
                case GameStateEnum.EmptyHouseGuarded:
                    DrawBehind();
                    break;
                case GameStateEnum.OpponentSittingOpen:
                    OpenHit();
                    break;
                case GameStateEnum.OpponentSittingGuarded:
                    //if the opponent's stone is well guarded and you do not have hammer, try drawing instead of hitting
                    if (Mathf.Abs(guard.transform.position.x - closestP1Stone.transform.position.x) < 0.2 && gs.stonesThrown % 2 == 0)
                    {
                        DrawBehind();
                    }
                    else
                    {
                        //otherwise try to hit the stone out
                        HitAround();
                    }
                    break;
                case GameStateEnum.SittingOpen:
                    //Guard the ai's stone if trying to steal
                    if (gs.stonesThrown % 2 == 0)
                    {
                        Guard(closestP2Stone);
                    }
                    //If have hammer get a second shot in
                    else
                    {
                        Split();
                    }
                    break;
                case GameStateEnum.SittingGuarded:
                    Split();
                    break;
                default:
                    Draw(1.3f);
                    break;
            }
        }
        CheckAccuracy();
        gc.ui.SetHandle(handle);
        gc.ui.SetWeight(weight.ToString());
        Throw();
    }
}
