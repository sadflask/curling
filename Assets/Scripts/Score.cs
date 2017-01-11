using UnityEngine;
using System.Collections;

public class Score {
    //Calculate the number of points scored
    public int blueScore;
    public int redScore;

    public int CalculateScore(GameController gc)
    {
        int score = 0;
        Stone blue, red;
        float minRed = gc.sc.FindMinimumRed(gc.stones, out red);
        float minBlue = gc.sc.FindMinimumBlue(gc.stones, out blue);
        if (minRed < minBlue)
        {
            if (minRed > 1.98)
            {
                score = 0;
                gc.redScores[gc.ends-1].text = "0";
            }
            else
            {
                gc.throwingTeam = "red";
                score = gc.sc.GetScoreValue(minBlue, gc.stones);
                string[] scores = gc.scoreText.text.Split(null);
                int currRed = int.Parse(scores[2]) + score;
                gc.scoreText.text = scores[0] + " - " + currRed.ToString();
                gc.redTotal.text = currRed.ToString();
                gc.redScores[gc.ends-1].text = score.ToString();
                redScore = currRed;
            }
            gc.blueScores[gc.ends-1].text = "0";
        }
        else
        {
            if (minBlue > 1.98)
            {
                score = 0;
                gc.blueScores[gc.ends-1].text = "0";
            }
            else
            {
                gc.throwingTeam = "blue";
                score = gc.sc.GetScoreValue(minRed, gc.stones);
                string[] scores = gc.scoreText.text.Split(null);
                int currBlue = int.Parse(scores[0]) + score;
                gc.scoreText.text = currBlue.ToString() + " - " + scores[2];
                gc.blueTotal.text = currBlue.ToString();
                gc.blueScores[gc.ends-1].text = score.ToString();
                blueScore = currBlue;
            }
            gc.redScores[gc.ends-1].text = "0";
        }
        return score;
    }

    //Iterate through stone array to find closest red stone
    public float FindMinimumRed(Stone[] stones, out Stone closestRed)
    {
        float min = 200;
        closestRed = null;
        foreach (Stone s in stones)
        {
            if (s!=null) {
                if (s.color == "red")
                {
                    float distance = (s.transform.position - new Vector3(0, 0.3f, 17.37f)).magnitude;
                    if (distance < min)
                    {
                        min = distance;
                        closestRed = s;
                    }
                }
            }
        }
        return min;
    }
    //Iterate through stone array to find closest blue stone
    public float FindMinimumBlue(Stone[] stones, out Stone closestBlue)
    {
        float min = 200;
        closestBlue = null;
        foreach (Stone s in stones)
        {
            if (s != null)
            {
                if (s.color == "blue")
                {
                    float distance = (s.transform.position - new Vector3(0, 0.3f, 17.37f)).magnitude;
                    if (distance < min)
                    {
                        min = distance;
                        closestBlue = s;
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
