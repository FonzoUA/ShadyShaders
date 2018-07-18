using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AUDIO_MUSIC
{
    NONE = -1,
    SIREN
}

public enum AUDIO_SFX
{
    NONE = -1,
}


public class AudioManager : MonoBehaviour
{

    private static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }

    [SerializeField] AudioSource musicSource;
    AudioSource[] sfxSources;
    [SerializeField] AudioClip[] musicClips;
    [SerializeField] AudioClip[] sfxClips;
    int currentSfxSource = 0;

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        sfxSources = GetComponents<AudioSource>();
    }

    //slap this fucker where music is played
    public void PlayMusic(AUDIO_MUSIC music, bool loop = true)
    {
        if (music != AUDIO_MUSIC.NONE)
        {
            musicSource.loop = loop;
            musicSource.clip = musicClips[(int)music];
            musicSource.Play();
        }
    }

	public void StopMusic(AUDIO_MUSIC music)
	{
		if (music != AUDIO_MUSIC.NONE)
		{
			musicSource.clip = musicClips[(int)music];
			musicSource.Stop();
		}
	}

    //slap this bitch where sfx are needed
    public void PlaySFX(AUDIO_SFX sfx, float pitch = 1f)
    {
        if (sfx != AUDIO_SFX.NONE)
        {
            sfxSources[currentSfxSource].pitch = pitch;
            sfxSources[currentSfxSource].clip =
                sfxClips[(int)sfx];

            sfxSources[currentSfxSource].Play();
            currentSfxSource = (currentSfxSource + 1) % sfxSources.Length;
        }
    }


    //--------------Example code---------------
    //void OnCollisionEnter2d(Collider2D other)
    //{
    //    int SoundChoice = 0;
    //    if (type == INGREDIENT_TYPE.BREAD)
    //    {
    //        SoundChoice = Random.Range(17, 20);
    //    }
    //    else if (type == INGREDIENT_TYPE.MEAT)
    //    {
    //        SoundChoice = Random.Range(59, 63);
    //    }
    //    else if (type == INGREDIENT_TYPE.LETTUCE)
    //    {
    //        SoundChoice = Random.Range(48, 52);
    //    }
    //    AudioManager.Instance.PlaySFX((AUDIO_SFX)SoundChoice, Random.Range(0.9f, 1.1f));
    //}
    //------------------------------------------

    public void MusicVolume(float value)
    {
        musicSource.volume = value;
    }
    public void SfxVolume(float value)
    {
        foreach (AudioSource sfx in sfxSources)
        {
            sfx.volume = value;
        }
    }
}
