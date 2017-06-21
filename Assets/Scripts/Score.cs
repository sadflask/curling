using UnityEngine;
using System.Collections;

public class Score {
    //Calculate the number of points scored
    public int firstPlayerScore;
    public int secondPlayerScore;

    GameController gController;

    public int CalculateScore(GameController gc)
    {
        gController = gc;
        GameState gState = gController.gState;
        Score sc = gController.sc;
        UIController ui = gController.ui;

        int score = 0;
        Stone closestFirstPlayerStone, closestSecondPlayerStone;
        float minSecondPlayerDistance = sc.FindClosestSecondPlayerStone(gState.stones, out closestSecondPlayerStone);
        float minFirstPlayerDistance = sc.FindClosestFirstPlayerStone(gState.stones, out closestFirstPlayerStone);
        if (minSecondPlayerDistance < minFirstPlayerDistance)
        {
            if (minSecondPlayerDistance > 1.98)
            {
                score = 0;
                ui.secondPlayerScores[gState.currentEnd-1].text = "0";
            }
            else
            {
                //Give the second player hammer
                gState.throwingPlayerIndex = 1;
                score = sc.GetScoreValue(minFirstPlayerDistance, gState.stones);
                string[] scores = ui.scoreText.text.Split(null);
                int currentSecondPlayerScore = int.Parse(scores[2]) + score;
                ui.scoreText.text = scores[0] + " - " + currentSecondPlayerScore.ToString();
                ui.secondPlayerTotal.text = currentSecondPlayerScore.ToString();
                ui.secondPlayerScores[gState.currentEnd-1].text = score.ToString();
                secondPlayerScore = currentSecondPlayerScore;
            }
            ui.firstPlayerScores[gState.currentEnd-1].text = "0";
        }
        else
        {
            if (minFirstPlayerDistance > 1.98)
            {
                score = 0;
                ui.firstPlayerScores[gState.currentEnd-1].text = "0";
            }
            else
            {
                //Give the first player hammer
                gState.throwingPlayerIndex = 0;
                score = sc.GetScoreValue(minSecondPlayerDistance, gState.stones);
                string[] scores = ui.scoreText.text.Split(null);
                int currentFirstPlayerScore = int.Parse(scores[0]) + score;
                ui.scoreText.text = currentFirstPlayerScore.ToString() + " - " + scores[2];
                ui.firstPlayerTotal.text = currentFirstPlayerScore.ToString();
                ui.firstPlayerScores[gState.currentEnd-1].text = score.ToString();
                firstPlayerScore = currentFirstPlayerScore;
            }
            ui.secondPlayerScores[gState.currentEnd-1].text = "0";
        }
        return score;
    }
    //Iterate through stone array to find closest secondplayer stone
    public float FindClosestSecondPlayerStone(Stone[] stones, out Stone closestSecondPlayerStone)
    {
        float min = 200;
        closestSecondPlayerStone = null;
        foreach (Stone s in stones)
        {
            if (s!=null) {
                if (s.playerIndex == 1)
                {
                    float distance = (s.transform.position - new Vector3(0, 0.3f, 17.37f)).magnitude;
                    if (distance < min)
                    {
                        min = distance;
                        closestSecondPlayerStone = s;
                    }
                }
            }
        }
        return min;
    }
    //Iterate through stone array to find closest player stone
    public float FindClosestFirstPlayerStone(Stone[] stones, out Stone closestPlayerStone)
    {
        float min = 200;
        closestPlayerStone = null;
        foreach (Stone s in stones)
        {
            if (s != null)
            {
                if (s.playerIndex == 0)
                {
                    float distance = (s.transform.position - new Vector3(0, 0.3f, 17.37f)).magnitude;
                    if (distance < min)
                    {
                        min = distance;
                        closestPlayerStone = s;
                    }
                }
            }
        }
        return min;
    }
    //This function finds the number of stones closer than that of the other team
    int GetScoreValue(float minDist, Stone[] stones)
    {
        int score = 0;
        foreach (Stone s in stones)
        { 
            float distance = (s.transform.position - new Vector3(0, 0.3f, 17.37f)).magnitude;
            
            if (distance < minDist && distance < 1.98)
            {
                score++;
                Debug.Log(string.Format("Stone is at: {0}", s.transform.position));
                Debug.Log(string.Format("Distance = {0}, Min Distance = {1}", distance, minDist));
            }

        }
        Debug.Log(score);
        return score;
    }
}
