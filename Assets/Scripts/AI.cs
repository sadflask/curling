using UnityEngine;
using System.Collections;
using System;

public abstract class AI {
    public GameController gc;
    protected UIController.Weight weight = UIController.Weight.Seven;
    protected int handle = -1;
    protected Stone closestRed = null;
    protected Stone closestBlue = null;
    protected Stone guard = null;

    protected enum GameState
    {
        SittingOpen,
        SittingGuarded,
        OpponentSittingOpen,
        OpponentSittingGuarded,
        EmptyHouseOpen,
        EmptyHouseGuarded
    }
    public abstract void DecideOnShot();
    //Draw to the other side of the house that the guard is on
    protected void DrawBehind()
    {
        Debug.Log("AI DRAWING BEHIND");
        Draw(1.4f * -1 * Mathf.Sign(guard.transform.position.x));
    }
    protected void Guard(Stone closestRed)
    {
        Debug.Log("AI GUARDING");
        //#TODO change this
        float line;
        if (closestRed.transform.position.x < 0)
        {
            line = closestRed.transform.position.x - 0.8f + (0.5f * (-closestRed.transform.position.x / 1.9f));
        }
        else
        {
            line = closestRed.transform.position.x + 0.8f - (0.5f * (closestRed.transform.position.x / 1.9f));
        }

        Vector3 toThrowAlong = new Vector3(line, 0, (17.375f + 20.75f));
        Vector3 centreline = new Vector3(0, 0, 1);
        float angleBetween = Vector3.Angle(toThrowAlong, centreline) * Mathf.Sign(line);

        gc.direction = angleBetween;
        weight = UIController.Weight.Two;
        handle = (int)Mathf.Sign(angleBetween) * -1;
    }
    protected void OpenHit()
    {
        Debug.Log("AI HITTING");
        //Find the stone to hit.
        float line;
        if (closestBlue.transform.position.x < 0)
        {
            line = closestBlue.transform.position.x - 0.4f + (0.5f * (-closestBlue.transform.position.x / 1.9f));
        }
        else
        {
            line = closestBlue.transform.position.x + 0.4f - (0.5f * (closestBlue.transform.position.x / 1.9f));
        }
        
        Vector3 toThrowAlong = new Vector3(line, 0, (17.375f + 20.75f));
        Vector3 centreline = new Vector3(0, 0, 1);
        float angleBetween = Vector3.Angle(toThrowAlong, centreline) * Mathf.Sign(line);

        gc.direction = angleBetween;
        weight = UIController.Weight.Control;
        handle = (int)Mathf.Sign(angleBetween) * -1;
    }
    protected void HitAround()
    {
        Debug.Log("AI HITTING BEHIND");
        //Find the stone to hit.
        float line;
        if (closestBlue.transform.position.x < 0)
        {
            //If the guard is inside the stone hit outside
            if (guard.transform.position.x > closestBlue.transform.position.x)
            {
                line = closestBlue.transform.position.x - 0.4f + (0.5f * (-closestBlue.transform.position.x / 1.9f));
                handle = 1;
            } else
            {
                line = closestBlue.transform.position.x + 0.6f + (0.5f * (-closestBlue.transform.position.x / 1.9f));
                handle = -1;
            }
        }
        else
        {
            if (guard.transform.position.x < closestBlue.transform.position.x)
            {
                line = closestBlue.transform.position.x + 0.4f - (0.5f * (closestBlue.transform.position.x / 1.9f));
                handle = -1;
            } else
            {
                line = closestBlue.transform.position.x - 0.6f - (0.5f * (closestBlue.transform.position.x / 1.9f));
                handle = 1;
            }
        }
        Vector3 toThrowAlong = new Vector3(line, 0, (17.375f + 20.75f));
        Vector3 centreline = new Vector3(0, 0, 1);
        float angleBetween = Vector3.Angle(toThrowAlong, centreline) * Mathf.Sign(line);

        gc.direction = angleBetween;
        weight = UIController.Weight.Control;
    }
    protected void Draw(float direction)
    {
        Debug.Log("AI DRAWING");
        weight = UIController.Weight.Seven;
        gc.direction = direction;
        handle = -1 * (int)Mathf.Sign(direction);
    }
    protected void Split()
    {
        Debug.Log("AI SPLITTING");
        weight = UIController.Weight.Six;
        gc.direction = 1f * Mathf.Sign(closestRed.transform.position.x);
        this.handle = -1 * (int)Mathf.Sign(gc.direction);
    }
    protected GameState DetermineState()
    {
        float redDistance = gc.sc.FindMinimumRed(gc.stones, out closestRed);
        float blueDistance = gc.sc.FindMinimumBlue(gc.stones, out closestBlue);
        if (redDistance > 1.98 && blueDistance > 1.98)
        {
            foreach (Stone s in gc.stones)
            {
                if (s != null)
                {
                    if (s.IsGuard())
                    {
                        guard = s;
                        return GameState.EmptyHouseGuarded;
                    }
                }
            }
            return GameState.EmptyHouseOpen;
        } else if (blueDistance < redDistance) 
        {
            if (IsGuarded(closestBlue, out guard))
            {
                return GameState.OpponentSittingGuarded;
            } else
            {
                return GameState.OpponentSittingOpen;
            }
        } else if (blueDistance >= redDistance)
        {
            if (IsGuarded(closestRed, out guard))
            {
                return GameState.SittingGuarded;
            } else
            {
                return GameState.SittingOpen;
            }
        } else
        {
            return GameState.EmptyHouseOpen;
        }
    }
    bool IsGuarded(Stone toGuard, out Stone guard)
    {
        foreach(Stone s in gc.stones)
        {
            if (s != null)
            {
                if ((Mathf.Abs(toGuard.transform.position.x - s.transform.position.x) < 0.2) &&
                    (toGuard.transform.position.z > s.transform.position.z))
                {
                    guard = s;
                    return true;
                }
            }
        }
        guard = null;
        return false;
    }
}
