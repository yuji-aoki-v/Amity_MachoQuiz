using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieRightPlayer : MonoBehaviour
{
    void Start()
    {
        // animator = GetComponent<Animator>(); // Animator を取得
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 左クリックで倒れる
        {
            GetComponent<Animator>().SetTrigger("DethTrigger"); // アニメーション発火
        }
    }
}
