using System.Collections; // コレクション（リスト、配列など）の基本クラスを提供します。
using System.Collections.Generic; // ジェネリックコレクション（List<T>、Dictionary<TKey,TValue>など）を提供します。
using UnityEngine; // Unityの基本機能（ゲームオブジェクト、コンポーネント、シーン管理など）を提供します。
using UnityEngine.UI; // UnityのUIコンポーネント（ボタン、テキスト、画像など）を操作するための機能を提供します。
using TMPro; // TextMeshPro（高品質なテキスト表示用）に関するクラスを提供します。
using UnityEngine.SceneManagement; // Unityのシーン管理（シーンの読み込みや切り替えなど）を提供します。
using UnityEngine.Networking; // UnityでのHTTPリクエストやネットワーク通信を扱うためのクラスを提供します。
using System.Linq; // LINQ（言語統合クエリ）を使用してコレクションを操作するための機能を提供します。
using Newtonsoft.Json.Linq; // Newtonsoft.Jsonを使用するために必要 // JSONデータの操作と解析を行うためのクラスを提供します（Newtonsoft.Jsonライブラリ）。

/*--------------------------------------------------------------------------------------------*/
public class Macho_GameManager : MonoBehaviour
{
    // フィールド
    public int listNum; // クイズ数をカウント
    public int judge = 0; // 正解か不正解を判断する
    public int correctCount = 0; // 正解数
    public float typingSpeed = 0.02f; // 文章出すスピード
    private string fullText; // 問題文代入用
    private string currentText = ""; // 現在の問題文を徐々に代入

    // オブジェクト用フィールド
    public GameObject correctUi; // 〇
    public GameObject incorrectUi; // ×
    public GameObject resultUi; // リザルト画面
    public Button correctButton; // 正解のボタン
    public TextMeshProUGUI resultNumber; // リザルトの正解数文字

    // jsonファイル対応用unity内フィールド
    private QuizList quizList; // こちらのリストにjsonデータを挿入
    public TextMeshProUGUI quizNum;
    public TextMeshProUGUI quiz;
    public TextMeshProUGUI button1;
    public TextMeshProUGUI button2;
    public TextMeshProUGUI button3;
    public TextMeshProUGUI button4;

    // FirestoreのURL
    private string firestoreUrl = "https://firestore.googleapis.com/v1/projects/amity-4bad3/databases/(default)/documents/hobbyQuiz/quiz";
    // 表示済みクイズ追跡
    private List<int> displayedQuizIndices = new List<int>(){0};
    // 表示可能問題番号
    List<int> possibleNumbers  = new List<int>();
    // クイズ番号をランダムに選定
    int randomNumber;

    // ボタン4つをリストに格納
    public RectTransform[] buttons;
    // ボタンの位置のランダム値リスト
    private Vector2[] positions = new Vector2[]
    {
        new Vector2(-100, -10)
        ,new Vector2(100, -10)
        ,new Vector2(-100, -70)
        ,new Vector2(100, -70)
    };

    // jsonファイル用のクラスと変数を定義
    [System.Serializable]
    public class QuizValue{
        public string quiz_json;
        public string button1_json;
        public string button2_json;
        public string button3_json;
        public string button4_json;
    }

    // jsonファイル用のリストを定義
    [System.Serializable]
    public class QuizList {
        public List<QuizValue> quizs;
    }
    
/*--------------------------------------------------------------------------------------------*/
    // jsonファイル読み込み＆初回表示
    void Start()
    {
        // ボダンランダム配置
        SetRandomPosition();
        // 変数初期化
        listNum = 0;
        // 1からxまでの数字をリストに追加(x=クイズ数)
        for (int i = 1; i <= 44; i++)
        {
            possibleNumbers .Add(i);
        }
        // quizListの初期化
        quizList = new QuizList { quizs = new List<QuizValue>() };
        GetDataFromFirestore();
    }

    // ボタンをランダムに配置
    void SetRandomPosition()
    {
        List<Vector2> availablePositions = new List<Vector2>(positions);

        foreach (RectTransform button in buttons)
        {
            // ランダムな位置を選択
            int randomIndex = Random.Range(0, availablePositions.Count);
            // 選択した位置をボタンに適用
            button.anchoredPosition = availablePositions[randomIndex];
            // 選択した位置をリストから削除
            availablePositions.RemoveAt(randomIndex);
        }
    }

    // 問題の文章、選択肢を表示
    void Display(List<QuizValue> quizs, int listNum)
    {
        currentText = "";
        fullText = "";
        if (listNum >= quizList.quizs.Count) 
        {
            resultUi.SetActive(true);
        }else
        {
            quizNum.text = (listNum + 1).ToString();
            fullText = quizs[listNum].quiz_json;
            StartCoroutine(TypeText());
            button1.text = quizs[listNum].button1_json;
            button2.text = quizs[listNum].button2_json;
            button3.text = quizs[listNum].button3_json;
            button4.text = quizs[listNum].button4_json;
        }
    }

