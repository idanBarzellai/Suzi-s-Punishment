using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFireball : MonoBehaviour
{

    GameObject player;
    Vector3 player_position;
    [SerializeField] float speed = 2f;


    [HideInInspector]
    public Collider2D ground = null;

    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        player_position = player.transform.position;
    }

    private void FixedUpdate()
    {
        float step = speed * Time.deltaTime;

        // move sprite towards the target location
        transform.position = Vector2.MoveTowards(transform.position, player_position, step);
        if (transform.position == player_position)
            Destroy(this.gameObject);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.GetComponent<Controls>().gotAttacked((int)transform.localScale.x, 4, 2.5f, 2.5f);
            /*if (player.GetComponent<Controls>().is_player_grounded())
                Destroy(this.gameObject);*/
        }
        if ((collision.CompareTag("Ground") && transform.position == player_position))
            Destroy(this.gameObject);

    }

    private void OnDestroy()
    {
        foreach (GameObject ground in GameObject.FindGameObjectsWithTag("FireFloor"))
        {
            if (Vector2.Distance(this.transform.position, ground.transform.position) < 2f && this.transform.position.y - ground.transform.position.y > 0)
            {
                FindObjectOfType<MonsterMaleFire>().start_fireball_firetrail(ground);
            }
        }
    }

    
}
