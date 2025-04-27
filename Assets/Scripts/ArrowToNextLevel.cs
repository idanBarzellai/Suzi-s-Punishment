using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArrowToNextLevel : MonoBehaviour
{
    Animator _player_animator;
    Animator _arrow_animator;
    [SerializeField] GameObject wall;
    Controls player;
    bool can_go = false;
    [SerializeField] Vector2 arrow_position = new Vector2(2.5f, -2.5f);

    private void Start()
    {
         
        player = FindObjectOfType<Controls>();
        _player_animator = player.GetComponent<Animator>();
        _arrow_animator = GetComponent<Animator>();
    }

    
    private void FixedUpdate()
    {
        if (can_go)
        {
            float step = 2f * Time.deltaTime;
            player.transform.localScale = new Vector2(Mathf.Abs(player.transform.localScale.x) , player.transform.localScale.y) ;
            // move sprite towards the target location
            player.transform.position = Vector2.MoveTowards(player.transform.position, new Vector2(14, -2.96f), step);
        }
    }
    public void go_to_next_level()
    {
        player.set_finished_level_emmobilazied(true);
        player.set_active_coins_added();
        player.get_clips()[7].Play();
        player.GetComponent<Animator>().SetTrigger("NextLevel");
        wall.GetComponent<Animator>().SetTrigger("Open");

        Invoke("can_go_enable", 1f);
        Invoke("next_level", 3f);

    }
    void can_go_enable()
    {
        can_go = true;
    }

    void next_level()
    {
        PlayerPrefs.SetInt("MaxHp", player.get_max_hp() + 65);
        if(player.get_current_hp() + 100 == player.get_max_hp() + 65)
            PlayerPrefs.SetInt("CurrentHp", PlayerPrefs.GetInt("MaxHp"));
        else
            PlayerPrefs.SetInt("CurrentHp", player.get_current_hp() + 100);
        
        StartCoroutine(FindObjectOfType<LevelLoader>().load_level(SceneManager.GetActiveScene().buildIndex + 1));
        
    }
}
