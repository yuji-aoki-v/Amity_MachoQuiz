using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{
    public void HobbyQuiz()
    {
        SceneManager.LoadScene("HobbyQuiz");
    }

    public void MachoQuiz()
    {
        SceneManager.LoadScene("MachoQuiz");
    }

    public void BackHome()
    {
        SceneManager.LoadScene("Home");
    }
}
