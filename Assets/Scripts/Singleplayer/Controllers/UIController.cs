using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

    public Canvas endCanvas, introCanvas, scoreCanvas, weightCanvas, lineCanvas, scoreboard, handleCanvas, resultCanvas;

    public Text scoreText, endText, secondPlayerTotal, firstPlayerTotal, endsTitle;

    public Text[] playerNames, scoreboardNames, scoreCanvasNames, firstPlayerScores, secondPlayerScores;

    public Image[] firstPlayerIcons, secondPlayerIcons;

    public GameController gc;
    
    public GameObject line;
    public Color lastColor;
    public Color buttonOff;

    public Sprite stone, rightDown, leftDown;

    //Changes camera
    public void SetCanvasCamera(Canvas canvas, Camera cameraToSet)
    {
        canvas.worldCamera = cameraToSet;
    }
    public void FinishedIntro()
    {
        introCanvas.gameObject.SetActive(false);
        scoreCanvas.gameObject.SetActive(true);
    }
    public void FinishedGame(int ends, int playerScore, int secondPlayerScore, bool won)
    {
        scoreCanvas.gameObject.SetActive(false);
        //Display win/loss message
        resultCanvas.gameObject.SetActive(true);
        foreach (Text t in resultCanvas.GetComponentsInChildren<Text>())
        {
            switch (t.name)
            {
                case "Result":
                    if (won)
                        t.text = "VICTORY!";
                    else
                        t.text = "DEFEAT";
                    break;
                case "End":
                    t.text = (ends).ToString();
                    break;
                case "Player Score":
                    t.text = playerScore.ToString();
                    break;
                case "Opponent Score":
                    t.text = secondPlayerScore.ToString();
                    break;
            }
        }
    }
    public void FinishedPNPGame(int ends, int playerScore, int secondPlayerScore, bool fpWon)
    {
        scoreCanvas.gameObject.SetActive(false);
        //Display win/loss message
        resultCanvas.gameObject.SetActive(true);
        foreach (Text t in resultCanvas.GetComponentsInChildren<Text>())
        {
            switch (t.name)
            {
                case "Result":
                    if (fpWon)
                        t.text = "Player 1 Wins!";
                    else
                        t.text = "Player 2 Wins!";
                    break;
                case "End":
                    t.text = (ends).ToString();
                    break;
                case "Player Score":
                    t.text = playerScore.ToString();
                    break;
                case "Opponent Score":
                    t.text = secondPlayerScore.ToString();
                    break;
            }
        }
    }
    public void StartingEnd()
    {
        ShowEndCanvas();
        endCanvas.GetComponentInChildren<Text>().text = endText.text;
    }
    public void ShowEndCanvas()
    {
        endCanvas.gameObject.SetActive(true);
    }
    public void HideEndCanvas()
    {
        endCanvas.gameObject.SetActive(false);
    }
    //Hides all the canvases except the shot type select one.
    public void BackToWeight()
    {
        weightCanvas.gameObject.SetActive(true);
        lineCanvas.gameObject.SetActive(false);
        line.SetActive(false);
    }
    public void ConfirmWeight()
    {
        if (gc.gState.players[gc.gState.throwingPlayerIndex].weight == 0)
        {
            //Popup about how user can't do that
            SetWeight("Seven");
        }
        else
        {
            weightCanvas.gameObject.SetActive(false);
            line.SetActive(true);
            SetCanvasCamera(lineCanvas, gc.gState.players[gc.gState.throwingPlayerIndex].cam);
            lineCanvas.gameObject.SetActive(true);
        }
    }
    public void ConfirmLine()
    {
        lineCanvas.gameObject.SetActive(false);
        line.SetActive(false);
        lineCanvas.GetComponentInChildren<LineSelector>().ToggleSelected();
        lineCanvas.GetComponentInChildren<LineSelector>().selected = false;
        SetCanvasCamera(handleCanvas, gc.gState.players[gc.gState.throwingPlayerIndex].cam);
        handleCanvas.gameObject.SetActive(true);
    }
    public void SetWeight(string weight)
    {
        if (weight.Contains("."))
        {
            gc.SetPlayerWeight(float.Parse(weight));
        }
        else
        {
            Curling.Weight weightEnum = (Curling.Weight)Enum.Parse(typeof(Curling.Weight), weight);
            gc.SetPlayerWeight((float)weightEnum);
        }
        Color tempColor = lastColor;
        //Enable all buttons that weren't pushed and disable the one that was
        foreach (Button b in weightCanvas.GetComponentsInChildren<Button>(true))
        {

            if (b.name == weight)
            {
                b.enabled = false;
                //Don't change color if button was the one previously pressed
                if (b.image.color != buttonOff)
                {
                    lastColor = b.image.color;
                }
                b.image.color = buttonOff;
            }
            else
            {
                if(b.image.color == buttonOff)
                {
                    b.image.color = tempColor;
                    b.enabled = true;
                }
            }
        }
    }
    public void SetHandle(int handle)
    {
        gc.SetPlayerHandle(handle);
        foreach (Button b in handleCanvas.GetComponentsInChildren<Button>())
        {
            if (b.name != "Back" && b.name != "Confirm")
            {
                if (b.name == "Left")
                {
                    if (handle == 1)
                    {
                        b.image.sprite = leftDown;
                    }
                    else
                    {
                        b.image.sprite = stone;
                    }
                }
                else
                {
                    if (handle == -1)
                    {
                        b.image.sprite = rightDown;
                    }
                    else
                    {
                        b.image.sprite = stone;
                    }
                }
            }
        }
    }
    public void ViewHouse()
    {
        for(int i=0;i<weightCanvas.transform.childCount;i++)
        {
            Transform t = weightCanvas.transform.GetChild(i);
            if (t.CompareTag("ViewHouse"))
            {
                t.gameObject.SetActive(true);
                if(t.GetChild(0).GetComponent<Text>().text == "View House")
                {
                    t.GetChild(0).GetComponent<Text>().text = "Hide House";
                } else
                {
                    t.GetChild(0).GetComponent<Text>().text = "View House";
                }
            }
            else
            {
                t.gameObject.SetActive(!t.gameObject.activeSelf);
            }
        }
    }
    public void BackToLine()
    {
        handleCanvas.gameObject.SetActive(false);
        lineCanvas.gameObject.SetActive(true);
        line.SetActive(true);
    }
    public void ConfirmHandle()
    {
        handleCanvas.gameObject.SetActive(false);
        gc.gState.players[gc.gState.throwingPlayerIndex].Throw();
    }
    public void ToNextEnd()
    {
        scoreboard.gameObject.SetActive(false);
        gc.ready = true;
        InitIconColours();
        if (gc.gState.currentEnd == 1)
            endText.text = "2nd End";
        else if (gc.gState.currentEnd == 2)
            endText.text = "3rd End";
        else if (gc.gState.currentEnd == 3)
            endText.text = "4th End";
        else if (gc.gState.currentEnd == 10)
            endText.text = "Finished";
        else
            endText.text = (gc.gState.currentEnd + 1).ToString() + "th End";
    }
    public void ToGameOptions()
    {
        SceneManager.LoadScene(1);
    }
    public void ToPNPGameOptions()
    {
        SceneManager.LoadScene(3);
    }
    public void ToMain()
    {
        SceneManager.LoadScene(0);
    }
    public void ToTutorial()
    {
        SceneManager.LoadScene(5);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void ChangeIcon(int playerIndex, int stone)
    {
        if (playerIndex == 0)
        {
            firstPlayerIcons[stone / 2].color = new Color(
                    firstPlayerIcons[stone / 2].color.r,
                    firstPlayerIcons[stone / 2].color.g,
                    firstPlayerIcons[stone / 2].color.b,
                    1 / 2.55f);
        }
        else
        {
            secondPlayerIcons[stone / 2].color = new Color(
                   secondPlayerIcons[stone / 2].color.r,
                   secondPlayerIcons[stone / 2].color.g,
                   secondPlayerIcons[stone / 2].color.b,
                   1 / 2.55f);
        }
    }
    public void HideIcon(int playerIndex, int stone)
    {
        if (playerIndex == 0)
        {
            firstPlayerIcons[stone / 2].color = new Color(0, 0, 0, 0);
        }
        else
        {
            secondPlayerIcons[stone / 2].color = new Color(0, 0, 0, 0);
        }
    }
    public void InitIconColours()
    {
        for (int i = 0; i < 8; i++)
        {
            firstPlayerIcons[i].color = gc.gState.stoneColours[gc.gState.players[0].stoneColorIndex];
            secondPlayerIcons[i].color = gc.gState.stoneColours[gc.gState.players[1].stoneColorIndex];
        }
    }
    public void UpdatePlayerScore(int playerIndex, int score, int end)
    {
        if (playerIndex==0)
        {
            firstPlayerScores[end - 1].text = score.ToString();
            secondPlayerScores[end - 1].text = "0";
        } else
        {
            firstPlayerScores[end - 1].text = "0";
            secondPlayerScores[end - 1].text = score.ToString(); ;
        }
    }
}
