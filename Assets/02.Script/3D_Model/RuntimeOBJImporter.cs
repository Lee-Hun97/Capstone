using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dummiesman;
using UnityEngine;

public class RuntimeOBJImporter : MonoBehaviour
{
    private string folderPath = "";
    private string modelFileName = "model.obj";
    private string modelmtlFileName = "model.mtl";

    [SerializeField]private ModelAdjustment modelAdjustment;

    void Start()
    {
        folderPath = AppData.Instance.ServerModelGetURL;
        string modelPath = Path.Combine(folderPath, modelFileName);
        string mtlpath = Path.Combine(Application.streamingAssetsPath, modelmtlFileName);

        LoadModel(modelPath, mtlpath);
    }

    void LoadModel(string path, string mtlpath)
    {
        OBJLoader importer = new OBJLoader();
        GameObject created3DModel = importer.Load(path,mtlpath);

        created3DModel.transform.position = Vector3.zero;

        MeshFilter[] meshFilters = created3DModel.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.mesh != null)
            {
                // �ش� �ڽĿ� MeshCollider ���̱�
                MeshCollider collider = mf.gameObject.AddComponent<MeshCollider>();
                collider.sharedMesh = mf.mesh;
                collider.convex = false; // convex �ʿ� ���ο� ���� ����
            }
        }

        //MeshCollider collider = created3DModel.AddComponent<MeshCollider>();
        //collider.convex = true; // ������ �޽��� ��� true�� �ϸ� ���� �浹�� �Ҿ������� �� ������ true���� �浹 ����

        //MeshFilter mf = created3DModel.AddComponent<MeshFilter>();
        //collider.sharedMesh = mf.mesh;

        //MeshRenderer renderer = modelGO.AddComponent<MeshRenderer>();
        //renderer.material = new Material(Shader.Find("Standard"));

        // �ؽ�ó �ڵ� �Ҵ� (mtl ������ ���� �Ľ��ϰų� texture.jpg�� �ִٰ� ����)
        string texturePath = Path.Combine(Path.GetDirectoryName(path), "texture.jpg");
        if (File.Exists(texturePath))
        {
            byte[] bytes = File.ReadAllBytes(texturePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            //renderer.material.mainTexture = tex;
        }

        modelAdjustment.targetObject = created3DModel.transform;
    }
}
