using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    float time = 0;

	public Text timerText;
	[SerializeField] private GameManager manager;
	private void Start()
	{
	}
	private void Update()
	{
		if(!manager.isEnded)
		{
			time += Time.deltaTime;
			DisplayTime(time);
		}
	}
	void DisplayTime(float time)
	{
		float minutes = Mathf.FloorToInt(time / 60);
		float seconds = Mathf.FloorToInt(time % 60);

		timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
	}

}
