using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour {

    public Text text;
    public TutorialSpawner tSpawner;
    public Transform toFollow;
    public Transform topPosition;
    public Text[] zones;
    public Image highlight;
    public GameObject[] otherHighlights;

    public Canvas weightCanvas, lineCanvas, handleCanvas, scoreCanvas;

    private const string welcome = "Welcome to Curling! Let's learn how to play!";
    private string[] aim = { "The primary aim of curling is to get your stones closer to the middle of the circles than your opponent's.",
                    "Teams take turns throwing stones until each has thrown 8.",
                    "These 16 stones make up an end."};
    private string[] throwing = { "To do this, we must first learn how to throw stones.",
                    "Throwing a curling stone has 3 parts.",
                    "The weight of the stone (how hard you throw)",
                    "The line of the stone (where you throw it)",
                    "And the handle (which way you turn it)." };
    private string[] weight = { "First we will learn about the weight.",
                    "The weight of the stone is referred to as to where it stops.",
                    "Where the stone can stop is divided into 10 zones, as shown here on the ice.",
                    "In addition to these zones, there are the 5 hit weights",
                    "The 5 hit weights increase from hack (one that would stop where the hacks are behind the house) through to peel (a stone that is meant to move more than 1 stone).",
                    "The last thing to know about weight is that the stone curls more at light weights, and less at high ones.",
                    "So if you want the stone to curl lots, you will have to throw it lighter." };
    private string[] line = { "Now we come to the line. The line of the stone determines where the stone will end up horizontally.",
                    "As the stone will curl away from the line it is thrown at, you don't put the line where you want the stone to finish, but as far away as you think it will curl.",
                    "For example, here is where you might put the line for drawing to the centre of the house." };
    private string[] handle = { "The last thing that affects the stone's path is the handle.",
                    "The handle is simple, a clockwise turn will cause the stone to curl to it's right, and the other turn will cause it to curl to the left.",
                    "As a general rule, if you put the line to the left, use a clockwise turn and vice-versa." };
    private string demo = "Take a look at how the line and handle affect the stone here.";
    private string[] score = { "Now we know how to throw, but how do we score points?",
                    "The scoring of curling is a very simple affair, and is calculated by the number of stones you have closer than all of your opponent's stones",
                    "The easiest way to figure this out is to look at your opponents closest stone. You get one point for each stone closer than that.",
                    "Here you can see blue scoring two points.",
                    "Here is another example.",
                    "Although blue has 4 stones in the house, Yellow has the closest stone, and therefore scores a point while blue gets 0." };
    private const string fin = "And there you have the basics of curling. So good luck, and good curling!";

    // Use this for initialization
	void Start () {
        StartCoroutine(Tut());
	}
	
    IEnumerator Tut ()
    {
        toFollow = tSpawner.Engage();

        text.text = welcome;
        yield return new WaitForSeconds(4);

        text.text = aim[0];
        yield return new WaitForSeconds(6);
        text.text = aim[1];
        yield return new WaitForSeconds(4);
        text.text = aim[2];
        yield return new WaitForSeconds(4);

        text.text = throwing[0];
        yield return new WaitForSeconds(4);

        //Disable the follow
        toFollow = null;

        //Move to an empty house
        Camera.main.transform.position = topPosition.position;
        Camera.main.transform.rotation = topPosition.rotation;

        text.text = throwing[1];
        yield return new WaitForSeconds(3);

        text.text = throwing[2];
        //Show the weight, line and handle canvases in turn.
        weightCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        weightCanvas.gameObject.SetActive(false);

        text.text = throwing[3];
        lineCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        lineCanvas.gameObject.SetActive(false);

        text.text = throwing[4];
        handleCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        handleCanvas.gameObject.SetActive(false);


        weightCanvas.gameObject.SetActive(true);
        text.text = weight[0];
        yield return new WaitForSeconds(3);
        weightCanvas.gameObject.SetActive(false);

        text.text = weight[1];
        yield return new WaitForSeconds(3);
        
        text.text = weight[2];
        //Sequence of zones appearing
        foreach (Text t in zones)
        {
            t.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f); 
        }

        
        yield return new WaitForSeconds(3);

        weightCanvas.gameObject.SetActive(true);
        text.text = weight[3];
        yield return new WaitForSeconds(3);

        
        highlight.gameObject.SetActive(true);
        text.text = weight[4];
        yield return new WaitForSeconds(6);

        foreach(GameObject g in otherHighlights)
        {
            g.SetActive(true);
        }
        text.text = weight[5];
        yield return new WaitForSeconds(3);
        text.text = weight[6];
        yield return new WaitForSeconds(3);

        text.text = line[0];
        yield return new WaitForSeconds(3);
        text.text = line[1];
        yield return new WaitForSeconds(3);
        text.text = line[2];
        yield return new WaitForSeconds(3);

        text.text = handle[0];
        yield return new WaitForSeconds(3);
        text.text = handle[1];
        yield return new WaitForSeconds(3);
        text.text = handle[2];
        yield return new WaitForSeconds(3);

        text.text = demo;
        yield return new WaitForSeconds(1);

        text.text = score[0];
        yield return new WaitForSeconds(3);
        text.text = score[1];
        yield return new WaitForSeconds(3);
        text.text = score[2];
        yield return new WaitForSeconds(3);
        text.text = score[3];
        yield return new WaitForSeconds(3);

        text.text = fin;
        yield return new WaitForSeconds(3);

        SceneManager.LoadScene(0);
    
}
    void Update()
    {
        if (toFollow)
        {
            Camera.main.transform.position = new Vector3(-12, 2, toFollow.position.z - 3.5f);
            Camera.main.transform.rotation = Quaternion.Euler(30, 0, 0);
        }
    }
}
