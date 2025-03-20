using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KanaInputManager : MonoBehaviour
{
    private string correctAnswer = "あいう"; // 任意の正解（変更可能）
    public GameObject Panel;
    public TextMeshProUGUI answerText; // 解答を表示するテキスト
    public Button[] kanaButtons; // 4つのボタン
    private string currentAnswer = ""; // 現在の入力内容

    private string allKana = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをん";

    void Start()
    {
        Panel.SetActive(true);
        SetRandomKana(); // 初回ランダム設定
    }

    public void OnAnswerButton()
    {
        ClearAnswer(); // 入力リセット
        Panel.SetActive(false);
    }

    public void OnKanaButtonPressed(TextMeshProUGUI kanaText)
    {
        currentAnswer += kanaText.text; // ボタンのテキストを取得して追加
        answerText.text = currentAnswer; // 画面に表示

        int currentIndex = currentAnswer.Length - 1; // 入力中のインデックス

        // 入力中の文字が正解の対応する文字と一致するか確認
        if (currentIndex < correctAnswer.Length && currentAnswer[currentIndex] == correctAnswer[currentIndex])
        {
            if (currentAnswer == correctAnswer) // 完全一致なら正解
            {
                answerText.text = "正解";
            }
        }
        else // 1文字でも間違えたら即不正解
        {
            answerText.text = "不正解";
            currentAnswer = "";
            SetRandomKana(); // ボタンを再抽選
        }
    }

    // 入力をクリアする処理
    public void ClearAnswer()
    {
        currentAnswer = "";
        answerText.text = "";
    }

    // ボタンにランダムなひらがなをセットする
    public void SetRandomKana()
    {
        HashSet<int> usedIndexes = new HashSet<int>(); // 重複防止

        for (int i = 0; i < kanaButtons.Length; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, allKana.Length);
            } while (usedIndexes.Contains(randomIndex));

            usedIndexes.Add(randomIndex);

            string kana = allKana[randomIndex].ToString();
            TextMeshProUGUI textComponent = kanaButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            if (textComponent != null)
            {
                textComponent.text = kana;
            }

            kanaButtons[i].onClick.RemoveAllListeners();
            kanaButtons[i].onClick.AddListener(() => OnKanaButtonPressed(textComponent));
        }
    }
}
