using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.Networking;
using System;
using System.Reflection;

public class WebCamTexture : MonoBehaviour
{
    [SerializeField] private RawImage previewImage;   // 카메라 실시간 화면 표시
    [SerializeField] private RectTransform elbum; //recttransform은 ui를 위해 -> 피봇처럼 어디든 적용할 수 있게, 기존의 transform은 실제 씬에서 적용하기 위해
    [SerializeField] private Image lastCaptureImage; // 표시해줄 마지막 촬영한 이미지
    [SerializeField] private GameObject loadingImage; // 로딩시 표시해줄 이미지 -> 나중에 popupUI로 빼면 좋을듯?
    [SerializeField] private TextMeshProUGUI previewText; //현재 캡쳐한 이미지 수
    //public Transform galleryContent;// 갤러리의 부모 객체
    //public GameObject imagePrefab;  // 갤러리에 추가할 이미지 프리팹, 제거 할 수 있어야 함

    private string bundleUrl;

    private string persistentFolderPath;
    private string temFolderPath;

    private string curTimeStamp = "";
    private UnityEngine.WebCamTexture webCamTexture;
    [SerializeField]private List<Texture2D> capturedImages = new List<Texture2D>();

    [System.Serializable]//latest_timestamp 반환 json 형식
    public class UploadResponse
    {
        public string status;
        public string timestamp;
        public string message;
    }

    private void Start()
    {
        persistentFolderPath = AppData.Instance.ServerModelGetbyNameURL;
        temFolderPath = AppData.Instance.ServerImageUploadURL;
        bundleUrl = AppData.Instance.GetBundleUrl; //{id,timestamp}

        // 웹캠 시작
        webCamTexture = new UnityEngine.WebCamTexture();
        previewImage.texture = webCamTexture;
        webCamTexture.Play();
    }

    public void CapturePhoto()
    {
        // 현재 WebCamTexture의 프레임을 Texture2D로 저장
        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();

        // 리스트에 저장 후 미리보기 업데이트
        capturedImages.Add(photo);

        ChangeElbumInfo(photo);
    }

    private void ChangeElbumInfo(Texture2D photo)//미리보기 정보를 업데이트 TODO : 촬영한 이미지를 선택해서 삭제하는 기능
    {
        Sprite sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0.5f, 0.5f));
        lastCaptureImage.sprite = sprite;
        previewText.text = $"{capturedImages.Count}";

        LayoutRebuilder.ForceRebuildLayoutImmediate(elbum);//ui 새로고침
    }

    public void SaveandSendToServer()
    {
        if (capturedImages != null)
        {
            StartCoroutine(SendImagesToServer());
        }
        else
        {
            Debug.Log("No Photos");
        }
    }

    private IEnumerator SendImagesToServer()
    {
        WWWForm form = new WWWForm();//기본 폼
        form.AddField("user_id", AppData.Instance.EMAIL);//폼의 추가 정보

        loadingImage.SetActive(true);

        for (int i = 0; i < capturedImages.Count; i++)
        {
            byte[] imageBytes = capturedImages[i].EncodeToJPG(75); // 용량 줄이기 위해 75% 품질
            form.AddBinaryData("file", imageBytes, $"image_{i}.jpg", "image/jpeg");
        }

        UnityWebRequest request = UnityWebRequest.Post(AppData.Instance.ServerImageUploadURL, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Upload successful!");
            StartCoroutine(GetTimeStampinServer());
        }
        else
            Debug.LogError("Upload failed: " + request.error);
    }

    private IEnumerator GetTimeStampinServer()//이미지 업로드 후 모델 생성 시 가장 최근에 만들어진 파일일 것이기 때문에
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", AppData.Instance.EMAIL);

        loadingImage.SetActive(true);

        UnityWebRequest request = UnityWebRequest.Post(AppData.Instance.GetTimeStampURL, form);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            UploadResponse response = JsonUtility.FromJson<UploadResponse>(jsonResponse);

            //현재 타임스템프를 저장해서 재활용
            curTimeStamp = response.timestamp;
            AppData.Instance.CurentTimeStamp = curTimeStamp;

            yield return new WaitForSeconds(0.3f);
            StartCoroutine(RunRCinServer(curTimeStamp));
        }
        else
            Debug.LogError("Upload failed: " + request.error);
    }

    private IEnumerator RunRCinServer(string curtimestamp)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", AppData.Instance.EMAIL);
        form.AddField("timestamp", curtimestamp);

        loadingImage.SetActive(true);

        UnityWebRequest request = UnityWebRequest.Post(AppData.Instance.RunRCURL, form);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            loadingImage.SetActive(false);
            Debug.Log("3D Modeling Finished!");

            yield return new WaitForSeconds(1f);
            StartCoroutine(SaveCreatedUnityBundletoPath());

            yield return new WaitForSeconds(1f);//***WaitForSeconds은 호출 시 새로 생성하는 것이기 때문에 재사용 할거라면 dic 같은걸로 값을 저장하는 것이 좋다.
            AppSceneManger.Instance.ChangeScene(Scene_name.ModelScene);
        }
        else
            Debug.LogError("Upload failed: " + request.error);
    }

    IEnumerator SaveCreatedUnityBundletoPath()//현재 제작한 모델링을 번들로 저장(특정 경로에 저장만 해주고 활용은 다른 사람이 함.)
    {
        string timestamp = curTimeStamp;
        string url = $"{bundleUrl}?user_id={AppData.Instance.EMAIL}&timestamp={timestamp}";
        
        string path = Path.Combine(Application.persistentDataPath, AppData.Instance.bundleName);

        // 번들이 존재하지 않으면 다운로드
        if (!File.Exists(path))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllBytes(path, request.downloadHandler.data);
                    Debug.Log("Bundle saved to: " + path);
                }
                else
                {
                    Debug.LogError("Download failed: " + request.error);
                }
            }
        }
    }

    private void OnDestroy()
    {
        webCamTexture.Stop();
    }

}
