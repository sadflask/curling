using UnityEngine;

public class ConfigData : MonoBehaviour {

    public string[] teamNames;
    public int[] colourIndices;
    public int numEnds;
    public int aiLevel;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}
}
