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
    // 大橋変更分
    // フィールド
    public int listNum; // クイズ数をカウント
    public int judge = 0; // 正解か不正解を判断する
    public int correctCount = 0; // 正解数
    public int coroutineStopJudge = 0; // コルーチン停止判定
    public float typingSpeed = 0.02f; // 文章出すスピード
    private string fullText; // 問題文代入用
    private string currentText = ""; // 現在の問題文を徐々に代入
    private bool isKeyDownEnabled = true; // KeyDownの有効・無効フラグ
    private bool isKeyDownEnabledLeft = true; // 左プレイヤーのKeyDownの有効・無効フラグ
    private bool isKeyDownEnabledRight = true; // 右プレイヤーのKeyDownの有効・無効フラグ
    private int correctAnswerNum = 0; // 正解番号

    // オブジェクト用フィールド
    public GameObject correctUi; // 〇
    public GameObject incorrectUi; // ×
    public GameObject nextQuizUi; // クイズ進行画面
    public GameObject resultUi; // リザルト画面
    public GameObject timeUpUi;
    public GameObject[] mukimuki;
    public Button correctButton; // 正解のボタン
    public TextMeshProUGUI resultNumber; // リザルトの正解数文字
    public TextMeshProUGUI nextQuizeText; // 次のクイズ表示用ボタンの文字

    // jsonファイル対応用unity内フィールド
    private QuizList quizList; // こちらのリストにjsonデータを挿入
    public TextMeshProUGUI quizNum;
    public TextMeshProUGUI quiz;
    public TextMeshProUGUI button1;
    public TextMeshProUGUI button2;
    public TextMeshProUGUI button3;
    public TextMeshProUGUI button4;
    public TextMeshProUGUI timeText;

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
        new Vector2(-104, -95)
        ,new Vector2(96,-95)
        ,new Vector2(-104,-155)
        ,new Vector2(96, -155)
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
        // 変数初期化
        listNum = 0;
        // 1からxまでの数字をリストに追加(x=クイズ数)
        for (int i = 1; i <= 44; i++)
        {
            possibleNumbers .Add(i);
        }
        // quizListの初期化
        quizList = new QuizList { quizs = new List<QuizValue>() };
        nextQuizeText.text = "スタート";
        nextQuizUi.SetActive(true);
    }

    void Update()
    {
        if (timeText.text == "0")
        {
            timeText.text = " ";
            StartCoroutine(TimeUp());
        }

        ControlLeftPlayer();
        ControlRightPlayer();
    }

    // KeyDown イベントを有効化・無効化するメソッド
    public void SetKeyDownEnabled(bool enabled)
    {
        isKeyDownEnabled = enabled;
    }
    // 左プレイヤーのKeyDown イベントを有効化・無効化するメソッド
    public void SetKeyDownEnabledLeft(bool enabled)
    {
        isKeyDownEnabledLeft = enabled;
    }
    // 右プレイヤーのKeyDown イベントを有効化・無効化するメソッド
    public void SetKeyDownEnabledRight(bool enabled)
    {
        isKeyDownEnabledRight = enabled;
    }

    // 正解判定するメソッド
    // TODO: カリー化してプレイヤー番号を受け取る
    public void JudgeKeyDown(int ansNum)
    {
        if (ansNum == correctAnswerNum) {
            // TODO: プレイヤー番号に対応した人にポイント付与
            NextQuiz_correct();
        } else {
            NextQuiz_incorrect();
        }
    }

    public void ControlLeftPlayer()
    {
        // 無効化中は処理しない
        if (!isKeyDownEnabled) return;
        if (!isKeyDownEnabledLeft) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetKeyDownEnabled(false);
            SetKeyDownEnabledLeft(false);
            
            JudgeKeyDown(0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetKeyDownEnabled(false);
            SetKeyDownEnabledLeft(false);
            
            JudgeKeyDown(1);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetKeyDownEnabled(false);
            SetKeyDownEnabledLeft(false);
            
            JudgeKeyDown(2);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetKeyDownEnabled(false);
            SetKeyDownEnabledLeft(false);
            
            JudgeKeyDown(3);
        }
    }

    public void ControlRightPlayer()
    {
        // 無効化中は処理しない
        if (!isKeyDownEnabled) return;
        if (!isKeyDownEnabledRight) return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            SetKeyDownEnabled(false);
            SetKeyDownEnabledRight(false);

            JudgeKeyDown(0);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SetKeyDownEnabled(false);
            SetKeyDownEnabledRight(false);
            
            JudgeKeyDown(1);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SetKeyDownEnabled(false);
            SetKeyDownEnabledRight(false);
            
            JudgeKeyDown(2);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetKeyDownEnabled(false);
            SetKeyDownEnabledRight(false);
            
            JudgeKeyDown(3);
        }
    }

    IEnumerator TimeUp()
    {
        timeUpUi.SetActive(true);
        // タイムアップ処理
        yield return new WaitForSeconds(5f);
        timeUpUi.SetActive(false);
        NextQuiz_incorrect();
    }

    void StartTime()
    {
        timeLimitCoroutine = StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        timeText.text = "10";
        for (int i = 10; i >= 0 && timeText.text != " "; i--)
        {
            timeText.text = i.ToString(); // 現在のカウントを表示
            yield return new WaitForSeconds(1f); // 1秒待機
        }
    }

    // 次のクイズへ
    public void NextQuiz()
    {
        // タイムリミットリセット処理
        StartOrResetTimeLimit();
        // 新しいコルーチンを開始
        coroutineStopJudge = 0;
        StartTime();
        // nextQuizUi.SetActive(false);
        // ボダンランダム配置
        SetRandomPosition();
        GetDataFromFirestore();
        // nextQuizUi.SetActive(false);
    }

    // クイズ開始
    public void StartQuiz()
    {
        // ボダンランダム配置
        SetRandomPosition();
        GetDataFromFirestore();
        nextQuizUi.SetActive(false);
        StartTime();
    }

    private Coroutine timeLimitCoroutine; // 実行中のコルーチンを保持
    // タイムリミットを開始またはリセット
    public void StartOrResetTimeLimit() // タイムリミットをリセットして再スタート
    {
        // 実行中のコルーチンがあれば停止
        if (timeLimitCoroutine != null)
        {
            StopCoroutine(timeLimitCoroutine);
            StopTimeLimit();
        }
    }

    // コルーチンをリセットする処理
    // コルーチンを停止して再起動することでリセットと同等の処理を実現できます
    // タイムリミットを手動停止
    public void StopTimeLimit()
    {
        //StopCoroutine(timeLimitCoroutine);
        timeLimitCoroutine = null;
        Debug.Log("タイムリミットがリセットされました");
        coroutineStopJudge = 1;
    }

    // ボタンを押すか30秒経過したらコルーチンを終了後、次の問題へ
    IEnumerator StartTimeLimit()
    {
        Debug.Log("タイムリミットスタート！");
        yield return new WaitForSeconds(5f); // 30秒待つ 検証時5秒
        if (!resultUi.activeSelf) // リザルト画面が表示されていない場合
        {
            if (coroutineStopJudge == 0)
            {
                NextQuiz_incorrect(); // 次の問題へ移動（不正解扱い）
            }
            coroutineStopJudge = 0;
            //yield break;
        }
    }

    // ボタンをランダムに配置
    void SetRandomPosition()
    {
        List<Vector2> availablePositions = new List<Vector2>(positions);

        int firstRandomIndex = -1; // 初期値を設定

        foreach (RectTransform button in buttons)
        {
            // ランダムな位置を選択
            int randomIndex = Random.Range(0, availablePositions.Count);

            // 正解のインデックスを記録（1回目のみ保存）
            if (firstRandomIndex == -1)
            {
                firstRandomIndex = randomIndex;
            }

            // 選択した位置をボタンに適用
            button.anchoredPosition = availablePositions[randomIndex];
            // 選択した位置をリストから削除
            availablePositions.RemoveAt(randomIndex);
        }

        correctAnswerNum = firstRandomIndex;
    }

    // 問題の文章、選択肢を表示
    void Display(List<QuizValue> quizs, int listNum)
    {
        currentText = "";
        fullText = "";
        if (listNum >= quizList.quizs.Count) 
        {
            //不要?
            resultUi.SetActive(true);
        }else
        {
            quizNum.text = (listNum + 1).ToString();
            fullText = quizs[listNum].quiz_json;

            // キー入力有効化
            SetKeyDownEnabled(true);
            SetKeyDownEnabledLeft(true);
            SetKeyDownEnabledRight(true);

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
/*--------------------------------------------------------------------------------------------*/
    // 正解の場合
    public void NextQuiz_correct()
    {
        timeText.text = " ";
        StartCoroutine(NextQuiz_coroutine(0));
        correctCount ++;
        resultNumber.text = correctCount.ToString();
        coroutineStopJudge = 1;
    }

    // 不正解の場合
    public void NextQuiz_incorrect()
    {
        timeText.text = " ";
        StartCoroutine(NextQuiz_coroutine(1));
        GetComponent<Animator>().SetTrigger("Attack Right");
    }

    // 正解不正解判定
    IEnumerator NextQuiz_coroutine(int judge)
    {
        yield return new WaitForSeconds(1f);
        UiDisplayOn(judge);
        correctButton.GetComponent<Image>().color = Color.green; Invoke("ResetButtonColor", 3f);
        yield return new WaitForSeconds(3f);
        UiDisplayOff(judge);
        listNum++;
        if (quizNum.text == "5")
        {
            resultUi.SetActive(true);
        }
        else
        {
            NextQuiz();
        }
    }
/*--------------------------------------------------------------------------------------------*/
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
            // mukimuki[correctCount - 1].GetComponent<SpriteRenderer>().color = new Color(mukimuki[0].GetComponent<SpriteRenderer>().color.r, mukimuki[0].GetComponent<SpriteRenderer>().color.g, mukimuki[0].GetComponent<SpriteRenderer>().color.b, 1f);
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
        SceneManager.LoadScene("MachoQuiz");
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