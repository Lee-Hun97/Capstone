using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginManger : MonoBehaviour
{
    [SerializeField]private TMP_InputField IDInputField;
    [SerializeField] private TMP_InputField PassWordInputField;

    private string ID;
    private string PassWord;

    public void CheckPlayerData()
    {
        ID = IDInputField.text;
        PassWord = PassWordInputField.text;

        if (false)
        {
            //TODO 나중에 서버가 생기면 연동
            Debug.Log("Wrong Data");
        }
        else
        {
            Debug.Log($"{ID} {PassWord}");
            SendPlayerData();
            AppSceneManger.Instance.ChangeScene(1);
        }
    }

    public void SendPlayerData()
    {
        AppData.Instance.SetInfo(ID, PassWord);
    }
}
