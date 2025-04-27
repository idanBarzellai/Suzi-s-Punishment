using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TotemEntities;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [SerializeField] GameObject Logo;
    [SerializeField] GameObject Login;
    [SerializeField] InputField name;
    [SerializeField] InputField pwd;
    [SerializeField] GameObject play_online;
    [SerializeField] GameObject login_button;
    [SerializeField] TotemCharacter totemCharcter;
    TotemUsersDB usersDB;

    bool login_ok = false;
    float time_delay_for_login = 4f;

    private void Start()
    {
        totemCharcter = FindObjectOfType<TotemCharacter>();
        Invoke("start_logo", 1f);
        usersDB =totemCharcter.get_totem_userdb();

        PlayerPrefs.SetInt("MaxHp", 350);
        PlayerPrefs.SetInt("CurrentHp", 350);
        PlayerPrefs.SetInt("CheckPoint", 1);
    }

    void start_logo()
    {
        Logo.SetActive(true);
        StartCoroutine(start_login());
    }

    private IEnumerator start_login()
    {
        yield return new WaitForSecondsRealtime(time_delay_for_login);
        
        Logo.SetActive(false);
        Login.SetActive(true);
        
    }

    public void play_offline_next_scene()
    {
        PlayerPrefs.SetInt("Avatar", 0);
        StartCoroutine(load_level());
    }

    public void play_online_next_scene()
    {
        PlayerPrefs.SetInt("Avatar", 1);
        StartCoroutine(load_level());
    }

    public void check_name_n_pwd()
    {
        usersDB = totemCharcter.get_totem_userdb();
        if (usersDB.AuthenticateUser(name.text, pwd.text))
        {
            totemCharcter.user_logged_in(name.text);
            login_button.SetActive(false);
            login_ok = true;
            play_online.SetActive(true);
        }
    }

    public void DeletePLayerprefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public IEnumerator load_level()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        FindObjectOfType<Music>().start_fade_out_login();
        StartCoroutine(FindObjectOfType<LevelLoader>().load_level(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void quit_button()
    {
        Application.Quit();
    }

}
