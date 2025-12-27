using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentBGM : MonoBehaviour
{
    private static PersistentBGM instance;
    private AudioSource audioSource;

    [System.Serializable]
    public class SceneReference
    {
        public string sceneName;
    #if UNITY_EDITOR
        public UnityEditor.SceneAsset sceneAsset;
    #endif
    }

    [SerializeField] private SceneReference[] allowedScenes = new SceneReference[0];

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool playInThisScene = false;
        foreach (var entry in allowedScenes)
        {
            if (!string.IsNullOrEmpty(entry.sceneName) && scene.name == entry.sceneName)
            {
                playInThisScene = true;
                break;
            }
        }

        if (playInThisScene)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (allowedScenes == null) return;
        for (int i = 0; i < allowedScenes.Length; i++)
        {
            var entry = allowedScenes[i];
            if (entry == null) continue;
            if (entry.sceneAsset != null)
            {
                entry.sceneName = entry.sceneAsset.name;
            }
        }
    }
#endif
}
