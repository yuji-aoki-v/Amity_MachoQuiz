using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataSelect : MonoBehaviour
{
    public void HobbyQuizData()
    {
        SceneManager.LoadScene("HobbyQuizData");
    }

    public void MachoQuizData()
    {
        SceneManager.LoadScene("MachoQuizData");
    }

    public void Home()
    {
        SceneManager.LoadScene("Home");
    }
    
}
