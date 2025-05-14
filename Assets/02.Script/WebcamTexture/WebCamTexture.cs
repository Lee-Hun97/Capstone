using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.Networking;
using System;

public class WebCamTexture : MonoBehaviour
{
    [SerializeField] private RawImage previewImage;   // ī�޶� �ǽð� ȭ�� ǥ��
    [SerializeField] private RectTransform elbum; //recttransform�� ui�� ���� -> �Ǻ�ó�� ���� ������ �� �ְ�, ������ transform�� ���� ������ �����ϱ� ����
    [SerializeField] private Image lastCaptureImage; // ǥ������ ������ �Կ��� �̹���
    [SerializeField] private GameObject loadingImage; // �ε��� ǥ������ �̹��� -> ���߿� popupUI�� ���� ������?
    [SerializeField] private TextMeshProUGUI previewText; //���� ĸ���� �̹��� ��
    //public Transform galleryContent;// �������� �θ� ��ü
    //public GameObject imagePrefab;  // �������� �߰��� �̹��� ������, ���� �� �� �־�� ��

    private string persistentFolderPath;
    private string temFolderPath;
    private UnityEngine.WebCamTexture webCamTexture;
    [SerializeField]private List<Texture2D> capturedImages = new List<Texture2D>();

    private void Start()
    {
        persistentFolderPath = AppData.Instance.ServerModelGetURL;
        temFolderPath = AppData.Instance.ServerImageSaveURL;

        // ��ķ ����
        webCamTexture = new UnityEngine.WebCamTexture();
        previewImage.texture = webCamTexture;
        webCamTexture.Play();
    }

    public void CapturePhoto()
    {
        // ���� WebCamTexture�� �������� Texture2D�� ����
        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();

        // ����Ʈ�� ���� �� �̸����� ������Ʈ
        capturedImages.Add(photo);

        ChangeElbumInfo(photo);
    }

    private void ChangeElbumInfo(Texture2D photo)//�̸����� ������ ������Ʈ TODO : �Կ��� �̹����� �����ؼ� �����ϴ� ���
    {
        Sprite sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0.5f, 0.5f));
        lastCaptureImage.sprite = sprite;
        previewText.text = $"{capturedImages.Count}";
        LayoutRebuilder.ForceRebuildLayoutImmediate(elbum);
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
        WWWForm form = new WWWForm();//�⺻ ��
        string serverURL = AppData.Instance.ServerImageSaveURL;//API ��� �ּ�
        form.AddField("user_id", AppData.Instance.ID);//���� �߰� ����

        for (int i = 0; i < capturedImages.Count; i++)
        {
            byte[] imageBytes = capturedImages[i].EncodeToJPG(75); // �뷮 ���̱� ���� 75% ǰ��
            form.AddBinaryData($"image_{i}", imageBytes, $"image_{i}.jpg", "image/jpeg");
        }

        UnityWebRequest request = UnityWebRequest.Post(AppData.Instance.ServerURL, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Upload successful!");
            StartCoroutine(RunRCinServer());
        }
        else
            Debug.LogError("Upload failed: " + request.error);
    }

    private IEnumerator RunRCinServer()
    {
        string runURL = "http://127.0.0.1:5000/latest_upload";
        WWWForm form = new WWWForm();

        form.AddField("user_id", AppData.Instance.ID);

        loadingImage.SetActive(true);
        
        UnityWebRequest request = UnityWebRequest.Post(AppData.Instance.ServerURL, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            loadingImage.SetActive(false);
            Debug.Log("3D Modeling Finished!");

            yield return new WaitForSeconds(1f);
            AppSceneManger.Instance.ChangeScene(Scene_name.ModelScene);
        }
        else
            Debug.LogError("Upload failed: " + request.error);
    }

    private void OnDestroy()
    {
        webCamTexture.Stop();
    }

}
