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
    public int right_correctCount = 0; // 正解数
    public int left_correctCount = 0; // 正解数
    public int god_correctCount = 0; // 正解数
    public int coroutineStopJudge = 0; // コルーチン停止判定
    public float typingSpeed = 0.02f; // 文章出すスピード
    public string player = "";
    private string fullText; // 問題文代入用
    private string currentText = ""; // 現在の問題文を徐々に代入
    private bool isKeyDownEnabled = true; // KeyDownの有効・無効フラグ
    private bool isKeyDownEnabledLeft = true; // 左プレイヤーのKeyDownの有効・無効フラグ
    private bool isKeyDownEnabledRight = true; // 右プレイヤーのKeyDownの有効・無効フラグ
    private int correctAnswerNum = 0; // 正解番号

    // オブジェクト用フィールド
    public GameObject Q_Button;
    public GameObject W_Button;
    public GameObject A_Button;
    public GameObject S_Button;
    public GameObject I_Button;
    public GameObject O_Button;
    public GameObject K_Button;
    public GameObject L_Button;
    public GameObject right_correctUi; // 〇
    public GameObject left_correctUi; // 〇
    public GameObject right_incorrectUi; // ×
    public GameObject left_incorrectUi; // ×
    public GameObject right_ele;
    public GameObject left_ele;
    public GameObject nextQuizUi; // クイズ進行画面
    public GameObject resultUi; // リザルト画面
    public GameObject timeUpUi;
    public GameObject[] right_mukimuki;
    public GameObject[] left_mukimuki;
    public GameObject[] god_mukimuki;
    public Button correctButton; // 正解のボタン
    public TextMeshProUGUI resultText; // リザルトの正解数文字
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
    private string firestoreUrl = "https://firestore.googleapis.com/v1/projects/amity-4bad3/databases/(default)/documents/machoQuiz/quiz";
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
        public string quiztext;
        public string button1;
        public string button2;
        public string button3;
        public string button4;
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
        for (int i = 1; i <= 30; i++)
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
        if(player == "rightPlayer")
        {
            right_ele.SetActive(true);
        }else if(player == "leftPlayer")
        {
            left_ele.SetActive(true);
        }
        if (ansNum == correctAnswerNum) {
            // TODO: プレイヤー番号に対応した人にポイント付与
            PointUi("correct");
            NextQuiz_correct();
        } else {
            PointUi("incorrect");
            NextQuiz_incorrect();
        }
    }

    public void PointUi(string correctjudge)
    {
        if (correctjudge == "correct") 
        {
            // 右プレイヤーのポイント判定
            if(player == "rightPlayer")
            {
                right_mukimuki[right_correctCount].GetComponent<SpriteRenderer>().color = new Color(right_mukimuki[0].GetComponent<SpriteRenderer>().color.r, right_mukimuki[0].GetComponent<SpriteRenderer>().color.g, right_mukimuki[0].GetComponent<SpriteRenderer>().color.b, 1f);
                right_correctCount += 1;
            }else if(player == "leftPlayer")
            {
                left_mukimuki[left_correctCount].GetComponent<SpriteRenderer>().color = new Color(left_mukimuki[0].GetComponent<SpriteRenderer>().color.r, left_mukimuki[0].GetComponent<SpriteRenderer>().color.g, left_mukimuki[0].GetComponent<SpriteRenderer>().color.b, 1f);
                left_correctCount += 1;
            }
        } else if(correctjudge == "incorrect") 
        {
            // 神のポイント増加\
            god_mukimuki[god_correctCount].GetComponent<SpriteRenderer>().color = new Color(god_mukimuki[0].GetComponent<SpriteRenderer>().color.r, god_mukimuki[0].GetComponent<SpriteRenderer>().color.g, god_mukimuki[0].GetComponent<SpriteRenderer>().color.b, 1f);
            god_correctCount += 1;
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
            
            Q_Button.GetComponent<Image>().color = Color.green; Invoke("ResetButtonColor", 1f);
            player = "leftPlayer";
            JudgeKeyDown(0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetKeyDownEnabled(false);
            SetKeyDownEnabledLeft(false);
            
            W_Button.GetComponent<Image>().color = Color.green; Invoke("ResetButtonColor", 1f);
            player = "leftPlayer";
            JudgeKeyDown(1);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetKeyDownEnabled(false);
            SetKeyDownEnabledLeft(false);
            
            A_Button.GetComponent<Image>().color = Color.green; Invoke("ResetButtonColor", 1f);
            player = "leftPlayer";
            JudgeKeyDown(2);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetKeyDownEnabled(false);
            SetKeyDownEnabledLeft(false);
            
            S_Button.GetComponent<Image>().color = Color.green; Invoke("ResetButtonColor", 1f);
            player = "leftPlayer";
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

            I_Button.GetComponent<Image>().color = Color.green; Invoke("ResetButtonColor", 1f);
            player = "rightPlayer";
            JudgeKeyDown(0);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SetKeyDownEnabled(false);
            SetKeyDownEnabledRight(false);
            
            O_Button.GetComponent<Image>().color = Color.green; Invoke("ResetButtonColor", 1f);
            player = "rightPlayer";
            JudgeKeyDown(1);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SetKeyDownEnabled(false);
            SetKeyDownEnabledRight(false);
            
            K_Button.GetComponent<Image>().color = Color.green; Invoke("ResetButtonColor", 1f);
            player = "rightPlayer";
            JudgeKeyDown(2);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetKeyDownEnabled(false);
            SetKeyDownEnabledRight(false);
            
            L_Button.GetComponent<Image>().color = Color.green; Invoke("ResetButtonColor", 1f);
            player = "rightPlayer";
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
            fullText = quizs[listNum].quiztext;

            // キー入力有効化
            SetKeyDownEnabled(true);
            SetKeyDownEnabledLeft(true);
            SetKeyDownEnabledRight(true);

            StartCoroutine(TypeText());
            button1.text = quizs[listNum].button1;
            button2.text = quizs[listNum].button2;
            button3.text = quizs[listNum].button3;
            button4.text = quizs[listNum].button4;
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
        coroutineStopJudge = 1;
    }

    // 不正解の場合
    public void NextQuiz_incorrect()
    {
        timeText.text = " ";
        StartCoroutine(NextQuiz_coroutine(1));
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
        if (quizNum.text == "7" || right_correctCount==3 || left_correctCount==3 || god_correctCount==3)
        {
            if(right_correctCount==3)
            {
                //右プレイヤー勝利
                resultText.text = "右プレイヤーの勝利!!!";
            }else if(left_correctCount==3)
            {
                //左プレイヤー勝利
                resultText.text = "左プレイヤーの勝利!!!";
            }else if(god_correctCount==3)
            {
                //ゲームオーバー処理
                resultText.text = "ゲームオーバー";
            }else if(quizNum.text == "7")
            {
                resultText.text = correctCount.ToString() + "問 正解しました。";
            }
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
        right_ele.SetActive(false);
        left_ele.SetActive(false);
        if (judge == 0)
        {
            if(player == "rightPlayer")
            {
                right_correctUi.SetActive(true);
            }else if(player == "leftPlayer")
            {
                left_correctUi.SetActive(true);
            }
        }
        else
        {
            if(player == "rightPlayer")
            {
                right_incorrectUi.SetActive(true);
            }else if(player == "leftPlayer")
            {
                left_incorrectUi.SetActive(true);
            }
        }
    }

    // 〇×を非表示にする
    void UiDisplayOff(int judge)
    {
        if (judge == 0)
        {
            right_correctUi.SetActive(false);
            left_correctUi.SetActive(false);
        }
        else
        {
            right_incorrectUi.SetActive(false);
            left_incorrectUi.SetActive(false);
        }
    }

    // 正解ボタンの色を戻す
    void ResetButtonColor()
    {
        correctButton.GetComponent<Image>().color = Color.white; // 元の色に戻す
        Q_Button.GetComponent<Image>().color = Color.white;
        W_Button.GetComponent<Image>().color = Color.white;
        A_Button.GetComponent<Image>().color = Color.white;
        S_Button.GetComponent<Image>().color = Color.white;
        I_Button.GetComponent<Image>().color = Color.white;
        O_Button.GetComponent<Image>().color = Color.white;
        K_Button.GetComponent<Image>().color = Color.white;
        L_Button.GetComponent<Image>().color = Color.white;
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
        // 含まれていない数字が存在する場合&クイズの出題数が7問以下の時
        if (possibleNumbers.Count > 0 && listNum < 7)
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
                quiztext = fields["quiztext"]?["stringValue"]?.ToString(),
                button1 = fields["button1"]?["stringValue"]?.ToString(),
                button2 = fields["button2"]?["stringValue"]?.ToString(),
                button3 = fields["button3"]?["stringValue"]?.ToString(),
                button4 = fields["button4"]?["stringValue"]?.ToString()
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