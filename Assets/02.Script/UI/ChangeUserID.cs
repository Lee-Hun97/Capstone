using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeUserID : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI user_idTXT;
    private string user_id;

    // Start is called before the first frame update
    void Start()
    {
        user_id = AppData.Instance.GetUserID();

        user_idTXT.text = user_id;
    }
}
