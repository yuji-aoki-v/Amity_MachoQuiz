using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Linq; // .Last() を使用するために必要
using UnityEngine.SceneManagement;

public class MachoQuizData : MonoBehaviour
{
    public Transform contentPanel;  // ScrollView の Content に設定
    public GameObject itemPrefab;   // リストのアイテムPrefab
    public Button updateButtonPrefab; // 更新ボタンのPrefab

    private string firestoreUrl = "https://firestore.googleapis.com/v1/projects/amity-4bad3/databases/(default)/documents/machoQuiz";
    public GameObject canvasData;
    public GameObject canvasAdd;
    
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
            string documentId = doc["name"]?.ToString()?.Split('/').Last(); // ドキュメントIDを取得

            string quizText = fields["quiztext"]?["stringValue"]?.ToString() ?? "No Data";
            string button1 = fields["button1"]?["stringValue"]?.ToString() ?? "";
            string button2 = fields["button2"]?["stringValue"]?.ToString() ?? "";
            string button3 = fields["button3"]?["stringValue"]?.ToString() ?? "";
            string button4 = fields["button4"]?["stringValue"]?.ToString() ?? "";

            // アイテムを生成
            GameObject newItem = Instantiate(itemPrefab, contentPanel);

            TMP_InputField[] inputFields = newItem.GetComponentsInChildren<TMP_InputField>();

            if (inputFields.Length >= 6)  // フィールドが6つあることを確認
            {
                inputFields[0].text = documentId;  // ドキュメントIDを表示
                inputFields[0].readOnly = true;
                inputFields[1].text = quizText;    // クイズテキスト
                inputFields[2].text = button1;     // 選択肢1
                inputFields[3].text = button2;     // 選択肢2
                inputFields[4].text = button3;     // 選択肢3
                inputFields[5].text = button4;     // 選択肢4
            }

            // 更新ボタンを設定
            Button updateButton = Instantiate(updateButtonPrefab, contentPanel);
            // ボタンのサイズを変更
            RectTransform buttonRect = updateButton.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(200f, 50f);  // 幅200、高さ50に変更

            // LayoutElement を追加してサイズを固定
            LayoutElement layoutElement = updateButton.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = updateButton.gameObject.AddComponent<LayoutElement>();
            }
            layoutElement.preferredWidth = 200f;
            layoutElement.preferredHeight = 50f;
            updateButton.onClick.AddListener(() => UpdateFirestoreData(documentId, inputFields)); // ボタンのクリックイベント
        }
    }

    // Firestoreのデータを更新するメソッド
    private void UpdateFirestoreData(string documentId, TMP_InputField[] inputFields)
    {
        string url = $"https://firestore.googleapis.com/v1/projects/amity-4bad3/databases/(default)/documents/machoQuiz/{documentId}";  // 修正されたURL
        Debug.Log("Request URL: " + url);
        
        JObject updatedData = new JObject
        {
            ["fields"] = new JObject
            {
                ["quiztext"] = new JObject { ["stringValue"] = inputFields[1].text },
                ["button1"] = new JObject { ["stringValue"] = inputFields[2].text },
                ["button2"] = new JObject { ["stringValue"] = inputFields[3].text },
                ["button3"] = new JObject { ["stringValue"] = inputFields[4].text },
                ["button4"] = new JObject { ["stringValue"] = inputFields[5].text }
            }
        };

        StartCoroutine(PostDataToFirestore(url, updatedData));
    }

    // FirestoreにデータをPOSTするメソッド
    private IEnumerator PostDataToFirestore(string url, JObject data)
    {
        string jsonData = data.ToString();

        using (UnityWebRequest request = new UnityWebRequest(url, "PATCH"))  // PATCHに変更
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("データの更新に成功しました");
            }
            else
            {
                Debug.LogError("データの更新に失敗: " + request.error);
            }
        }
    }

    public void BackHome()
    {
        SceneManager.LoadScene("Home");
    }

    public void AddData()
    {
        canvasData.SetActive(false);
        canvasAdd.SetActive(true);
    }

    public void RevisionData()
    {
        canvasData.SetActive(true);
        canvasAdd.SetActive(false);
    }
}