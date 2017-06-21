using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PNPGameController : GameController {

    public new void Update()
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

    protected override IEnumerator Throws()
    {
        Debug.Log("Called sub method");
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
        ui.FinishedPNPGame(gState.ends, sc.firstPlayerScore, sc.secondPlayerScore, sc.firstPlayerScore > sc.secondPlayerScore);
    }
    
    new IEnumerator PlayEnd()
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
            gState.players[gState.throwingPlayerIndex].EnableCam();
            gState.players[1-gState.throwingPlayerIndex].ChangeState(Curling.State.Passive);
            gState.players[1-gState.throwingPlayerIndex].DisableCam();

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
        sc.CalculateScore(GetComponent<PNPGameController>());
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
