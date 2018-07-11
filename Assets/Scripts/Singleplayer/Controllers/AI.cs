using UnityEngine;
using System.Collections;
using System;

public abstract class AI : Player
{
    public int level;

    /// <summary>
    /// The difference between the player scores. A positive value means the AI is winning.
    /// </summary>
    protected int pointsUpBy;

    public override void LoadData(ConfigData configData)
    {
        level = configData.aiLevel;
        base.LoadData(configData);
    }
    protected void CheckAccuracy()
    {
        //W represents the chance of the weight being off, l the line.
        int w = UnityEngine.Random.Range(0, 100);
        int l = UnityEngine.Random.Range(0, 100);
        //These will be used to determine which direction the weight or line is off 
        int wDir = UnityEngine.Random.Range(0, 100);
        int lDir = UnityEngine.Random.Range(0, 100);

        Debug.Log(string.Format("Weight: {0}", w));
        Debug.Log(string.Format("Line: {0}", l));

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
    protected bool HasHammer
    {
        get
        {
            return gameState.stonesThrown % 2 == 1;
        }
    }
    protected bool BeforeLastTwoStones
    {
        get
        {
            return gameState.stonesThrown < 14;
        }
    }
    protected bool FreeGuardActive
    {
        get
        {
            return gameState.stonesThrown < 4;
        }
    }
    public bool IsAIStone(Stone stone)
    {
        return stone.playerIndex == 1;
    }
    public abstract override void DecideOnShot();

    //Draw to the other side of the house that the guard is on
    protected void DrawBehind(Stone guard)
    {
        Draw(1.5f * -1 * Mathf.Sign(guard.transform.position.x));
    }
    protected void Guard(Stone toGuard)
    {
        //#TODO change this
        float line;
        if (toGuard.transform.position.x < 0)
        {
            line = toGuard.transform.position.x - 1.0f;
        }
        else
        {
            line = toGuard.transform.position.x + 1.0f;
        }

        Vector3 toThrowAlong = new Vector3(line, 0, (17.375f + 20.75f));
        Vector3 centreline = new Vector3(0, 0, 1);
        float angleBetween = Vector3.Angle(toThrowAlong, centreline) * Mathf.Sign(line);

        direction = angleBetween;
        weight = (float)Curling.Weight.Two;
        handle = (int)Mathf.Sign(angleBetween) * -1;
    }
    protected void OpenHit(Stone toHit)
    {
        //Find the stone to hit.
        float line;
        if (toHit.transform.position.x < 0)
            line = toHit.transform.position.x - 0.35f;
        else
            line = toHit.transform.position.x + 0.35f;

        Vector3 toThrowAlong = new Vector3(line, 0, (17.375f + 20.75f));
        Vector3 centreline = new Vector3(0, 0, 1);
        float angleBetween = Vector3.Angle(toThrowAlong, centreline) * Mathf.Sign(line);

        direction = angleBetween;
        weight = (float)Curling.Weight.Control;
        handle = (int)Mathf.Sign(angleBetween) * -1;
    }
    protected void DoubleTakeout(Stone stone1, Stone stone2)
    {
        throw new NotImplementedException();
    }
    protected void HitAround(Stone toHit, Stone guard)
    {
        //Find the stone to hit.
        float line;
        if (toHit.transform.position.x < 0)
        {
            //If the guard is inside the stone hit outside
            if (guard.transform.position.x > toHit.transform.position.x)
            {
                line = toHit.transform.position.x - 0.35f;
                handle = 1;
            }
            else
            {
                line = toHit.transform.position.x + 0.35f;
                handle = -1;
            }
        }
        else
        {
            if (guard.transform.position.x < toHit.transform.position.x)
            {
                line = toHit.transform.position.x + 0.35f;
                handle = -1;
            }
            else
            {
                line = toHit.transform.position.x - 0.35f;
                handle = 1;
            }
        }
        Vector3 toThrowAlong = new Vector3(line, 0, (17.375f + 20.75f));
        Vector3 centreline = new Vector3(0, 0, 1);
        float angleBetween = Vector3.Angle(toThrowAlong, centreline) * Mathf.Sign(line);

        direction = angleBetween;
        weight = (float)Curling.Weight.Control;
    }
    protected void Hit(Stone toHit)
    {
        if (toHit.IsGuarded) HitAround(toHit, toHit.GetGuard());
        else OpenHit(toHit);
    }
    protected void Draw(float dir)
    {
        weight = (float)Curling.Weight.Seven;
        direction = dir;
        handle = -1 * (int)Mathf.Sign(direction);
    }
    protected void CentreGuard()
    {
        weight = (float)Curling.Weight.Two;
        direction = 1.3f;
        handle = -1;
    }
    protected void LongCentreGuard()
    {
        weight = (float)Curling.Weight.One;
        direction = 1.3f;
        handle = -1;
    }
    protected void CornerGuard(int handle)
    {
        weight = (float)Curling.Weight.Two;
        direction = 0;
        this.handle = handle;
    }
    protected void Split(Stone shot)
    {
        weight = (float)Curling.Weight.Six;
        direction = 1f * Mathf.Sign(shot.transform.position.x);
        handle = -1 * (int)Mathf.Sign(direction);
    }
    protected void DrawOnto(Stone shot)
    {
        throw new NotImplementedException();
    }
    
}
