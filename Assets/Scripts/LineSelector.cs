using UnityEngine;
using UnityEngine.EventSystems;

public class LineSelector : MonoBehaviour
{

    public bool selected = false;
    public GameObject line;
    public Canvas canvas;
    public Material confirm;
    public Material selecting;
    public float currentLine;
    public GameController gc;
	
	// Update is called once per frame
	void Update () {
        if (Input.mousePosition.y < 150)
        {
            //Don't log button clicks as changes to the line
            return;
        }
        //If not selected set position to that of mouse.
        if (!selected)
        {
            float newPosition = (Input.mousePosition.x-512)/77;
            line.transform.position = new Vector3(newPosition,
                line.transform.position.y,
                line.transform.position.z);
        }
        if (Input.GetMouseButtonDown(0))
        {
            
            selected = !selected;
            if (selected)
            {
                
                //Change colour to green
                line.GetComponent<Renderer>().material = confirm;
                Vector3 toThrowAlong = new Vector3(line.transform.position.x, 0, (17.375f + 20.75f));
                Vector3 centreline = new Vector3(0, 0, 1);
                float angleBetween = Vector3.Angle(toThrowAlong, centreline) * Mathf.Sign(line.transform.position.x);
                gc.direction = angleBetween;
                
            }
            else
            {
                //Change colour to red
                line.GetComponent<Renderer>().material = selecting;
            }
        }
    }
}
