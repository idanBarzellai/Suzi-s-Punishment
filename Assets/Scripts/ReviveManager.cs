using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReviveManager : MonoBehaviour
{
    bool is_revived = false;
    int revive_coin_amount = 5;
    int counter = 5;
    Controls player;
    [SerializeField] GameObject revive_menu;
    [SerializeField] Text countdown_to_revive;
    [SerializeField] GameObject menu;
    [SerializeField] Text coins;

    Music music;
    private void Start()
    {
        player = FindObjectOfType<Controls>();
        music = FindObjectOfType<Music>();
        
    }
    private void Update()
    {
        player = FindObjectOfType<Controls>();
        coins.text = PlayerPrefs.GetInt("Coins").ToString();
        /*if (!is_revived && counter <= 0)
        {
            not_reviving();
            
        }*/
    }
    public void start_revive_counter()
    {
        StartCoroutine(countdown());
    }
    public IEnumerator countdown()
    {
        
        revive_menu.SetActive(true);
        menu.SetActive( false);
        while (counter != -1)
        {
            countdown_to_revive.text = "" + counter--;
            if (is_revived)
            {
                counter = 5;
                yield break;
            }
            yield return new WaitForSecondsRealtime(1.2f);
        }
        not_reviving();
    }
    public void revive_button()
    {
        if (PlayerPrefs.GetInt("Coins") >= 5 && counter >= 0)
        {
            is_revived = true;
            countdown_to_revive.enabled = false;
            StopCoroutine(countdown());
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - revive_coin_amount);
           
            StartCoroutine(on_revive_click());
        }
        
    }

    IEnumerator on_revive_click()
    {
        
        yield return new WaitForSecondsRealtime(1.5f);
        
        revive_menu.active = (false);
        
        Time.timeScale = 1;
        player.set_current_hp(player.get_max_hp());
        countdown_to_revive.enabled = true;
        is_revived = false;
        //counter = 5;
        player.get_clips()[5].Play();
        player.active_revive_avatar();
        Invoke("set_effect_on_player", 2f);
    }

    public void not_reviving()
    {
        PlayerPrefs.SetInt("MaxHp", player.get_basic_hp_for_reset());
        PlayerPrefs.SetInt("CurrentHp", player.get_basic_hp_for_reset());

        music.start_fade_out_level(false);
        StartCoroutine(FindObjectOfType<LevelLoader>().load_level(12)); // change to gameover on build index
    }

    void set_effect_on_player()
    {
        
        player.active_revive_avatar();
        menu.SetActive(true);
        countdown_to_revive.enabled = true;
    }
    

    public void quit_button()
    {
        Application.Quit();
    }
}