    // 文章を決まったスピードで徐々に出力
    IEnumerator TypeText()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            currentText += fullText[i];
            quiz.text = currentText;
            yield return new WaitForSeconds(typingSpeed);
        }
    } 

    // 正解の場合
    public void NextQuiz_correct()
    {
        StartCoroutine(NextQuiz_coroutine(0));
        correctCount ++;
        resultNumber.text = correctCount.ToString();

    }

    // 不正解の場合
    public void NextQuiz_incorrect()
    {
        StartCoroutine(NextQuiz_coroutine(1));
    }

    // 次のクイズへ
    IEnumerator NextQuiz_coroutine(int judge)
    {
        yield return new WaitForSeconds(1f);
        UiDisplayOn(judge);
        correctButton.GetComponent<Image>().color = Color.green; Invoke("ResetButtonColor", 1f);
        yield return new WaitForSeconds(1f);
        UiDisplayOff(judge);
        listNum++;
        // ボダンランダム配置
        SetRandomPosition();
        GetDataFromFirestore();
    }

    // 〇×を表示する
    void UiDisplayOn(int judge)
    {
        if (judge == 0)
        {
            correctUi.SetActive(true);
        }
        else
        {
            incorrectUi.SetActive(true);
        }
    }

    // 〇×を非表示にする
    void UiDisplayOff(int judge)
    {
        if (judge == 0)
        {
            correctUi.SetActive(false);
        }
        else
        {
            incorrectUi.SetActive(false);
        }
    }

    // 正解ボタンの色を戻す
    void ResetButtonColor()
    {
        correctButton.GetComponent<Image>().color = Color.white; // 元の色に戻す
    }

    // 再挑戦
    public void Continue()
    {
        SceneManager.LoadScene("HobbyQuiz");
    }

    // ホームへ戻る
    public void Home()
    {
        SceneManager.LoadScene("Home");
    }

/*--------------------------------------------------------------------------------------------*/
    // Firebaseに関するコード
    public void GetDataFromFirestore()
    {
        StartCoroutine(FetchDataFromFirestore());
    }
    // Firestoreからデータを取得するコルーチン
    IEnumerator FetchDataFromFirestore()
    {
        // 含まれていない数字を取得
        possibleNumbers = possibleNumbers.Except(displayedQuizIndices).ToList();
        // 含まれていない数字が存在する場合&クイズの出題数が5問以下の時
        if (possibleNumbers.Count > 0 && listNum < 5)
        {
            randomNumber = possibleNumbers[Random.Range(0, possibleNumbers.Count)]; // ランダムに選択
            displayedQuizIndices.Add(randomNumber); // 選ばれた数字をリストに追加
        }
        else
        {
            Debug.Log("すべての数字が表示済みです。");
            quizNum.text = "";
            quiz.text = "";
            button1.text = "";
            button2.text = "";
            button3.text = "";
            button4.text = "";
            resultUi.SetActive(true);
            yield break;
        }
        // Firestore APIキー
        string apiKey = "AIzaSyAjRLVe_pXqiO8eZs2CHGqN08VSo5DqjAc"; 
        string urlWithApiKey = firestoreUrl + randomNumber + "?key=" + apiKey;

        // HTTPリクエストの作成
        UnityWebRequest request = new UnityWebRequest(urlWithApiKey, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // リクエスト送信
        yield return request.SendWebRequest();

        // レスポンスの確認
        if (request.result == UnityWebRequest.Result.Success)
        {
            ProcessFetchedData(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Firestoreからのデータ取得に失敗: " + request.error);
        }
    }

    // 取得したデータを処理する関数
    private void ProcessFetchedData(string jsonDataGet)
    {
        // JSONをJObjectとして解析
        JObject json = JObject.Parse(jsonDataGet);

        // フィールドを持つFirestoreレスポンスを取得
        var fields = json["fields"];

        // fieldsがnullでないか確認
        if (fields != null)
        {
            // Firestoreのレスポンスの各フィールドにアクセス
            QuizValue quizValue = new QuizValue
            {
                quiz_json = fields["quiz_json"]?["stringValue"]?.ToString(),
                button1_json = fields["button1_json"]?["stringValue"]?.ToString(),
                button2_json = fields["button2_json"]?["stringValue"]?.ToString(),
                button3_json = fields["button3_json"]?["stringValue"]?.ToString(),
                button4_json = fields["button4_json"]?["stringValue"]?.ToString()
            };

            // 作成したQuizValueオブジェクトをリストに追加
            quizList.quizs.Add(quizValue);

            // 表示の更新
            Display(quizList.quizs, listNum);
        }
        else
        {
            Debug.LogError("fieldsが見つかりませんでした。Firestoreレスポンスを確認してください。");
        }
    }

    // Firestoreからのレスポンス構造に対応するクラス
    [System.Serializable]
    public class FirestoreResponse
    {
        public Dictionary<string, FirestoreStringValue> fields; // フィールドのディクショナリ
        public string name; // ドキュメントの名前
        public string createTime; // 作成時間
        public string updateTime; // 更新時間
    }

    [System.Serializable]
    public class FirestoreStringValue
    {
        public string stringValue; // 各フィールドの値
    }
}
