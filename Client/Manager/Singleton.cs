using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance = null;
    protected bool isDontDestroySet = false;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (isDontDestroySet == false)
        {
            if (transform.parent != null && transform.root != null)
                DontDestroyOnLoad(transform.root.gameObject);
            else
                DontDestroyOnLoad(gameObject);

            isDontDestroySet = true;
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance)
            Destroy(gameObject);

        Resources.UnloadUnusedAssets();
    }
}
