using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSetter : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
	[SerializeField] private Slider slider;
	[SerializeField] private string type;

	private void Start()
	{
		slider.value = PlayerPrefs.GetFloat(type + "Volume", 0.5f);
	}
	public void SetVolume(float sliderValue)
    {
        mixer.SetFloat(type + "Volume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat(type + "Volume", sliderValue);
	}
}
