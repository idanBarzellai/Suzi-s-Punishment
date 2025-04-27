using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour
{
    [SerializeField] AudioClip[] clips;
	[SerializeField] float fade_n_out_time = 20;
	AudioSource audiosource;

    private void Start()
    {
		audiosource = GetComponent<AudioSource>();
		audiosource.ignoreListenerPause = true;
		StartCoroutine(start_fade_in(0, 0));
		audiosource.Play();
		
	}
	public IEnumerator start_fade_in(int time , int clip)
    {
		yield return new WaitForSecondsRealtime(time);
		audiosource.clip = clips[clip];
		StartCoroutine(FadeIn(audiosource, fade_n_out_time));
	}
    public void start_fade_out_login()
    {
		StartCoroutine(FadeOut(GetComponent<AudioSource>(), fade_n_out_time));
		StartCoroutine(start_fade_in(2, 1));
	}

	/*public IEnumerator start_fade_in_level()
    {
		yield return new WaitForSecondsRealtime(2f);
		GetComponent<AudioSource>().clip = clips[1];
		StartCoroutine(FadeIn(GetComponent<AudioSource>(), fade_n_out_time));
	}*/
	public void start_fade_out_level(bool to_credits)
	{
		StartCoroutine(FadeOut(GetComponent<AudioSource>(), fade_n_out_time));
        if (to_credits)
        {
			StartCoroutine(start_fade_in(2, 0));
        }
        else
        {
			StartCoroutine(start_fade_in(2, 2));
        }
		
	}
	/*public IEnumerator start_fade_in_gameover(int time)
	{
		yield return new WaitForSecondsRealtime(time);
		GetComponent<AudioSource>().clip = clips[2];
		StartCoroutine(FadeIn(GetComponent<AudioSource>(), fade_n_out_time));
	}*/
	/*public void start_fade_out_gameover()
	{
		StartCoroutine(FadeOut(GetComponent<AudioSource>(), fade_n_out_time	));
		StartCoroutine(start_fade_in())
	}*/

    public IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
	{
		float startVolume = audioSource.volume;
		while (audioSource.volume > 0)
		{
			audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
			yield return null;
		}
		audioSource.Stop();
	}

	public IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
	{
		audioSource.Play();
		audioSource.volume = 0f;
		while (audioSource.volume < 1)
		{
			audioSource.volume += Time.deltaTime / FadeTime;
			yield return null;
		}
	}
}
