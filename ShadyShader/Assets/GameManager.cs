using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public Material[] materials;
    public float TransitionSpeed = 0.01f;

    private float dissolveVal;
    private bool isChanging = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        DontDestroyOnLoad(this);

        dissolveVal = 0;
        foreach (Material m in materials)
        {
            m.SetFloat("_CutoutThresh", dissolveVal);
        }
    }


    private void Update()
    {
        if (isChanging && dissolveVal < 1.01f)
        {
            dissolveVal += TransitionSpeed * Time.deltaTime;
            foreach (Material m in materials)
            {
                m.SetFloat("_CutoutThresh", dissolveVal);
            }
            Debug.Log(dissolveVal);
        }
        //SpookyMaterial.SetFloat("_CutoutThresh", dissolveVal);
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AudioManager.Instance.PlayMusic(AUDIO_MUSIC.SIREN, false);
            isChanging = true;
        }
    }
    public float GetDissolveValue()
    {
        return dissolveVal;
    }
}
