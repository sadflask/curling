using UnityEngine;
using System.Collections;

public class NormalAI : AI {

    public override void DecideOnShot()
    {
        GameState gs = DetermineState();
        Debug.Log(gs);

        int score = (int.Parse(gc.redTotal.text) - int.Parse(gc.blueTotal.text));
        //If the stone is the first stone
        if (gc.stonesThrown == 0)
        {
            //If less than 2 up throw a guard
            if (score < 2)
            {
                weight = UIController.Weight.Two;
                gc.direction = 1.4f;
                handle = -1;
            }
            //If 2-3 up throw a draw
            else if (score < 4)
            {
                Draw(1.4f);
            }
            //If more than 4 up throw it through
            else
            {
                weight = UIController.Weight.Control;
                gc.direction = 0.8f;
                handle = -1;
            }
            //If the stone is the second stone
        }
        else
        {
            Debug.Log(string.Format("AI STATE: {0}", gs));
            switch (gs)
            {
                case GameState.EmptyHouseOpen:
                    Draw(1.4f);
                    break;
                case GameState.EmptyHouseGuarded:
                    DrawBehind();
                    break;
                case GameState.OpponentSittingOpen:
                    OpenHit();
                    break;
                case GameState.OpponentSittingGuarded:
                    //if the opponent's stone is well guarded and you do not have hammer, try drawing instead of hitting
                    if (Mathf.Abs(guard.transform.position.x - closestBlue.transform.position.x) < 0.2 && gc.stonesThrown % 2 == 0)
                    {
                        DrawBehind();
                    }
                    else
                    {
                        //otherwise try to hit the stone out
                        HitAround();
                    }
                    break;
                case GameState.SittingOpen:
                    //Guard the ai's stone if trying to steal
                    if (gc.stonesThrown % 2 == 0)
                    {
                        Guard(closestRed);
                    }
                    //If have hammer get a second shot in
                    else
                    {
                        Split();
                    }
                    break;
                case GameState.SittingGuarded:
                    Split();
                    break;
                default:
                    Draw(1.4f);
                    break;
            }
        }
        gc.ui.SetHandle(handle);
        gc.ui.SetWeight(weight.ToString());
        gc.ThrowStone();
    }
}
