using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Over : MonoBehaviour
{

    public Text currentScore;
    public Text bestScore;

    // Start is called before the first frame update
    void Start()
    {
        currentScore.text = PlayerPrefs.GetInt("score").ToString();
        bestScore.text = PlayerPrefs.GetInt("best").ToString();
    }

    public void quitapp()
    {
        Application.Quit();
    }

    public void restart()
    {
        SceneManager.LoadScene("Game"); 
    }

}
