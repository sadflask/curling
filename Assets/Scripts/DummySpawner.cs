using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySpawner : MonoBehaviour
{
    public Transform stoneSpawn;
    public DummyPooler[] poolers;
    
    // Use this for initialization
    void Start()
    {
        StartCoroutine(ThrowStones());
    }

    IEnumerator ThrowStones()
    {
        while(true)
        {
            for(int i=0;i<8;i++)
            {
                
                //Throw a stone from each team, 10-15 seconds apart
                ThrowStone(poolers[0]);
                yield return new WaitForSeconds(Random.Range(10.0f, 15.0f));
                ThrowStone(poolers[1]);
                yield return new WaitForSeconds(Random.Range(10.0f, 15.0f));
            }
            //Wait between ends
            yield return new WaitForSeconds(60f);
            poolers[0].resetPool();
            poolers[1].resetPool();
        }

    }
    private void ThrowStone(DummyPooler pool)
    {
        DummyStone ds = pool.getStone();
        if (ds)
        {
            ds.transform.position = stoneSpawn.position;
            ds.gameObject.SetActive(true);
        }
    }
}
