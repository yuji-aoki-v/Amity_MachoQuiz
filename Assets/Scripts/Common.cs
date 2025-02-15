using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common : MonoBehaviour
{
    private static Common instance; // シングルトン用のインスタンス
    private bool isFullScreen = false; // フルスクリーン状態を保持

    void Awake()
    {
        // 既に存在する場合は破棄
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // このオブジェクトをシーン遷移しても破棄しない
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // F11キーで切り替え
        if (Input.GetKeyDown(KeyCode.F11))
        {
            isFullScreen = !isFullScreen; // 状態を切り替え
            SetScreenMode(isFullScreen);
        }
    }

    void SetScreenMode(bool fullscreen)
    {
        if (fullscreen)
        {
            // フルスクリーンモード
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        else
        {
            // ウィンドウモード（1280x720）
            Screen.SetResolution(1280, 720, false);
        }
    }
}