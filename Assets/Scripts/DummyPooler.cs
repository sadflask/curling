using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPooler : MonoBehaviour {

    public DummyStone stoneToMake;

    public List<DummyStone> stonePool;
	// Use this for initialization

	void Start () {
        stonePool = new List<DummyStone>();
        for (int j = 0; j < 8; j++)
        {
            //Make 8 stones and add them to the pool.
            DummyStone ds = Instantiate(stoneToMake, transform);
            stonePool.Add(ds);
            ds.gameObject.SetActive(false);
        }        
	}
	
	public DummyStone getStone()
    {
        foreach (DummyStone ds in stonePool)
        {
            if (!ds.gameObject.activeSelf)
            {
                return ds;
            }
        }
        //Should never happen in this case.
        return null;
    }
    public void resetPool()
    {
        foreach(DummyStone ds in stonePool)
        {
            ds.gameObject.SetActive(false);
        }
    }
}
