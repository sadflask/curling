using UnityEngine;
using UnityEngine.UI;

public class LineSelector : MonoBehaviour
{
    public Material confirm;
    public Material selecting;

    public bool selected = false;
    public GameObject line;
    public Canvas canvas;
    
    public float currentLine;
    public GameController gc;
	
    void OnEnable()
    {
        selected = false;
        line.GetComponent<MeshRenderer>().material = selecting;
    }
	// Update is called once per frame
	void Update () {
        if (Input.mousePosition.y < Screen.height/4)
        {
            //Don't log button clicks as changes to the line
            return;
        }
        //If not selected set position to that of mouse.
        if (!selected)
        {
            float newx = (Input.mousePosition.x - Screen.width / 2) / Screen.width * 8.8f * 2 ;
            line.transform.position = new Vector3(newx, line.transform.position.y, line.transform.position.z);
        }
        if (Input.GetMouseButtonDown(0))
        {
            ToggleSelected();
        }
    }
    public void ToggleSelected()
    {
        selected = !selected;
        if (selected)
        {
            //Change colour to green
            line.GetComponent<MeshRenderer>().material = confirm;
            Vector3 toThrowAlong = new Vector3(line.transform.position.x, 0, (17.375f + 20.75f));
            Vector3 centreline = new Vector3(0, 0, 1);
            float angleBetween = Vector3.Angle(toThrowAlong, centreline) * Mathf.Sign(line.transform.position.x);
            gc.SetPlayerDirection(angleBetween);
        }
        else
        {
            //Change colour to red
            line.GetComponent<MeshRenderer>().material = selecting;
        }
    }
}
