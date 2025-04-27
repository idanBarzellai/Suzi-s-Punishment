using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    float start_time;

    void Start()
    {
        //Time.timeScale = 0;
        start_time = Time.time;
    }

    private void Update()
    {
        if (Time.time - start_time > 4f)
        {
            //Time.timeScale = 1;
            FindObjectOfType<Tutorial>().GetComponent<Animator>().SetTrigger("fadeout");
        }
        if (Time.time - start_time > 5.5f)
        {
            this.gameObject.SetActive(false);
        }
    }

}
