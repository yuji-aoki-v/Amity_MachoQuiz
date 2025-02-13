using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class AddData : MonoBehaviour
{
    public TMP_InputField docNameInput;  // ドキュメント名を入力するテキスト欄
    public TMP_InputField quizTextInput; // クイズの問題を入力するテキスト欄
    public TMP_InputField button1Input;  // 選択肢1
    public TMP_InputField button2Input;  // 選択肢2
    public TMP_InputField button3Input;  // 選択肢3
    public TMP_InputField button4Input;  // 選択肢4

    private string firestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/amity-4bad3/databases/(default)/documents/machoQuiz/";

    public void AddQuizToFirestore()
    {
        string docName = docNameInput.text.Trim();  // ドキュメント名
        if (string.IsNullOrEmpty(docName))
        {
            Debug.LogError("ドキュメント名が空です");
            return;
        }

        string quizText = quizTextInput.text.Trim(); // クイズの問題
        string button1 = button1Input.text.Trim();
        string button2 = button2Input.text.Trim();
        string button3 = button3Input.text.Trim();
        string button4 = button4Input.text.Trim();

        // Firestore に送るデータを作成
        JObject quizData = new JObject
        {
            ["fields"] = new JObject
            {
                ["quiztext"] = new JObject { ["stringValue"] = quizText },
                ["button1"] = new JObject { ["stringValue"] = button1 },
                ["button2"] = new JObject { ["stringValue"] = button2 },
                ["button3"] = new JObject { ["stringValue"] = button3 },
                ["button4"] = new JObject { ["stringValue"] = button4 }
            }
        };

        string url = firestoreBaseUrl + docName; // 指定した名前のドキュメントに保存
        Debug.Log("Request URL: " + url);
        StartCoroutine(PostDataToFirestore(url, quizData.ToString()));
    }

    IEnumerator PostDataToFirestore(string url, string jsonData)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "PATCH"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Firestoreにデータを追加成功: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Firestoreへのデータ追加に失敗: " + request.error);
            }
        }
    }
}