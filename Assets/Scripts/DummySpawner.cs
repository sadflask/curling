using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySpawner : MonoBehaviour
{
    public Transform stoneSpawn;
    public DummyPooler[] poolers;
    
    // Use this for initialization
    protected virtual void Start()
    {
        StartCoroutine(ThrowStones());
    }

    IEnumerator ThrowStones()
    {
        while(true)
        {
            for(int i=0;i<8;i++)
            {
                
                //Throw a stone from each team, 20-30 seconds apart

                ThrowStone(poolers[0]);
                yield return new WaitForSeconds(Random.Range(20f, 30f));
                ThrowStone(poolers[1]);
                yield return new WaitForSeconds(Random.Range(20f, 30f));
            }
            //Wait between ends
            poolers[0].resetPool();
            poolers[1].resetPool();
            yield return new WaitForSeconds(60f);
        }

    }
    protected Transform ThrowStone(DummyPooler pool)
    {
        DummyStone ds = pool.getStone();
        if (ds)
        {
            ds.transform.position = stoneSpawn.position;
            ds.gameObject.SetActive(true);
            return ds.transform;
        }
        return null;
    }
}
