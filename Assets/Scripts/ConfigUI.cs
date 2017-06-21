using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfigUI : MonoBehaviour {
    public ConfigData cData;
    public Button[] P1ColourButtons;
    public Button[] P2ColourButtons;
    public Canvas[] playerOptions;
    public Button[] AILevelButtons;
    public Button[] endButtons;
    public Color[] difficultyColours;
    
    public void setP1Colour(string args)
    {
        string[] parts = args.Split('.');
        int colourIndex = int.Parse(parts[0]);
        int playerIndex = int.Parse(parts[1]);
        cData.colourIndices[playerIndex] = colourIndex;
        for (int i = 0; i < P1ColourButtons.Length; i++)
        {

            if (!(P1ColourButtons[i].name == args))
            {
                P1ColourButtons[i].GetComponentInChildren<Text>().text = "";
                P2ColourButtons[i].GetComponentInChildren<Text>().text = "";
            }
            else
            {
                P1ColourButtons[i].GetComponentInChildren<Text>().text = "-";
                P2ColourButtons[i].GetComponentInChildren<Text>().text = "X";
            }
        }
    }
    public void setP2Colour(string args)
    {
        //First thing to check is if you are trying to set the same colour as your opponent
        foreach(Button b in P2ColourButtons)
        {
            if (b.GetComponentInChildren<Text>().text == "X" && b.name == args)
            {
                return;
            }
        }
       
        for (int i = 0; i < P2ColourButtons.Length; i++)
        {
            if (!(P2ColourButtons[i].name == args))
            {
                if (P2ColourButtons[i].GetComponentInChildren<Text>().text != "X")
                {
                    P2ColourButtons[i].GetComponentInChildren<Text>().text = "";
                }
            }
            else
            {
                P2ColourButtons[i].GetComponentInChildren<Text>().text = "-";
            }
        }
        string[] parts = args.Split('.');
        int colourIndex = int.Parse(parts[0]);
        int playerIndex = int.Parse(parts[1]);
        cData.colourIndices[playerIndex] = colourIndex;
    }
    public void setP1Name(string name)
    {
        cData.teamNames[0] = name;
    }
    public void setP2Name(string name)
    {
        cData.teamNames[1] = name;
    }
    public void setEnds(string ends)
    {
        foreach (Button b in endButtons)
        {
            if (b.name == ends)
            {
                b.GetComponent<Image>().color = Color.gray;
            } else
            {
                b.GetComponent<Image>().color = Color.white;
            }
        }
        cData.numEnds = int.Parse(ends);
    }
    public void toGame(bool AI)
    {
        if (cData.numEnds != 0 
            && cData.colourIndices[0] != -1 && cData.colourIndices[1] != -1
            && cData.teamNames[0] != "" && cData.teamNames[0] != "" )
        {
            if (AI)
                if (cData.aiLevel == 0)
                    return;
            SceneManager.LoadScene(2);
        }
    }
    public void toPNPGame()
    {
        if (cData.numEnds != 0
            && cData.colourIndices[0] != -1 && cData.colourIndices[1] != -1
            && cData.teamNames[0] != "" && cData.teamNames[0] != "")
        {
            SceneManager.LoadScene(4);
        }
    }
    public void ToP2settings()
    {
        playerOptions[0].gameObject.SetActive(false);
        playerOptions[1].gameObject.SetActive(true);
    }
    public void ToP1Settings()
    {
        playerOptions[0].gameObject.SetActive(true);
        playerOptions[1].gameObject.SetActive(false);
    }
    public void SetAIDifficulty(int difficulty)
    {
        foreach (Button b in AILevelButtons)
        {
            if (b.name == difficulty.ToString())
            {
                b.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                b.GetComponent<Image>().color = difficultyColours[int.Parse(b.name)-1];
            }
        }
        cData.aiLevel = difficulty;
    }
}
