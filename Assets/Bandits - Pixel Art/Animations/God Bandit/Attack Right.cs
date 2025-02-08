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
    void Update()
    {
        // マウスボタンクリックでマッチョ神が右腕を振りかざす
        if (Input.GetMouseButtonUp(0))
            GetComponent<Animator>().SetTrigger("Attack Right");
    }
}
