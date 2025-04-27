using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    
    [SerializeField] Animator _animator;
    [SerializeField] GameObject to_arrow_collider_object;
    [SerializeField] GameObject boss_text;
    [SerializeField] GameObject[] avatars;
    Music music;

    private void Start()
    {
/*        Screen.orientation = ScreenOrientation.LandscapeRight;*/
        if (SceneManager.GetActiveScene().name.Equals("Credits"))
            StartCoroutine(load_login_level());
        if (!SceneManager.GetActiveScene().name.Equals("Login"))
            avatars[PlayerPrefs.GetInt("Avatar")].SetActive(true);
        music = FindObjectOfType<Music>();

        
    }
    public IEnumerator load_level(int level_index)
    {
        _animator.SetTrigger("Start");
        
        yield return new WaitForSecondsRealtime(1f);
        if (level_index % 5 == 1 && level_index != 11)
            PlayerPrefs.SetInt("CheckPoint", level_index);
        SceneManager.LoadScene(level_index);
        Time.timeScale = 1;
        

    }
    public IEnumerator load_checkpoint_level()
    {
        _animator.SetTrigger("Start");

        yield return new WaitForSecondsRealtime(1f);
        
        SceneManager.LoadScene(PlayerPrefs.GetInt("CheckPoint", 1)); // PlayerPrefs.GetInt("CheckPoint", 1));
        Time.timeScale = 1;
    }

    public IEnumerator load_login_level()
    {
        

        yield return new WaitForSecondsRealtime(25f);
        _animator.SetTrigger("Start");
        SceneManager.LoadScene(0); // PlayerPrefs.GetInt("CheckPoint", 1));
        Time.timeScale = 1;
    }
    public void change_text (string colorhexa, string hairstyle)
    {
        boss_text.GetComponentInChildren<Text>().text = "You can't run away from me !!!  Stupid <color=#" + colorhexa + "><b>" + hairstyle + "</b></color>-haired girl";
    }

    public GameObject get_boss_text()
    {
        return boss_text;
    }
    public void boss_is_dead()
    {
        StartCoroutine(start_text_end());

    } 

    IEnumerator start_text_end()
    {
        yield return new WaitForSecondsRealtime(4f);
        boss_text.SetActive(true);
        yield return new WaitForSecondsRealtime(4f);
        Debug.Log("im here");
        StartCoroutine(load_level(SceneManager.GetActiveScene().buildIndex + 1));
        music.start_fade_out_level(true);
    }

    public void pause_button()
    {
        if (Time.timeScale == 1)
            Time.timeScale = 0;
        else
            Time.timeScale = 1; 
    }

    public void back_button()
    {
        music.start_fade_out_level(true);
        StartCoroutine(load_level(0));
    }

    public void win_level_go_to_arrow()
    {
        to_arrow_collider_object.active = true;
    }

    public void play_again_button()
    {
        music.start_fade_out_login();
        StartCoroutine(load_checkpoint_level());
    }

    public void quit_button()
    {
        Application.Quit();
    }

    public GameObject[] get_avatars()
    {
        return avatars;
    }
}
