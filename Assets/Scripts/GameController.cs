using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public GameObject blue, red;
    public UIController ui;
    public int stonesThrown;
    public Stone[] stones = new Stone[16];
    public string throwingTeam = "red";
    public Camera endCam;
    public Camera topCam;
    public int timeout;
    public Canvas scoreCanvas;
    public Canvas typeCanvas;
    public Canvas endCanvas;
    public Image[] blueIcons, redIcons;
    public Text scoreText, endText;
    public float weight, direction;
    public Stone stone = null;
    public int handle;
    public Text[] redScores;
    public Text[] blueScores;
    public Text redTotal;
    public Text blueTotal;
    public int ends;
    public Canvas scoreboard;
    public Text endsTitle;
    public bool ready;
    private AI ai;
    public Canvas intro;
    public Score sc;

    void Start()
    {
        timeout = 2;
        ends = 0;
        ai = new NormalAI();
        ai.gc = this;
        sc = new Score();
        StartCoroutine(Throws());
       
    }

    public Stone ThrowStone()
    {
        GameObject toThrow;
        if (throwingTeam == "blue")
        {
            toThrow = blue;
        } else
        {
            toThrow = red;
        }
        stone = Instantiate(toThrow).GetComponent<Stone>();
        stone.force = weight/1000f;
        stone.transform.rotation = Quaternion.Euler(0.0f, direction, 0.0f);
        stone.handle = handle;
        stone.color = throwingTeam;
        stones[stonesThrown] = stone;
        stone.endCam = endCam;
        stone.topCam = topCam;
        stone.canvas = scoreCanvas;
        stone.gc = this;

        endCam.enabled = true;
        topCam.enabled = false;
        scoreCanvas.worldCamera = endCam;
        return stone;
        
    }
    IEnumerator Throws()
    {
        endCam.enabled = true;
        topCam.enabled = false;
        endCam.transform.position = new Vector3(0, 13, 22.25f);
        endCam.transform.rotation = Quaternion.Euler(new Vector3(30, 180, 0));
        
        //Do intro sequence
        for (int i = 0; i < 400; i++)
        {
            endCam.transform.RotateAround(Vector3.zero, Vector3.up, 45 * 0.01f);
            endCam.transform.position = new Vector3(endCam.transform.position.x, endCam.transform.position.y-0.02f, endCam.transform.position.z);
            //Do something
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1);
        intro.gameObject.SetActive(false);
        scoreCanvas.gameObject.SetActive(true);

        stonesThrown = 0;
        for (ends = 1; ends < 7; ends++)
        {
            StartCoroutine(PlayEnd());
            ready = false;
            yield return new WaitUntil(() => ready);
        }
        //End of game unless scores tied
        if(sc.blueScore == sc.redScore) {
            //Play another end
            StartCoroutine(PlayEnd());
            ends++;
        }
        if (sc.blueScore > sc.redScore)
        {
            ui.ShowResult(ends, sc.blueScore, sc.redScore, true);
        } else
        {
            //Display Loss Message
            ui.ShowResult(ends, sc.blueScore, sc.redScore, false);
        }
    }
    void DisplayMessage()
    {
        endCanvas.gameObject.SetActive(true);
    }
    void CleanUp()
    {
        stonesThrown = 0;
        foreach (Stone s in stones)
        {
            Destroy(s.gameObject);
        }
        stones = new Stone[16];
        for (int i = 0; i < 8; i++)
        {
            redIcons[i].color = new Color(1, 1 / 2.55f, 1 / 2.55f);
            redIcons[i].gameObject.SetActive(true);
            blueIcons[i].color = new Color(1 / 2.55f, 1 / 2.55f, 1);
            blueIcons[i].gameObject.SetActive(true);
        }
        if (ends == 1)
            endText.text = "2nd End";
        else if (ends == 2)
            endText.text = "3rd End";
        else if (ends == 3)
            endText.text = "4th End";
        else if (ends == 10)
            endText.text = "Finished";
        else
            endText.text = (ends+1).ToString() + "th End";
    }

    IEnumerator PlayEnd()
    {
        Debug.Log("Playing End");
        //Play one end
        endCam.enabled = false;
        topCam.enabled = true;
        scoreCanvas.worldCamera = topCam;
        endCanvas.gameObject.SetActive(true);
        endCanvas.GetComponentInChildren<Text>().text = endText.text;
        yield return new WaitForSeconds(2.0f);
        endCanvas.gameObject.SetActive(false);
        for (int i = 0; i < 16; i++)
        {
            if (throwingTeam == "blue")
            {
                blueIcons[i / 2].color = new Color(1 / 2.55f, 1 / 2.55f, 1, 1 / 2.55f);

                //Open the throw dialog
                typeCanvas.gameObject.SetActive(true);
                topCam.enabled = true;
                endCam.enabled = false;

                //Wait until the user throws the stone
                while (stone == null)
                {
                    //Do nothing
                    yield return null;
                }
                stonesThrown++;
                //Wait until the stone stops to do anything else
                while (stone.rb.velocity != Vector3.zero)
                {
                    //Do nothing
                    yield return null;
                }

                //Wait
                yield return new WaitForSeconds(timeout);

                blueIcons[i / 2].gameObject.SetActive(false);
                stone = null;
                throwingTeam = "red";
            }
            else
            {
                redIcons[i / 2].color = new Color(1, 1 / 2.55f, 1 / 2.55f, 1 / 2.55f);

                /*//Open the throw dialog
                typeCanvas.gameObject.SetActive(true);*/
                topCam.enabled = true;
                endCam.enabled = false;

                //Get the AI to choose what stone to throw
                ai.DecideOnShot();

                //Wait until the user throws the stone
                while (stone == null)
                {
                    //Do nothing
                    yield return null;
                }
                stonesThrown++;

                yield return new WaitForSeconds(1);

                //Wait until the stone stops to do anything else
                while (stone.rb.velocity != Vector3.zero)
                {
                    //Do nothing
                    yield return null;
                }

                Debug.Log("Ready to go");
                //Wait
                yield return new WaitForSeconds(timeout);

                redIcons[i / 2].gameObject.SetActive(false);
                stone = null;
                throwingTeam = "blue";
            }
        }

        yield return new WaitForSeconds(2);

        //Calculate who scored
        sc.CalculateScore(GetComponent<GameController>());
        CleanUp();

        endsTitle.text = "Score after " + ends.ToString() + " end";
        if (ends != 1)
        {
            endsTitle.text += "s";
        }
        scoreboard.gameObject.SetActive(true);

        yield return new WaitUntil(() => ready);

        scoreboard.gameObject.SetActive(false);
    }

}
