using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public bool readyToStart;
    public int timeout;

    [SerializeField]
    public GameState gState;
    
    public Score sc;

    public UIController ui;

    public bool ready;
    public int playersReady;
    
    public void Update()
    {
        if (playersReady > 1)
        {
            playersReady = 0;
            //Change the stone icons and title colours
            ui.InitIconColours();
            for (int i = 0; i < 2; i++)
            {
                ui.playerNames[i].color = gState.stoneColours[gState.players[i].stoneColorIndex];
                ui.playerNames[i].text = gState.players[i].teamName.ToUpper();
                ui.scoreboardNames[i].text = gState.players[i].teamName.ToUpper();
                ui.scoreCanvasNames[i].text = gState.players[i].teamName.ToUpper();
            }

            timeout = 2;

            gState.currentEnd = 0;
            sc = new Score();
            StartCoroutine(Throws());
        }
    }

    public void SetPlayerWeight(float weight)
    {
        gState.players[0].weight = weight;
        gState.players[1].weight = weight;
    }
    public void SetPlayerDirection(float dir)
    {
        gState.players[gState.throwingPlayerIndex].direction = dir;
    }
    public void SetPlayerHandle(int handle)
    {
        gState.players[0].handle = handle;
        gState.players[1].handle = handle;
    }
    
    public void ThrowStone(int handle, float weight, float direction, int playerIndex)
    {
        GameObject toThrow = gState.playerStones[gState.players[playerIndex].stoneColorIndex];

        Stone stone = Instantiate(toThrow).GetComponent<Stone>();
        stone.weight = weight / 1000f;
        stone.transform.rotation = Quaternion.Euler(0.0f, direction, 0.0f);
        stone.handle = handle;
        stone.playerIndex = gState.throwingPlayerIndex;
        gState.currentStone = stone;
        gState.stones[gState.stonesThrown] = stone;
        stone.canvas = ui.scoreCanvas;
        stone.gc = this;
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
        ui.FinishedIntro();

        gState.stonesThrown = 0;
        for (gState.currentEnd = 1; gState.currentEnd < gState.ends + 1; gState.currentEnd++)
        {
            StartCoroutine(PlayEnd());
            ready = false;
            yield return new WaitUntil(() => ready);
        }
        //End of game unless scores tied
        if (sc.firstPlayerScore == sc.secondPlayerScore)
        {
            //Play another end
            StartCoroutine(PlayEnd());
            ready = false;
            yield return new WaitUntil(() => ready);
        }
        ui.scoreCanvas.gameObject.SetActive(false);
        //Display win/loss message
        ui.FinishedGame(gState.ends, sc.firstPlayerScore, sc.secondPlayerScore, sc.firstPlayerScore > sc.secondPlayerScore);
    }
    public bool SweepStone(bool sweep, Player p)
    {
        if (gState.currentStone == null)
            return false;
        if (p.Equals(gState.players[gState.currentStone.playerIndex]))
        {
            gState.currentStone.isBeingSwept = sweep;
        }
        else
        {
            Debug.Log("TRYING TO SWEEP OPPOSITION STONE");
        }
        return gState.currentStone.isBeingSwept;
    }
    void DisplayMessage()
    {
        ui.endCanvas.gameObject.SetActive(true);
    }
    protected void CleanUp()
    {
        gState.stonesThrown = 0;
        foreach (Stone s in gState.stones)
        {
            Destroy(s.gameObject);
        }
        gState.stones = new Stone[16];
    }
    protected IEnumerator PlayEnd()
    {
        //Play one end
        ui.endCanvas.gameObject.SetActive(true);
        ui.endCanvas.GetComponentInChildren<Text>().text = ui.endText.text;
        yield return new WaitForSeconds(2.0f);
        ui.endCanvas.gameObject.SetActive(false);
        for (int i = 0; i < 16; i++)
        {
            //Change the colour of the icons
            ui.ChangeIcon(gState.throwingPlayerIndex, i);
            gState.players[gState.throwingPlayerIndex].ChangeState(Curling.State.Ready);
            gState.players[1-gState.throwingPlayerIndex].ChangeState(Curling.State.Passive);
            
            //Wait until the user throws the stone
            while (gState.currentStone == null)
            {
                //Do nothing
                yield return null;
            }
            gState.stonesThrown++;
            //Wait until the stone stops to do anything else
            while (gState.currentStone.velocity != Vector3.zero)
            {
                //Do nothing
                yield return null;
            }

            //Wait
            yield return new WaitForSeconds(timeout);

            //Switch throwers;
            ui.HideIcon(gState.throwingPlayerIndex, i);
            gState.throwingPlayerIndex = 1 - gState.throwingPlayerIndex;
            
            
            //Reset current stone to null
            gState.currentStone = null;
        }
        yield return new WaitForSeconds(3);

        //Calculate who scored
        sc.CalculateScore(GetComponent<GameController>());
        CleanUp();

        ui.endsTitle.text = "Score after " + gState.currentEnd.ToString() + " end";
        if (gState.currentEnd != 1)
        {
            ui.endsTitle.text += "s";
        }
        ui.scoreboard.gameObject.SetActive(true);

        yield return new WaitUntil(() => ready);
    }
}
