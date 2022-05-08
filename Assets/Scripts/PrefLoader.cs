using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PrefLoader : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    void Start()
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 0.5f)) * 20);
        mixer.SetFloat("EffectVolume", Mathf.Log10(PlayerPrefs.GetFloat("EffectVolume", 0.5f)) * 20);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
