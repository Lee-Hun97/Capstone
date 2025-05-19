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
    [SerializeField]private TMP_InputField IDInputField;
    [SerializeField]private TMP_InputField PassWordInputField;
    [SerializeField]private Image loginCheckImage;

    [System.Serializable]
    public class LoginData//json 형태로 전잘해 줘야하므로 클래스 형식, 이후에 데이터를 추가할수 있음
    {
        public string email;
        public string password;
    }
    private LoginData loginData;//로그인을 위한 로그인 데이터 선언

    private void Start()
    {
        loginData = new LoginData();
    }

    public void CheckPlayerData()
    {
        loginData.email = IDInputField.text;
        loginData.password = PassWordInputField.text;

        StartCoroutine(SendPlayerData());
    }

    public IEnumerator SendPlayerData()
    {
        string jsonData = JsonUtility.ToJson(loginData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(AppData.Instance.ServerLoginURL, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        request.timeout = 10;
        //request.chunkedTransfer = false;

        loginCheckImage.color = Color.white;

        // 요청 전송
        yield return request.SendWebRequest();

        loginCheckImage.color = Color.blue;
        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            loginCheckImage.color = Color.red;
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Login Success: " + request.downloadHandler.text);
            AppData.Instance.SetInfo(loginData.email, loginData.password);//로그인 성공 시 서버에서 데이터를 가져와서 저장
            AppSceneManger.Instance.ChangeScene(Scene_name.MainScene);//성공했을 때만 이동
        }
        else
        {
            
            //TODO : 로그인 실패 시 팝업 생성
            Debug.LogWarning("Login Failed: " + request.responseCode + " " + request.downloadHandler.text);
        }
    }
}
