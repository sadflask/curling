using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PlayerData : MonoBehaviour {

    //Singleton reference
    public static PlayerData instance;
    private int games, wins;
    private string filePath;

	// Use this for initialization
	void Awake() {
		if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this)
        {
            Destroy(this);
        }
        //Force load on awake

        filePath = Application.persistentDataPath + "/SaveData.dat";
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            //Deserialize file
            Data data = (Data)bf.Deserialize(file);
            file.Close();
            //Load fields from data class
            wins = data.wins;
            games = data.games;
        }
	}

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);
        Data data = new Data();
        data.wins = wins;
        data.games = games;
        bf.Serialize(file, data);
        file.Close();
    }
	
    public int GetWins()
    {
        return wins;
    }
    public int GetGames()
    {
        return games;
    }
    public void AddGame(bool won)
    {
        games++;
        if(won)
            wins++;
        Save();
    }

    //The serializable class to hold the wins/losses data
    [Serializable]
    class Data
    {
        public int wins;
        public int games;
    }
}
