using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Attack_Right()
    {
        GetComponent<Animator>().SetTrigger("Attack Right");
    }
}
