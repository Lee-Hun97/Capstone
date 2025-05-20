using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ServerInteract : MonoBehaviour
{
    private string serverURL = "";
    private string temImageFolderPath = "";

    // Start is called before the first frame update
    void Start()
    {
        serverURL = AppData.Instance.ServerURL;
        temImageFolderPath = AppData.Instance.ServerImageUploadURL;
    }

    public void SendImageToServer()
    {

    }
}
