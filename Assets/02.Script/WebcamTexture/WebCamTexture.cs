using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.Networking;

public class WebCamTexture : MonoBehaviour
{
    [SerializeField] private RawImage previewImage;   // 카메라 실시간 화면 표시
    [SerializeField] private RectTransform elbum; //recttransform은 ui를 위해 -> 피봇처럼 어디든 적용할 수 있게, 기존의 transform은 실제 씬에서 적용하기 위해
    [SerializeField] private Image lastCaptureImage; // 표시해줄 마지막 촬영한 이미지
    [SerializeField] private TextMeshProUGUI previewText; //현재 캡쳐한 이미지 수
    //public Transform galleryContent;// 갤러리의 부모 객체
    //public GameObject imagePrefab;  // 갤러리에 추가할 이미지 프리팹, 제거 할 수 있어야 함

    private string folderPath;
    private UnityEngine.WebCamTexture webCamTexture;
    [SerializeField]private List<Texture2D> capturedImages = new List<Texture2D>();

    private void Start()
    {
        folderPath = Application.persistentDataPath;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

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
        Debug.Log("Shot");
        ChangeElbumInfo(photo);
    }

    private void ChangeElbumInfo(Texture2D photo)//미리보기 정보를 업데이트 TODO : 촬영한 이미지를 선택해서 삭제하는 기능
    {
        Sprite sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0.5f, 0.5f));
        lastCaptureImage.sprite = sprite;
        previewText.text = $"{ capturedImages.Count}";
        LayoutRebuilder.ForceRebuildLayoutImmediate(elbum);
    }

    public void SendToServer()//정확하게는 캡쳐한 이미지 저장
    {
        if (capturedImages != null)
        {
            StartCoroutine(SendImagesToServer());
            //ClearImageFoldaer(); //불필요한 데이터는 제거
        }
        else
        {
            Debug.Log("No Photos");
        }
    }

    private IEnumerator SendImagesToServer()
    {
        WWWForm form = new WWWForm();

        for (int i = 0; i < capturedImages.Count; i++)
        {
            byte[] imageBytes = capturedImages[i].EncodeToJPG(75); // 용량 줄이기 위해 75% 품질
            form.AddBinaryData($"image_{i}", imageBytes, $"image_{i}.jpg", "image/jpeg");
        }

        UnityWebRequest request = UnityWebRequest.Post(AppData.Instance.ServerURL, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("Upload successful!");
        else
            Debug.LogError("Upload failed: " + request.error);
    }

    private void OnApplicationQuit()//앱 종료 시 수행하는 명령어
    {
        //ClearImageFoldaer();
    }

    public void ClearImageFoldaer()//TODO : 캡쳐 폴더 초기화, 임시 앱 데이터를 삭제 등의 기능 추가
    {
        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
    }

    private void OnDestroy()
    {
        webCamTexture.Stop();
    }

}
