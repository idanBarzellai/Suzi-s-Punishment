using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] int wave = 0;
    [SerializeField] GameObject arrow;
    int coins_increment_after_win = 3;
    [HideInInspector]
    public MonsterWave current_wave;
    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("Level1"))
            Invoke("tickNextWave", 6f);
        else
            Invoke("tickNextWave", 3f);
    }

    public void tickNextWave()
    {
        if (gameObject.transform.childCount > wave)
        {
            Debug.Log("Spawning New Wave");
            current_wave = gameObject.transform.GetChild(wave++).GetComponent<MonsterWave>().spawnWave(this);
        }
        else
        {
            if(!SceneManager.GetActiveScene().name.Equals("Login") && !SceneManager.GetActiveScene().name.Equals("GameOver") && !SceneManager.GetActiveScene().name.Equals("Credits"))
                FindObjectOfType<Controls>().get_clips()[3].Play();
            Debug.Log("Waves Ended! Game Should Jump To Next Level Now?");
            if (SceneManager.GetActiveScene().name.Equals("Level5"))
            {
                GameObject.FindObjectOfType<LevelLoader>().get_boss_text().SetActive(true);
                if (PlayerPrefs.GetInt("Avatar") == 1)
                    GameObject.FindObjectOfType<LevelLoader>().change_text(ColorUtility.ToHtmlStringRGB(FindObjectOfType<TotemCharacter>().get_hair_color()), FindObjectOfType<TotemCharacter>().get_hair_style());
                else
                    GameObject.FindObjectOfType<LevelLoader>().change_text("B80C08", "ponytails");
                StartCoroutine(set_arrow_active(5));
            }
            else
            {
                StartCoroutine(set_arrow_active(0));
            }
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + coins_increment_after_win);
            // add to player coins animation
        }
    }

    IEnumerator set_arrow_active(int time)
    {
        yield return new WaitForSecondsRealtime(time);
        arrow.SetActive(true);
    }

    public void checkDone()
    {
        if (current_wave.checkDone())
            tickNextWave();
    }
}
