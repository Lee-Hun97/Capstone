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
    public class LoginData//json ���·� ������ ����ϹǷ� Ŭ���� ����, ���Ŀ� �����͸� �߰��Ҽ� ����
    {
        public string ID;
        public string PassWord;
    }
    private LoginData loginData;//�α����� ���� �α��� ������ ����
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
        AppSceneManger.Instance.ChangeScene(1);//�������� ���� �̵�
    }

    public IEnumerator SendPlayerData(string username, string password)
    {
        // JSON ������ ����
        string jsonData = JsonUtility.ToJson(new LoginData { ID = loginData.ID, PassWord = loginData.ID });

        // ��û ����
        UnityWebRequest request = new UnityWebRequest(serverURL, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // ��û ����
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            isLogined = true;
            Debug.Log("Login Success: " + request.downloadHandler.text);
        }
        else
        {
            //TODO : �α��� ���� �� �˾� ����
            Debug.LogWarning("Login Failed: " + request.responseCode + " " + request.downloadHandler.text);
        }
    }
}
