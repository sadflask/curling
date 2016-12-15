using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public GameObject blue, red;
    int stonesThrown;
    public Stone[] stones = new Stone[16];
    public string throwingTeam = "red";
    public Camera endCam;
    public Camera topCam;
    public int timeout;
    public Canvas scoreCanvas;
    public Canvas typeCanvas;
    public Image[] blueIcons, redIcons;
    public Text scoreText, endText;
    public float weight, direction;
    public string type;
    public Stone stone = null;
    public int handle;

    void Start()
    {
        timeout = 2;
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
        stone.force = weight;
        stone.transform.rotation = Quaternion.Euler(0.0f, direction, 0.0f);
        stone.handle = handle;
        stone.color = throwingTeam;
        stones[stonesThrown] = stone;
        stone.endCam = endCam;
        stone.topCam = topCam;
        stone.canvas = scoreCanvas;

        endCam.enabled = true;
        topCam.enabled = false;
        scoreCanvas.worldCamera = endCam;
        return stone;
        
    }
    IEnumerator Throws()
    {
        stonesThrown = 0;
        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < 16; i++)
            {
                if (throwingTeam == "blue")
                {
                    blueIcons[i/2].color = new Color(1 / 2.55f, 1 / 2.55f, 1, 1 / 2.55f);

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

                    //Wait until the stone stops to do anything else
                    while(stone.rb.velocity != Vector3.zero)
                    {
                        //Do nothing
                        yield return null;
                    }

                    //Wait
                    yield return new WaitForSeconds(timeout);
                    stonesThrown++;
                    blueIcons[i/2].gameObject.SetActive(false);
                    stone = null;
                    throwingTeam = "red";
                }
                else
                {
                    redIcons[i / 2].color = new Color(1, 1 / 2.55f, 1 / 2.55f, 1 / 2.55f);

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

                    //Wait until the stone stops to do anything else
                    while (stone.rb.velocity != Vector3.zero)
                    {
                        //Do nothing
                        yield return null;
                    }
                    //Wait
                    yield return new WaitForSeconds(timeout);
                    stonesThrown++;
                    redIcons[i / 2].gameObject.SetActive(false);
                    stone = null;
                    throwingTeam = "blue";
                }
            }

            yield return new WaitForSeconds(2);
            
            //Calculate who scored
            Score.score(GetComponent<GameController>());
            cleanUp();
        }
    }
    
    void cleanUp()
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
        if (endText.text == "1st End")
            endText.text = "2nd End";
        else if (endText.text == "2nd End")
            endText.text = "3rd End";
        else if (endText.text == "3rd End")
            endText.text = "4th End";
        else if (endText.text == "10th End")
            endText.text = "Finished";
        else
            endText.text = (int.Parse(endText.text.ToCharArray()[0].ToString()) + 1).ToString() + "th End";
    }

}
