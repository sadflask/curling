using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Canvas[] weightCanvases;
    public Canvas typeCanvas;
    public GameController gc;

    //Hides all the canvases except the shot type select one.
    public void Back()
    {
        foreach (Canvas c in weightCanvases)
        {
            c.gameObject.SetActive(false);
        }
        typeCanvas.gameObject.SetActive(true);
    }
    public void toWeights()
    {
        typeCanvas.gameObject.SetActive(false);
        if (gc.type == "Guard")
        {
            weightCanvases[0].gameObject.SetActive(true);
        } else if (gc.type == "Draw") 
        {
            weightCanvases[1].gameObject.SetActive(true);
        }
        else if (gc.type == "Tap")
        {
            weightCanvases[2].gameObject.SetActive(true);
        } else
        {
            weightCanvases[3].gameObject.SetActive(true);
        }
        foreach (Toggle t in typeCanvas.GetComponentsInChildren<Toggle>())
        {
            t.isOn = false;
        }
    }
    public void confirmWeight()
    {
        foreach (Canvas c in weightCanvases)
        {
            c.gameObject.SetActive(false);
            foreach (Toggle t in c.GetComponentsInChildren<Toggle>())
            {
                t.isOn = false;
            }
        }
        gc.nextHandle= Random.Range(-1, 1);
        if (gc.nextHandle == 0)
        {
            gc.nextHandle = 1;
        }
        if (gc.nextHandle == 1)
        {
            gc.nextDirection = -1.4f;
        }
        else
        {
            gc.nextDirection = 1.4f;
        }

        if (gc.nextWeight == 0)
        {
            if (gc.type == "Guard")
            {
                gc.nextWeight = 5.2f;
            } else if (gc.type == "Draw")
            {
                gc.nextWeight = 5.4f;
            } else if (gc.type == "Tap")
            {
                gc.nextWeight = 5.58f;
            } else
            {
                gc.nextWeight = 5.85f;
            }
        }
            gc.ThrowStone();
    }

    public void setType(string type)
    {
        gc.type = type;
    }
    public void setWeight(float weight)
    {
        gc.nextWeight = weight;
    }
}
