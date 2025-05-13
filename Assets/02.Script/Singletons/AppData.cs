using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dummiesman;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class AppData : Singlton<AppData>
{
    //모든 데이터를 AppData가 가지고 있어서 변경이 편리하고 다른 곳에서 불필요한 데이터를 가지지않도록 구성
    //가능한 모든 데이터를 이곳에 모아준다.
    private string serverURL = "http://<서버IP>:5000/upload";//서버 이름으로 변경 필요
    private string serverModelUrl = ""; // 서버에 있는 .obj 경로
    private string captureImageFolderPath = Path.Combine(Application.temporaryCachePath, "CapturedImage");
    private string user3DModelPath = Path.Combine(Application.persistentDataPath, "User3DModels");//모델의 경우 재사용을 해야하기에 삭제의 위험이 적은 persistentpath 사용

    //중요 데이터이기에 읽기 전용으로, 변경이 필요한 값은 set도 설정
    public string ServerURL { get { return serverURL; } }
    public string CaptureImageFolderPath { get { return captureImageFolderPath; } }
    public string User3DModelPath { get { return user3DModelPath; } }

    public string ID;
    public string PW;
    public GameObject[] GameObjects = new GameObject[3];//사용자에게 제공할 3개의 모델 저장 기능
    public GameObject[] loadedModels = new GameObject[3]; // 받아온 GameObjects

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("AppData Initialized");
    }

    protected override void Start()
    {
        base.Start();
    }

    public void SetInfo(string id, string pw)
    {
        ID = id;
        PW = pw;

        for(int i =0;i<3; i++)
        {
            StartCoroutine(LoadModelFromServer(serverModelUrl, i));
        }
    }

    //private void SetModelData(GameObject[] gameObjects)//사용자의 저장데이터를 불러옴
    //{
    //    for (int i = 0; i < gameObjects.Length; i++)
    //    {
    //        GameObjects[i] = gameObject;
    //    }
    //}

    IEnumerator LoadModelFromServer(string url, int index)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string objData = request.downloadHandler.text;

            string tempPath = user3DModelPath + "/tempModel_" + index + ".obj";
            System.IO.File.WriteAllText(tempPath, objData);
        }
        else
        {
            Debug.LogError($"모델 {index} 다운로드 실패: {request.error}");
        }
    }

    void OnApplicationQuit()
    {
        if (Directory.Exists(captureImageFolderPath))
        {
            Directory.Delete(captureImageFolderPath, true);
            //Debug.Log("임시 이미지 폴더 삭제");
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Debug.Log("AppData destroyed");
    }
}
