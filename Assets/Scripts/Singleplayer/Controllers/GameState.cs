using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour {

    public GameController gc;
    public GameObject[] playerStones;
    public Color[] stoneColours;
    
    public int stonesThrown;

    public Stone[] stones = new Stone[16];
    
    public Stone currentStone = null;

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
