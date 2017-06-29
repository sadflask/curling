using UnityEngine;
using System.Collections;
using System;

public abstract class AI : Player {
    protected Stone closestP2Stone = null;
    protected Stone closestP1Stone = null;
    protected Stone guard = null;
    public int level;

    protected int score;

    protected enum GameStateEnum
    {
        SittingOpen,
        SittingGuarded,
        OpponentSittingOpen,
        OpponentSittingGuarded,
        EmptyHouseOpen,
        EmptyHouseGuarded
    }
    public override void LoadData(ConfigData cData)
    {
        level = cData.aiLevel;
        base.LoadData(cData);
    }
    protected void CheckAccuracy()
    {
        //W represents the chance of the weight being off, l the line.
        int w = UnityEngine.Random.Range(0, 100);
        int l = UnityEngine.Random.Range(0, 100);
        //These will be used to determine which direction the weight or line is off 
        int wDir = UnityEngine.Random.Range(0, 100);
        int lDir = UnityEngine.Random.Range(0, 100);

        Debug.Log(string.Format("Weight: {0}",w));
        Debug.Log(string.Format("Line: {0}",l));

        switch (level)
        {
            case 1:
                //This AI has 80% accuracy for weight and line
                if (w > 80)
                {
                    //Put the weight off by 2 zones
                    Debug.Log(string.Format("MISS! Should have been: {0}", weight));
                    weight = (wDir > 50) ? weight + 30 : weight - 30;
                    Debug.Log(string.Format("Was actually: {0}", weight));

                }
                if (l > 80)
                {
                    //Put the line off by .5 degrees
                    Debug.Log(string.Format("MISS! Should have been: {0}", direction));
                    direction = (lDir > 50) ? direction + 0.3f : direction - 0.3f;
                    Debug.Log(string.Format("Was actually: {0}", direction));
                }
                break;
            case 2:
                //This AI has 90% accuracy for weight and 90% accuracy for line
                if (w > 90)
                {
                    //Put the weight off by 1 zone
                    weight = (wDir > 50) ? weight + 15 : weight - 15;
                }
                if (l > 90)
                {
                    //Put the line off by .05 degrees
                    direction = (lDir > 50) ? direction + 0.15f : direction - 0.15f;
                }
                break;
            case 3:
                return;
            default:
                Debug.Log(string.Format("NO AI LEVEL SET!"));
                break;

        }
    }
    public abstract override void DecideOnShot();
    //Draw to the other side of the house that the guard is on
    protected void DrawBehind()
    {
        Draw(1.3f * -1 * Mathf.Sign(guard.transform.position.x));
    }
    protected void Guard(Stone closestAIStone)
    {
        //#TODO change this
        float line;
        if (closestAIStone.transform.position.x < 0)
        {
            line = closestAIStone.transform.position.x - 1.0f;
        }
        else
        {
            line = closestAIStone.transform.position.x + 1.0f;
        }

        Vector3 toThrowAlong = new Vector3(line, 0, (17.375f + 20.75f));
        Vector3 centreline = new Vector3(0, 0, 1);
        float angleBetween = Vector3.Angle(toThrowAlong, centreline) * Mathf.Sign(line);

        direction = angleBetween;
        weight = (float)Curling.Weight.Two;
        handle = (int)Mathf.Sign(angleBetween) * -1;
    }
    protected void OpenHit()
    {
        //Find the stone to hit.
        float line;
        if (closestP1Stone.transform.position.x < 0)
        {
            line = closestP1Stone.transform.position.x - 0.5f;
        }
        else
        {
            line = closestP1Stone.transform.position.x + 0.5f;
        }
        
        Vector3 toThrowAlong = new Vector3(line, 0, (17.375f + 20.75f));
        Vector3 centreline = new Vector3(0, 0, 1);
        float angleBetween = Vector3.Angle(toThrowAlong, centreline) * Mathf.Sign(line);

        direction = angleBetween;
        weight = (float)Curling.Weight.Control;
        handle = (int)Mathf.Sign(angleBetween) * -1;
    }
    protected void HitAround()
    {
        //Find the stone to hit.
        float line;
        if (closestP1Stone.transform.position.x < 0)
        {
            //If the guard is inside the stone hit outside
            if (guard.transform.position.x > closestP1Stone.transform.position.x)
            {
                line = closestP1Stone.transform.position.x - 0.5f;
                handle = 1;
            } else
            {
                line = closestP1Stone.transform.position.x + 0.5f;
                handle = -1;
            }
        }
        else
        {
            if (guard.transform.position.x < closestP1Stone.transform.position.x)
            {
                line = closestP1Stone.transform.position.x + 0.5f;
                 handle = -1;
            } else
            {
                line = closestP1Stone.transform.position.x - 0.5f;
                handle = 1;
            }
        }
        Vector3 toThrowAlong = new Vector3(line, 0, (17.375f + 20.75f));
        Vector3 centreline = new Vector3(0, 0, 1);
        float angleBetween = Vector3.Angle(toThrowAlong, centreline) * Mathf.Sign(line);

        direction = angleBetween;
        weight = (float)Curling.Weight.Control;
    }
    protected void Draw(float dir)
    {
        weight = (float)Curling.Weight.Seven;
        direction = dir;
        handle = -1 * (int)Mathf.Sign(direction);
    }
    protected void Split()
    {
        weight = (float)Curling.Weight.Six;
        direction = 1f * Mathf.Sign(closestP2Stone.transform.position.x);
        this.handle = -1 * (int)Mathf.Sign(direction);
    }
    protected GameStateEnum DetermineState()
    {
        float closestP2StoneDistance = gc.sc.FindClosestSecondPlayerStone(gc.gState.stones, out closestP2Stone);
        float closestP1StoneDistance = gc.sc.FindClosestFirstPlayerStone(gc.gState.stones, out closestP1Stone);
        if (closestP2StoneDistance > 1.98 && closestP1StoneDistance > 1.98)
        {
            foreach (Stone s in gc.gState.stones)
            {
                if (s != null)
                {
                    if (s.IsGuard())
                    {
                        guard = s;
                        return GameStateEnum.EmptyHouseGuarded;
                    }
                }
            }
            return GameStateEnum.EmptyHouseOpen;
        } else if (closestP1StoneDistance < closestP2StoneDistance) 
        {
            if (IsGuarded(closestP1Stone, out guard))
            {
                return GameStateEnum.OpponentSittingGuarded;
            } else
            {
                return GameStateEnum.OpponentSittingOpen;
            }
        } else if (closestP1StoneDistance >= closestP2StoneDistance)
        {
            if (IsGuarded(closestP2Stone, out guard))
            {
                return GameStateEnum.SittingGuarded;
            } else
            {
                return GameStateEnum.SittingOpen;
            }
        } else
        {
            return GameStateEnum.EmptyHouseOpen;
        }
    }
    bool IsGuarded(Stone toGuard, out Stone guard)
    {
        foreach(Stone s in gc.gState.stones)
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
