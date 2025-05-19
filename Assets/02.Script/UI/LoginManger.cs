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
    public class LoginData//json ���·� ������ ����ϹǷ� Ŭ���� ����, ���Ŀ� �����͸� �߰��Ҽ� ����
    {
        public string email;
        public string password;
    }
    private LoginData loginData;//�α����� ���� �α��� ������ ����

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

        // ��û ����
        yield return request.SendWebRequest();

        loginCheckImage.color = Color.blue;
        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            loginCheckImage.color = Color.red;
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
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
