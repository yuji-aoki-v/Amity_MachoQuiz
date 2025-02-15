using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditRight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Death()
    {
        GetComponent<Animator>().SetTrigger("Death");
    }

    public void DeathGame()
    {
        GetComponent<Animator>().SetTrigger("DeathGame");
    }

    public void Idle()
    {
        Debug.Log("呼ばれたよ");
        GetComponent<Animator>().SetTrigger("Idle");
    }
}
