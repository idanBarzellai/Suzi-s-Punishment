using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWave : MonoBehaviour
{
    [SerializeField] List<GameObject> mobs_list;
    [SerializeField] List<GameObject> spawn_places;
    List<GameObject> spawn_places_remaning;
    GameObject mob;
    private int count;
    int index;

    public MonsterWave spawnWave(LevelManager lvl_mngr_ref)
    {
        spawn_places_remaning = spawn_places;
        for (int i = 0; i < mobs_list.Count; i++)
        {
                       
            index = Random.Range(0, spawn_places_remaning.Count - 1);
            GameObject spawn_place_at_index = spawn_places_remaning[index];
            spawn_places_remaning.RemoveAt(index);
            spawn_place_at_index.active = true;
            StartCoroutine(close_portal(index, spawn_place_at_index));
            StartCoroutine(spawn_monster(i, index,spawn_place_at_index, lvl_mngr_ref));
        }

        return this;
    }

    public bool checkDone()
    {
        
        return --count == 0;
    }

    private IEnumerator spawn_monster(int i, int index,GameObject portal, LevelManager lvl_mngr_ref)
    {
        yield return new WaitForSecondsRealtime(1f);
        count++;
        mob = Instantiate<GameObject>(mobs_list[i]);
        mob.transform.position = portal.transform.position;
        mob.GetComponent<Monster>().lvl_mngr_ref = lvl_mngr_ref;
    }

    private IEnumerator close_portal(int index, GameObject portal)
    {
        yield return new WaitForSecondsRealtime(1f);

        portal.active = false;
    }
}
