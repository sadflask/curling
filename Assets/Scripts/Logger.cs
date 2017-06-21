using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour {

    public Text output;

    public void Print(string s)
    {
        output.text = s;
    }
}
