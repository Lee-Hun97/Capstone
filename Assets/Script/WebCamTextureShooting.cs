using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class WebCamTextureShooting : MonoBehaviour
{
    public RawImage previewImage;   // ī�޶� �ǽð� ȭ�� ǥ��
    public RectTransform elbum; //recttransform�� ui�� ���� -> �Ǻ�ó�� ���� ������ �� �ְ�, ������ transform�� ���� ������ �����ϱ� ����
    public Image lastCaptureImage; // ǥ������ ������ �Կ��� �̹���
    public TextMeshProUGUI previewText; //���� ĸ���� �̹��� ��
    //public Transform galleryContent;// �������� �θ� ��ü
    //public GameObject imagePrefab;  // �������� �߰��� �̹��� ������

    private WebCamTexture webCamTexture;
    private string folderPath = @"C:/Work/CapturedImages";//�� ��ε� ������ ���� �� ����
    [SerializeField]private List<Texture2D> capturedImages = new List<Texture2D>();

    private void Start()
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // ��ķ ����
        webCamTexture = new WebCamTexture();
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
        // �������� �߰�
    }

    private void ChangeElbumInfo(Texture2D photo)//�̸����� ������ ������Ʈ
    {
        Sprite sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0.5f, 0.5f));
        lastCaptureImage.sprite = sprite;
        previewText.text = $"{ capturedImages.Count}";
        LayoutRebuilder.ForceRebuildLayoutImmediate(elbum);
    }

    public void SendPhotoToRC()//��Ȯ�ϰԴ� ĸ���� �̹��� ����
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
    //    // �������� �����Ͽ� ������ ��Ͽ� �߰�
    //    gameobject newimageobj = instantiate(imageprefab, gallerycontent);
    //    newimageobj.getcomponent<rawimage>().texture = texture;
    //}

    private void OnApplicationQuit()//�� ���� �� �����ϴ� ��ɾ�
    {
        //ClearImageFoldaer();
    }

    public void ClearImageFoldaer()//ĸ�� ���� �ʱ�ȭ
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
