using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewMenu : MonoBehaviour
{
    [SerializeField] private GameObject loadingGameObj;
    public void QuitGame()
    {
        Application.Quit();
	}

    public void ShowLoading()
    {
        loadingGameObj.SetActive(true);
	}
}
