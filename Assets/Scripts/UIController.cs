using UnityEngine;
using UnityEngine.UI;
using System;

public class UIController : MonoBehaviour {

    public Canvas weightCanvas;
    public GameController gc;
    public Canvas lineCanvas;
    public GameObject line;
    public Canvas handleCanvas;
    public Color lastColor;
    public Color buttonOff;
    public Sprite stone;
    public Sprite rightdown;
    public Sprite leftdown;

    public enum Weight
    {
        One = 5200,
        Two = 5300,
        Three = 5400,
        Four = 5460,
        Five = 5510,
        Six = 5540,
        Seven = 5560,
        Eight = 5580,
        Nine = 5615,
        Ten = 5655,
        Hack = 5750,
        Board = 5850,
        Control = 6050,
        Normal = 6250,
        Peel = 6450
    }

    //Hides all the canvases except the shot type select one.
    public void Back()
    {
        /*foreach (Canvas c in weightCanvases)
        {
            c.gameObject.SetActive(false);
        }
        typeCanvas.gameObject.SetActive(true);*/
    }
    public void confirmWeight()
    {
        if (gc.weight == 0)
        {
            //Popup about how user can't do that
        }
        else
        {
            weightCanvas.gameObject.SetActive(false);
            line.SetActive(true);
            lineCanvas.gameObject.SetActive(true);
        }
    }
    public void confirmLine()
    {
        lineCanvas.gameObject.SetActive(false);
        line.SetActive(false);
        handleCanvas.gameObject.SetActive(true);
    }
    public void setWeight(string weight)
    {
        Weight weightEnum = (Weight)Enum.Parse(typeof(Weight), weight);
        gc.weight = (float)weightEnum;

        Color tempColor = lastColor;
        //Enable all buttons that weren't pushed and disable the one that was
        foreach (Button b in weightCanvas.GetComponentsInChildren<Button>(true))
        {
            if (b.name == weight)
            {
                b.enabled = false;
                lastColor = b.image.color;
                b.image.color = buttonOff;
            }
            else
            {
                /*if (b.name != "ViewHouse" && b.name != "Confirm")*/
                if(b.image.color == buttonOff)
                {
                    b.image.color = tempColor;
                    b.enabled = true;
                }
            }
        }

    }
    public void setHandle(int handle)
    {
        gc.handle = handle;
        foreach (Button b in handleCanvas.GetComponentsInChildren<Button>())
        {
            if (b.name != "Back" && b.name != "Confirm")
            {
                if (b.name == "Left")
                {
                    if (gc.handle == 1)
                    {
                        b.image.sprite = leftdown;
                    }
                    else
                    {
                        b.image.sprite = stone;
                    }
                }
                else
                {
                    if (gc.handle == -1)
                    {
                        b.image.sprite = rightdown;
                    }
                    else
                    {
                        b.image.sprite = stone;
                    }
                }
            }
        }
    }
    public void viewHouse()
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
    public void backToLine()
    {
        handleCanvas.gameObject.SetActive(false);
        lineCanvas.gameObject.SetActive(true);
        line.SetActive(true);
    }
    public void confirmHandle()
    {
        gc.ThrowStone();
        handleCanvas.gameObject.SetActive(false);
    }
    public void toNextEnd()
    {
        gc.ready = true;
    }
}
