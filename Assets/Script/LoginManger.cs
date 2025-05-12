using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Text;
using static LoginManger;

public class LoginManger : MonoBehaviour
{
    private string serverURL;
    [SerializeField]private TMP_InputField IDInputField;
    [SerializeField] private TMP_InputField PassWordInputField;

    [System.Serializable]
    public class LoginData//json 형태로 전잘해 줘야하므로 클래스 형식, 이후에 데이터를 추가할수 있음
    {
        public string ID;
        public string PassWord;
    }
    private LoginData loginData;//로그인을 위한 로그인 데이터 선언
    private bool isLogined = false;

    private void Start()
    {
        serverURL = AppData.Instance.ServerURL;
        loginData = new LoginData();
    }

    public void CheckPlayerData()
    {
        loginData.ID = IDInputField.text;
        loginData.PassWord = PassWordInputField.text;

        //StartCoroutine(SendPlayerData(loginData.ID, loginData.PassWord));
        AppSceneManger.Instance.ChangeScene(1);//성공했을 때만 이동
    }

    public IEnumerator SendPlayerData(string username, string password)
    {
        // JSON 데이터 구성
        string jsonData = JsonUtility.ToJson(new LoginData { ID = loginData.ID, PassWord = loginData.ID });

        // 요청 생성
        UnityWebRequest request = new UnityWebRequest(serverURL, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 전송
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            isLogined = true;
            Debug.Log("Login Success: " + request.downloadHandler.text);
        }
        else
        {
            //TODO : 로그인 실패 시 팝업 생성
            Debug.LogWarning("Login Failed: " + request.responseCode + " " + request.downloadHandler.text);
        }
    }
}
