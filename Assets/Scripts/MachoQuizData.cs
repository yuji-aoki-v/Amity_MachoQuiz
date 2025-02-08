using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement; // Unityのシーン管理（シーンの読み込みや切り替えなど）を提供します

public class MachoQuizData : MonoBehaviour
{
    public Transform contentPanel;  // ScrollView の Content に設定
    public GameObject itemPrefab;   // リストのアイテムPrefab

    private string firestoreUrl = "https://firestore.googleapis.com/v1/projects/amity-4bad3/databases/(default)/documents/machoQuiz";
    
    void Start()
    {
        StartCoroutine(GetDataFromFirestore());
    }

    IEnumerator GetDataFromFirestore()
    {
        string urlWithApiKey = firestoreUrl; // APIキーなし（Firestoreのルール設定を変更する）

        using (UnityWebRequest request = UnityWebRequest.Get(urlWithApiKey))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ProcessFetchedData(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Firestoreからのデータ取得に失敗: " + request.error);
            }
        }
    }

    private void ProcessFetchedData(string jsonData)
    {
        JObject json = JObject.Parse(jsonData);
        JArray documents = (JArray)json["documents"];

        foreach (Transform child in contentPanel)  // 既存リストをクリア
        {
            Destroy(child.gameObject);
        }

        foreach (JObject doc in documents)
        {
            JObject fields = (JObject)doc["fields"];

            string quizText = " 0   " + fields["quiztext"]?["stringValue"]?.ToString() ?? "No Data";
            string button1 = "   1   " + fields["button1"]?["stringValue"]?.ToString() ?? "";
            string button2 = "   2   " + fields["button2"]?["stringValue"]?.ToString() ?? "";
            string button3 = "   3   " + fields["button3"]?["stringValue"]?.ToString() ?? "";
            string button4 = "   4   " + fields["button4"]?["stringValue"]?.ToString() ?? "";

            // アイテムを生成
            GameObject newItem = Instantiate(itemPrefab, contentPanel);
            TMP_Text[] texts = newItem.GetComponentsInChildren<TMP_Text>();
            texts[0].text = quizText;
            texts[1].text = button1;
            texts[2].text = button2;
            texts[3].text = button3;
            texts[4].text = button4;
        }
    }
}