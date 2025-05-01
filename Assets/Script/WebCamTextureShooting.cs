using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class WebCamTextureShooting : MonoBehaviour
{
    public RawImage previewImage;   // 카메라 실시간 화면 표시
    public RectTransform elbum; //recttransform은 ui를 위해 -> 피봇처럼 어디든 적용할 수 있게, 기존의 transform은 실제 씬에서 적용하기 위해
    public Image lastCaptureImage; // 표시해줄 마지막 촬영한 이미지
    public TextMeshProUGUI previewText; //현재 캡쳐한 이미지 수
    //public Transform galleryContent;// 갤러리의 부모 객체
    //public GameObject imagePrefab;  // 갤러리에 추가할 이미지 프리팹

    private WebCamTexture webCamTexture;
    private string folderPath = @"C:/Work/CapturedImages";//이 경로도 앱으로 제작 시 변경
    [SerializeField]private List<Texture2D> capturedImages = new List<Texture2D>();

    private void Start()
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 웹캠 시작
        webCamTexture = new WebCamTexture();
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
        // 갤러리에 추가
    }

    private void ChangeElbumInfo(Texture2D photo)//미리보기 정보를 업데이트
    {
        Sprite sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0.5f, 0.5f));
        lastCaptureImage.sprite = sprite;
        previewText.text = $"{ capturedImages.Count}";
        LayoutRebuilder.ForceRebuildLayoutImmediate(elbum);
    }

    public void SendPhotoToRC()//정확하게는 캡쳐한 이미지 저장
    {
        if (capturedImages != null)
        {
            for (int i = 0; i < capturedImages.Count; i++)
            {
                string filePath = folderPath + "/photo_" + i + ".png";
                File.WriteAllBytes(filePath, capturedImages[i].EncodeToPNG());
                Debug.Log(i);
            }
        }
        else
        {
            Debug.Log("No Photos");
        }
    }

    //void addtogallery(texture2d texture)
    //{
    //    // 프리팹을 복제하여 갤러리 목록에 추가
    //    gameobject newimageobj = instantiate(imageprefab, gallerycontent);
    //    newimageobj.getcomponent<rawimage>().texture = texture;
    //}

    private void OnApplicationQuit()//앱 종료 시 수행하는 명령어
    {
        //ClearImageFoldaer();
    }

    public void ClearImageFoldaer()//캡쳐 폴더 초기화
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
}
