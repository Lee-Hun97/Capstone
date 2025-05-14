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
                // 해당 자식에 MeshCollider 붙이기
                MeshCollider collider = mf.gameObject.AddComponent<MeshCollider>();
                collider.sharedMesh = mf.mesh;
                collider.convex = false; // convex 필요 여부에 따라 설정
            }
        }

        //MeshCollider collider = created3DModel.AddComponent<MeshCollider>();
        //collider.convex = true; // 복잡한 메시일 경우 true로 하면 물리 충돌이 불안정해질 수 있지만 true여야 충돌 가능

        //MeshFilter mf = created3DModel.AddComponent<MeshFilter>();
        //collider.sharedMesh = mf.mesh;

        //MeshRenderer renderer = modelGO.AddComponent<MeshRenderer>();
        //renderer.material = new Material(Shader.Find("Standard"));

        // 텍스처 자동 할당 (mtl 파일을 수동 파싱하거나 texture.jpg가 있다고 가정)
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
