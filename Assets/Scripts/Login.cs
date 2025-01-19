using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class Login : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_InputField newUsernameField;
    public TMP_InputField newPasswordField;
    public GameObject newAccountUi;

    // FirestoreのURLとAPIキー
    private string firestoreUrl = "https://firestore.googleapis.com/v1/projects/amity-4bad3/databases/(default)/documents/account/user1";
    private string apiKey = "AIzaSyAjRLVe_pXqiO8eZs2CHGqN08VSo5DqjAc"; // FirestoreのAPIキーを入力

    void Start()
    {
        #if UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = true;
        #endif
        usernameField.onSelect.AddListener((string text) =>
        {
            OpenMobileKeyboard();
        });
    }

    void OpenMobileKeyboard()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    public class Account
    {
        public string password; //{ get; set; }
        public string username; //{ get; set; }
    }
/*--------------------------------------------------------------------------------------------*/
    // SHA256 を使ったパスワードのハッシュ化
    public static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(password);
            byte[] hashArray = sha256.ComputeHash(byteArray);

            StringBuilder hashString = new StringBuilder();
            foreach (byte b in hashArray)
            {
                hashString.Append(b.ToString("x2"));
            }
            return hashString.ToString();
        }
    }
/*--------------------------------------------------------------------------------------------*/
    // アカウント情報をFirestoreから取得して検証
    public void LoginAccount()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        StartCoroutine(VerifyLogin(username, password));
    }

private IEnumerator VerifyLogin(string username, string inputPassword)
{
    username = username.Replace("\u200B", "");
    // Firestore APIキー
    string apiKey = "AIzaSyAjRLVe_pXqiO8eZs2CHGqN08VSo5DqjAc"; 
    string urlWithApiKey = firestoreUrl + "?key=" + apiKey;

    // HTTPリクエストの作成
    UnityWebRequest request = new UnityWebRequest(urlWithApiKey, "GET");
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");

    // リクエスト送信
    yield return request.SendWebRequest();

    // JSONをJObjectとして解析
    JObject json = JObject.Parse(request.downloadHandler.text);
    // フィールドを持つFirestoreレスポンスを取得
    var fields = json["fields"];

    if (request.result == UnityWebRequest.Result.Success)
    {
        string storedPasswordHash = fields["password"]?["stringValue"]?.ToString();
        string storedUsername = fields["username"]?["stringValue"]?.ToString();

        // ユーザー名とパスワードを確認
        if (storedUsername == username && storedPasswordHash == HashPassword(inputPassword))
        {
            Debug.Log("ログイン成功");
            SceneManager.LoadScene("Home");
        }
        else
        {
            Debug.Log("ログインに失敗しました");
        }
    }
    else
    {
        Debug.LogError("Firestoreからのデータ取得に失敗: " + request.error);
    }
}

    public void NewAccount()
    {
        newAccountUi.SetActive(true);
    }

    public void OnSelectUser() {
        usernameField.Select();
        usernameField.ActivateInputField();
    }

    public void OnSelectPass() {
        passwordField.Select();
        passwordField.ActivateInputField();
    }

}