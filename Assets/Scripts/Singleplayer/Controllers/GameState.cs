using System.Collections;
using UnityEngine;

public class GameState : MonoBehaviour {

    public GameController gc;
    public GameObject[] playerStones;
    public Color[] stoneColours;
    
    public int stonesThrown;

    public ArrayList stones = new ArrayList();
    
    public Stone currentStone = null;
    public Stone lastThrownStone = null;

    public GameObject skipPosition, topPosition, stonePosition;

    public Player[] players;
    
    public int throwingPlayerIndex = 0;
    
    public int currentEnd;

    //The customisable fields to be loaded from the ConfigData object.
    public int ends;

    // Use this for initialization
    void Start () {
        //Load in the config data from existing object.
        ConfigData cData = Object.FindObjectOfType<ConfigData>();
        ends = cData.numEnds;
        foreach (Player p in players)
        {
            p.LoadData(cData);
        }
    }
	
}
