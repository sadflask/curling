using UnityEngine;
using System.Collections;

public class Score {
    //Calculate the number of points scored
    private static Score sc;

    public static int score(GameController gc)
    {
        sc = new Score(); 
        int score = 0;
        float minRed = sc.findMinimumRed(gc.stones);
        float minBlue = sc.findMinimumBlue(gc.stones);
        if (minRed < minBlue)
        {
            if (minRed > 190)
            {
                score = 0;
            }
            else
            {
                gc.throwingTeam = "red";
                score = sc.getScore(minBlue, gc.stones);
                string[] scores = gc.scoreText.text.Split(null);
                int currRed = int.Parse(scores[2]) + score;
                gc.scoreText.text = scores[0] + " - " + currRed.ToString();
            }
        }
        else
        {
            if (minBlue > 190)
            {
                score = 0;
            }
            else
            {
                gc.throwingTeam = "blue";
                score = sc.getScore(minRed, gc.stones);
                string[] scores = gc.scoreText.text.Split(null);
                int currBlue = int.Parse(scores[2]) + score;
                gc.scoreText.text = currBlue.ToString() + " - " + scores[2];
            }
        }
        return score;
    }

    //Iterate through stone array to find closest red stone
    float findMinimumRed(Stone[] stones)
    {
        float min = 200;
        foreach (Stone s in stones)
        {
            if (s.color == "red")
            {
                float distance = (s.transform.position - new Vector3(0, 0.3f, 39.62f)).magnitude;
                if (distance < min)
                {
                    min = distance;
                }
            }
        }
        return min;
    }
    //Iterate through stone array to find closest blue stone
    float findMinimumBlue(Stone[] stones)
    {
        float min = 200;
        foreach (Stone s in stones)
        {
            if (s.color == "blue")
            {
                float distance = (s.transform.position - new Vector3(0, 0.3f, 39.62f)).magnitude;
                if (distance < min)
                {
                    min = distance;
                }
            }
        }
        return min;
    }

    //This function finds the number of stones closer than that of the other team
    int getScore(float minDist, Stone[] stones)
    {
        int score = 0;
        foreach (Stone s in stones)
        {
            float distance = (s.transform.position - new Vector3(0, 0.3f, 39.62f)).magnitude;
            if (distance < minDist)
            {
                score++;
            }

        }
        return score;
    }

}
