using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForMobileTemSave : MonoBehaviour
{
    private string tempImagePath;

    public RawImage previewImage;   // ī�޶� �ǽð� ȭ�� ǥ��
    public RectTransform elbum; //recttransform�� ui�� ���� -> �Ǻ�ó�� ���� ������ �� �ְ�, ������ transform�� ���� ������ �����ϱ� ����
    public Image lastCaptureImage; // ǥ������ ������ �Կ��� �̹���
    public TextMeshProUGUI previewText; //���� ĸ���� �̹��� ��
    //public Transform galleryContent;// �������� �θ� ��ü
    //public GameObject imagePrefab;  // �������� �߰��� �̹��� ������

    private UnityEngine.WebCamTexture webCamTexture;
    private string folderPath = "";
    [SerializeField] private List<Texture2D> capturedImages = new List<Texture2D>();

    // Start is called before the first frame update
    void Start()
    {
        folderPath = AppData.Instance.ServerImageSaveURL;

        //�Կ��� ���� ��ķ ����
        webCamTexture = new UnityEngine.WebCamTexture();
        previewImage.texture = webCamTexture;
        webCamTexture.Play();
    }

    private void OnApplicationQuit()
    {
        if (!string.IsNullOrEmpty(tempImagePath) && File.Exists(tempImagePath))
        {
            File.Delete(tempImagePath);
            Debug.Log("Deleted temp image on exit.");
        }
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
        // �������� �߰�
    }

    private void ChangeElbumInfo(Texture2D photo)//�̸����� ������ ������Ʈ
    {
        Sprite sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0.5f, 0.5f));
        lastCaptureImage.sprite = sprite;
        previewText.text = $"{capturedImages.Count}";
        LayoutRebuilder.ForceRebuildLayoutImmediate(elbum);
    }

    public void SaveTemImage()
    {
        int index = 0;
        string filename = $"temPicture{index}.jpg";
        string temFilePath = Path.Combine(Application.temporaryCachePath, filename);

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
}
