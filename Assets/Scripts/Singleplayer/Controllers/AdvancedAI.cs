using UnityEngine;
using Curling;

public class AdvancedAI : AI
{

    public override void DecideOnShot()
    {
        var sortedStones = gameController.gameState.stones;
        sortedStones.Sort();

        pointsUpBy = (gameController.scoreHelper.secondPlayerScore - gameController.scoreHelper.firstPlayerScore);
        var stonesThrown = gameState.stonesThrown;

        //#TODO Code this to ignore stones that are out of the house behind the tee line.
        var closestStone = (Stone)sortedStones[0];
        var i = 1;

        //If stone exists, but isn't in front of T or in house, find the next closest.
        //We don't care about stones past the Tee line that aren't in the house.
        while( closestStone != null && !closestStone.IsInHouse && !closestStone.IsGuard && i < stonesThrown)
        {
            closestStone = (Stone)sortedStones[i];
            i++;
        }

        Stone secondStone = null;
        if (i < stonesThrown)
            i++;
            secondStone = (Stone)sortedStones[i];
        while (secondStone != null && !secondStone.IsInHouse && !secondStone.IsGuard && i<stonesThrown)
        {
            secondStone = (Stone)sortedStones[i];
            i++;
        }

        //If the stone is the first stone
        if (stonesThrown == 0)
        {
            //If less than 2 up throw a guard
            if (pointsUpBy < 2)
            {
                CentreGuard();
            }
            //If 2-3 up throw a draw
            else if (pointsUpBy < 4)
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
            if (closestStone == null) //There is no stone in play. 
                NoStonesInPlay(stonesThrown);
            else if (secondStone == null) //There is one stone in play
                OneStoneInPlay(stonesThrown, closestStone);
            else //There are two or more stones in play
                MultipleStonesInPlay(stonesThrown, closestStone, secondStone);
        }
        CheckAccuracy();
    }
    private void NoStonesInPlay(int stonesThrown)
    {
        if (HasHammer)
        {
            if (BeforeLastTwoStones)
                CornerGuard(-1);
            else
                Draw(1.3f);
        }
        else
        {
            if (FreeGuardActive)
                CentreGuard();
            else
                Draw(1.3f);
        }
    }
    private void OneStoneInPlay(int stonesThrown, Stone closestStone)
    {
        if (closestStone.IsGuard)
        { //There is an empty house that is guarded.
            if (closestStone.IsCentreGuard)
            {
                if (HasHammer)
                {
                    if (BeforeLastTwoStones)
                    {
                        //Throw a corner guard on the opposite side to the centre guard
                        CornerGuard((int)(Mathf.Sign(closestStone.transform.position.x)));
                    }
                    else
                    { //If you have two or fewer stones left
                      //Draw around the guard
                        DrawBehind(closestStone);
                    }
                }
                else
                { //If the AI doesn't have hammer
                  //If in the first few stones throw another centre guard, otherwise draw around
                    if (FreeGuardActive) CentreGuard();
                    else
                        DrawBehind(closestStone);
                }

            }
            else
            { //If it's not a centre guard it must be a corner
                if (HasHammer)
                {
                    if (BeforeLastTwoStones)
                    {
                        //Throw a corner on the other side
                        CornerGuard((int)(Mathf.Sign(closestStone.transform.position.x)));
                    }
                    else
                    {
                        //Draw behind the corner guard
                        DrawBehind(closestStone);
                    }
                }
                else
                {
                    //If you don't have hammer draw behind it unless it's early.
                    if (FreeGuardActive) CentreGuard();
                    else DrawBehind(closestStone);
                }
            }
        }
        else
        { //Single stone in play is in the house
            if (isAIStone(closestStone))
            {
                //The AI is sitting, split the house no matter what
                Split(closestStone);
            }
            else
            { //Stone belongs to other team
                if (HasHammer) OpenHit(closestStone);
                else if (closestStone.IsBehindTee) DrawOnto(closestStone);
                else OpenHit(closestStone);
            }
        }
    }
    private void MultipleStonesInPlay(int stonesThrown, Stone closestStone, Stone secondStone)
    {
        if (isAIStone(closestStone) && isAIStone(secondStone))
        {//Both stones belong to AI
            if (closestStone.IsInHouse)
            {
                if (secondStone.IsInHouse)
                { //Both stones in house
                    //Guard either stone if they aren't guarded.
                    if (!closestStone.IsGuarded) Guard(closestStone);
                    else if (!secondStone.IsGuarded) Guard(secondStone);
                    //If both shots are guarded draw in.
                    else Split(closestStone);
                }
                else
                {
                    //Never guard one stone, always split house.
                    if (!closestStone.IsGuarded) DrawBehind(secondStone);
                    else Split(closestStone);
                }
            }
            else
            { //Neither stone is in the house
                //Draw behind either stone if they are guards. Prefer longer guard
                if (secondStone.IsGuard) DrawBehind(secondStone);
                else if (closestStone.IsGuard) DrawBehind(closestStone);
                else NoStonesInPlay(stonesThrown); //This shouldn't happen ever, but if it does treat it as having no stones in play.
            }
        }
        else if (!isAIStone(closestStone) && !isAIStone(secondStone))
        { //Both stones are player stones
            if (closestStone.IsInHouse && secondStone.IsInHouse) DoubleTakeout(closestStone, secondStone);
            else if (closestStone.IsInHouse) Hit(closestStone);
            //If they are centre guards and you have hammer, clear them.
            else
            {
                if (closestStone.IsCentreGuard && secondStone.IsCentreGuard)
                    DoubleTakeout(closestStone, secondStone);
                //Otherwise draw behind either stone that is a corner guard (unless it is the last two)
                else if (BeforeLastTwoStones)
                {
                    if (!closestStone.IsCentreGuard) DrawBehind(closestStone);
                    else DrawBehind(secondStone);
                }
                else
                {
                    //If it is the last two, draw behind the centre one.
                    if (closestStone.IsCentreGuard) DrawBehind(closestStone);
                    else DrawBehind(secondStone);
                }
            }
        } else if (isAIStone(closestStone) && !isAIStone(secondStone))
        { //AI sitting one, player next closest
            if (closestStone.IsInHouse && secondStone.IsInHouse)
            { //Hit the player's stone.
                Hit(secondStone);
            } else if(closestStone.IsInHouse)
            {
                //Check if the player's stone is guarding the AI stone.
                if (closestStone.GetGuard() == secondStone)
                {
                    Split(closestStone);
                } else
                {
                    DrawBehind(secondStone);
                }
            }
        } else
        { //Player sitting one, AI next closest
            Hit(closestStone);
        }
    }
}
