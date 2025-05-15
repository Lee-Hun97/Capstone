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
    [SerializeField]private TMP_InputField PassWordInputField;
    [SerializeField]private Image image;

    [System.Serializable]
    public class LoginData//json ���·� ������ ����ϹǷ� Ŭ���� ����, ���Ŀ� �����͸� �߰��Ҽ� ����
    {
        public string email;
        public string password;
    }
    private LoginData loginData;//�α����� ���� �α��� ������ ����
    private bool isLogined = false;

    private void Start()
    {
        serverURL = AppData.Instance.ServerLoginURL;
        loginData = new LoginData();
    }

    public void CheckPlayerData()
    {
        loginData.email = IDInputField.text;
        loginData.password = PassWordInputField.text;

        image.color = Color.green;
        StartCoroutine(SendPlayerData());
    }

    public IEnumerator SendPlayerData()
    {
        string email = "test@example.com";
        string password = "1234";
        string jsonData = $"{{\"email\":\"{email}\",\"password\":\"{password}\"}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(serverURL, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        request.timeout = 10;
        //request.chunkedTransfer = false;

        image.color = Color.white;

        // ��û ����
        yield return request.SendWebRequest();

        image.color = Color.blue;
        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            image.color = Color.red;
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            isLogined = true;
            Debug.Log("Login Success: " + request.downloadHandler.text);
            AppData.Instance.SetInfo(loginData.email, loginData.password);//�α��� ���� �� �������� �����͸� �����ͼ� ����
            AppSceneManger.Instance.ChangeScene(Scene_name.MainScene);//�������� ���� �̵�
        }
        else
        {
            
            //TODO : �α��� ���� �� �˾� ����
            Debug.LogWarning("Login Failed: " + request.responseCode + " " + request.downloadHandler.text);
        }
    }
}
