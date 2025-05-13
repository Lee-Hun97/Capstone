using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singlton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance = null;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                Create();
            return _instance;
        }
    }

    protected static void Create()
    {
        if (_instance == null)
        {
            T[] objects = FindObjectsOfType<T>();

            if (objects.Length > 0)
            {
                _instance = objects[0];

                for (int i = 1; i < objects.Length; ++i)
                {
                    if (Application.isPlaying)
                        Destroy(objects[i].gameObject);
                    else
                        DestroyImmediate(objects[i].gameObject);
                }
            }
            else
            {
                GameObject go = new GameObject(string.Format("{0}", typeof(T).Name));
                _instance = go.AddComponent<T>();
            }
        }
    }

    protected virtual void Awake()
    {
        Create();

        if (_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
        }
    }

    protected virtual void OnEnable() { }

    protected virtual void Start() { }

    protected virtual void Update() { }

    protected virtual void FixedUpdate() { }

    protected virtual void LateUpdate() { }

    protected virtual void OnDisable() { }

    protected virtual void OnDestroy()
    {
        _instance = null;
    }
}
