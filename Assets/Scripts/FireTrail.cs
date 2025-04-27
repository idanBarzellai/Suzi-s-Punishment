using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrail : MonoBehaviour
{
    float last_time_active;
    float lifetime = 4f;

    float time_delay = .5f;
    float last_fire_hurt = 0f;
    
    bool is_in_fire = false;
    bool is_active = false;
    Controls player;

    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Controls>();
    }
    private void FixedUpdate()
    {
        
        if(is_active && Time.time - last_fire_hurt > time_delay)
        {
            last_fire_hurt = Time.time;
            if (is_in_fire)
            {
                player.getDmg(6);
            }
        }

        if (Time.time - last_time_active > lifetime)
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.enabled = false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (is_active && collision.CompareTag("Player"))
        {
            is_in_fire = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            is_in_fire = false;
        }
    }

    public void set_active(bool set)
    {
        
        is_active = set;
        if (set)
        {
            last_time_active = Time.time;
        }
        
    }
}
