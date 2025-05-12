using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.Networking;

public class WebCamTexture : MonoBehaviour
{
    [SerializeField] private RawImage previewImage;   // ī�޶� �ǽð� ȭ�� ǥ��
    [SerializeField] private RectTransform elbum; //recttransform�� ui�� ���� -> �Ǻ�ó�� ���� ������ �� �ְ�, ������ transform�� ���� ������ �����ϱ� ����
    [SerializeField] private Image lastCaptureImage; // ǥ������ ������ �Կ��� �̹���
    [SerializeField] private TextMeshProUGUI previewText; //���� ĸ���� �̹��� ��
    //public Transform galleryContent;// �������� �θ� ��ü
    //public GameObject imagePrefab;  // �������� �߰��� �̹��� ������, ���� �� �� �־�� ��

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
        Debug.Log("Shot");
        ChangeElbumInfo(photo);
    }

    private void ChangeElbumInfo(Texture2D photo)//�̸����� ������ ������Ʈ TODO : �Կ��� �̹����� �����ؼ� �����ϴ� ���
    {
        Sprite sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0.5f, 0.5f));
        lastCaptureImage.sprite = sprite;
        previewText.text = $"{ capturedImages.Count}";
        LayoutRebuilder.ForceRebuildLayoutImmediate(elbum);
    }

    public void SendToServer()//��Ȯ�ϰԴ� ĸ���� �̹��� ����
    {
        if (capturedImages != null)
        {
            StartCoroutine(SendImagesToServer());
            //ClearImageFoldaer(); //���ʿ��� �����ʹ� ����
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
            byte[] imageBytes = capturedImages[i].EncodeToJPG(75); // �뷮 ���̱� ���� 75% ǰ��
            form.AddBinaryData($"image_{i}", imageBytes, $"image_{i}.jpg", "image/jpeg");
        }

        UnityWebRequest request = UnityWebRequest.Post(AppData.Instance.ServerURL, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("Upload successful!");
        else
            Debug.LogError("Upload failed: " + request.error);
    }

    private void OnApplicationQuit()//�� ���� �� �����ϴ� ��ɾ�
    {
        //ClearImageFoldaer();
    }

    public void ClearImageFoldaer()//TODO : ĸ�� ���� �ʱ�ȭ, �ӽ� �� �����͸� ���� ���� ��� �߰�
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
