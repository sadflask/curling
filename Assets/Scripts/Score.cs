using UnityEngine;
using System.Collections;

public class ScoreHelper {
    //Calculate the number of points scored
    public int firstPlayerScore;
    public int secondPlayerScore;

    GameController gameController;

    public int CalculateScore(GameController gameController)
    {
        this.gameController = gameController;
        GameState gameState = this.gameController.gameState;
        ScoreHelper scoreHelper = this.gameController.scoreHelper;
        UIController userInterface = this.gameController.userInterface;

        int score = 0;
        Stone closestFirstPlayerStone = scoreHelper.FindClosestFirstPlayerStone(gameState.stones);
        Stone closestSecondPlayerStone = scoreHelper.FindClosestSecondPlayerStone(gameState.stones);
        float minFirstPlayerDistance = closestFirstPlayerStone.Distance;
        float minSecondPlayerDistance = closestSecondPlayerStone.Distance;
        if (minSecondPlayerDistance < minFirstPlayerDistance)
        {
            if (minSecondPlayerDistance > 1.98)
            {
                score = 0;
                userInterface.secondPlayerScores[gameState.currentEnd-1].text = "0";
            }
            else
            {
                //Give the second player hammer
                gameState.throwingPlayerIndex = 1;
                score = scoreHelper.GetScoreValue(minFirstPlayerDistance, gameState.stones);
                string[] scores = userInterface.scoreText.text.Split(null);
                int currentSecondPlayerScore = int.Parse(scores[2]) + score;
                userInterface.scoreText.text = scores[0] + " - " + currentSecondPlayerScore.ToString();
                userInterface.secondPlayerTotal.text = currentSecondPlayerScore.ToString();
                userInterface.secondPlayerScores[gameState.currentEnd-1].text = score.ToString();
                secondPlayerScore = currentSecondPlayerScore;
            }
            userInterface.firstPlayerScores[gameState.currentEnd-1].text = "0";
        }
        else
        {
            if (minFirstPlayerDistance > 1.98)
            {
                score = 0;
                userInterface.firstPlayerScores[gameState.currentEnd-1].text = "0";
            }
            else
            {
                //Give the first player hammer
                gameState.throwingPlayerIndex = 0;
                score = scoreHelper.GetScoreValue(minSecondPlayerDistance, gameState.stones);
                string[] scores = userInterface.scoreText.text.Split(null);
                int currentFirstPlayerScore = int.Parse(scores[0]) + score;
                userInterface.scoreText.text = currentFirstPlayerScore.ToString() + " - " + scores[2];
                userInterface.firstPlayerTotal.text = currentFirstPlayerScore.ToString();
                userInterface.firstPlayerScores[gameState.currentEnd-1].text = score.ToString();
                firstPlayerScore = currentFirstPlayerScore;
            }
            userInterface.secondPlayerScores[gameState.currentEnd-1].text = "0";
        }
        return score;
    }
    //Iterate through stone raray to find closest secondplayer stone
    public Stone FindClosestSecondPlayerStone(ArrayList sortedStones)
    {
        for (int i = 0; i < sortedStones.Count; i++)
        {
            Stone currentStone = (Stone)sortedStones[i];
            if (currentStone != null)
            {
                if (currentStone.playerIndex == 1)
                {
                    return currentStone;
                }
            }
        }
        return null;
    }
     
    //Iterate through stone array to find closest player stone
    public Stone FindClosestFirstPlayerStone(ArrayList sortedStones)
    {
        for (int i=0;i<sortedStones.Count;i++)
        {
            Stone currentStone = (Stone)sortedStones[i];
            if (currentStone != null)
            {
                if (currentStone.playerIndex == 0)
                {
                    return currentStone;
                }
            }
        }
        return null;
    }
    //This function finds the number of stones closer than that of the other team
    int GetScoreValue(float minOtherTeamDistance, ArrayList stones)
    {
        int score = 0;
        foreach (Stone stone in stones)
        { 
          
            if (stone.Distance < minOtherTeamDistance && stone.Distance < 1.98)
            {
                score++;
                Debug.Log(string.Format("Stone is at: {0}", stone.transform.position));
                Debug.Log(string.Format("Distance = {0}, Min Distance = {1}", stone.Distance, minOtherTeamDistance));
            }

        }
        Debug.Log(score);
        return score;
    }
    bool IsGuarded(Stone toGuard, out Stone guard)
    {
        foreach (Stone stone in gameController.gameState.stones)
        {
            if (stone != null)
            {
                if ((Mathf.Abs(toGuard.transform.position.x - stone.transform.position.x) < 0.2) &&
                    (toGuard.transform.position.z > stone.transform.position.z))
                {
                    guard = stone;
                    return true;
                }
            }
        }
        guard = null;
        return false;
    }
}
