using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditLeft : MonoBehaviour
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
        GetComponent<Animator>().SetTrigger("Idle");
    }
}
