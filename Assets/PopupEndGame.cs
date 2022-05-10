using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
using UnityEngine.UI;
public class PopupEndGame : MonoBehaviour
{ 
    [SerializeField] private Text textScore;
    public void Show(string score)
    {
        UIPopup popup = UIPopup.GetPopup("Endgame");
        popup.GetComponent<PopupEndGame>().textScore.text = score.ToString();
        popup.Show();
    }
}
