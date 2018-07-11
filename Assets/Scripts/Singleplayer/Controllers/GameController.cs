﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public bool readyToStart;
    public int timeout;

    [SerializeField]
    public GameState gameState;
    
    public ScoreHelper scoreHelper;

    public UIController userInterface;

    public bool ready;
    public int playersReady;
    
    public void Update()
    {
        if (playersReady > 1)
        {
            playersReady = 0;
            //Change the stone icons and title colours
            userInterface.InitIconColours();
            for (int i = 0; i < 2; i++)
            {
                userInterface.playerNames[i].color = gameState.stoneColours[gameState.players[i].stoneColorIndex];
                userInterface.playerNames[i].text = gameState.players[i].teamName.ToUpper();
                userInterface.scoreboardNames[i].text = gameState.players[i].teamName.ToUpper();
                userInterface.scoreCanvasNames[i].text = gameState.players[i].teamName.ToUpper();
            }

            timeout = 2;

            gameState.currentEnd = 0;
            scoreHelper = new ScoreHelper();
            StartCoroutine(Throws());
        }
    }

    public void SetPlayerWeight(float weight)
    {
        gameState.players[0].weight = weight;
        gameState.players[1].weight = weight;
    }
    public void SetPlayerDirection(float direction)
    {
        gameState.players[gameState.throwingPlayerIndex].direction = direction;
    }
    public void SetPlayerHandle(int handle)
    {
        gameState.players[0].handle = handle;
        gameState.players[1].handle = handle;
    }
    
    public void ThrowStone(int handle, float weight, float direction, int playerIndex)
    {
        GameObject toThrow = gameState.playerStones[gameState.players[playerIndex].stoneColorIndex];

        Stone stone = Instantiate(toThrow).GetComponent<Stone>();
        stone.weight = weight / 1000f;
        stone.transform.rotation = Quaternion.Euler(0.0f, direction, 0.0f);
        stone.handle = handle;
        stone.playerIndex = gameState.throwingPlayerIndex;
        gameState.currentStone = stone;
        gameState.stones[gameState.stonesThrown] = stone;
        stone.canvas = userInterface.scoreCanvas;
        stone.gameController = this;
        return;
    }
    
    protected virtual IEnumerator Throws()
    {
        Debug.Log("Called base method");

        /*//Do intro sequence
        for (int i = 0; i < 400; i++)
        {
            endCam.transform.RotateAround(Vector3.zero, Vector3.up, 45 * 0.01f);
            endCam.transform.position = new Vector3(endCam.transform.position.x, endCam.transform.position.y-0.02f, endCam.transform.position.z);
            //Do something
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1);*/
        userInterface.FinishedIntro();

        gameState.stonesThrown = 0;
        for (gameState.currentEnd = 1; gameState.currentEnd < gameState.ends + 1; gameState.currentEnd++)
        {
            StartCoroutine(PlayEnd());
            ready = false;
            yield return new WaitUntil(() => ready);
        }
        //End of game unless scores tied
        if (scoreHelper.firstPlayerScore == scoreHelper.secondPlayerScore)
        {
            //Play another end
            StartCoroutine(PlayEnd());
            ready = false;
            yield return new WaitUntil(() => ready);
        }
        userInterface.scoreCanvas.gameObject.SetActive(false);
        //Display win/loss message
        userInterface.FinishedGame(gameState.ends, scoreHelper.firstPlayerScore, scoreHelper.secondPlayerScore, scoreHelper.firstPlayerScore > scoreHelper.secondPlayerScore);
    }
    public bool SweepStone(bool sweep, Player p)
    {
        if (gameState.currentStone == null)
            return false;
        if (p.Equals(gameState.players[gameState.currentStone.playerIndex]))
        {
            gameState.currentStone.isBeingSwept = sweep;
            if (sweep)
                gameState.players[gameState.currentStone.playerIndex].Sweep();
            else
                gameState.players[gameState.currentStone.playerIndex].StopSweep();
        }
        else
        {
            Debug.Log("TRYING TO SWEEP OPPOSITION STONE");
        }
        return gameState.currentStone.isBeingSwept;
    }
    void DisplayMessage()
    {
        userInterface.endCanvas.gameObject.SetActive(true);
    }
    protected void CleanUp()
    {
        gameState.stonesThrown = 0;
        foreach (Stone s in gameState.stones)
        {
            Destroy(s.gameObject);
        }
        gameState.stones.Clear();
    }
    protected IEnumerator PlayEnd()
    {
        //Play one end
        userInterface.endCanvas.gameObject.SetActive(true);
        userInterface.endCanvas.GetComponentInChildren<Text>().text = userInterface.endText.text;
        yield return new WaitForSeconds(2.0f);
        userInterface.endCanvas.gameObject.SetActive(false);
        for (int i = 0; i < 16; i++)
        {
            //Change the colour of the icons
            userInterface.ChangeIcon(gameState.throwingPlayerIndex, i);
            gameState.players[gameState.throwingPlayerIndex].ChangeState(Curling.State.Ready);
            gameState.players[1-gameState.throwingPlayerIndex].ChangeState(Curling.State.Passive);
            
            //Wait until the user throws the stone
            while (gameState.currentStone == null)
            {
                //Do nothing
                yield return null;
            }
            gameState.stonesThrown++;
            //Wait until the stone stops to do anything else
            while (gameState.currentStone.velocity != Vector3.zero)
            {
                //Do nothing
                yield return null;
            }
            //Wait
            yield return new WaitForSeconds(timeout);

            //Switch throwers;
            userInterface.HideIcon(gameState.throwingPlayerIndex, i);
            gameState.throwingPlayerIndex = 1 - gameState.throwingPlayerIndex;
            gameState.players[1 - gameState.throwingPlayerIndex].StopSweep();
            
            //Reset current stone to null
            gameState.currentStone = null;
        }
        yield return new WaitForSeconds(3);

        //Calculate who scored
        scoreHelper.CalculateScore(GetComponent<GameController>());
        CleanUp();

        userInterface.endsTitle.text = "Score after " + gameState.currentEnd.ToString() + " end";
        if (gameState.currentEnd != 1)
        {
            userInterface.endsTitle.text += "s";
        }
        userInterface.scoreboard.gameObject.SetActive(true);

        yield return new WaitUntil(() => ready);
    }
}
