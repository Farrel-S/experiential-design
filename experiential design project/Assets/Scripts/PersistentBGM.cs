using UnityEngine;

public class PersistentBGM : MonoBehaviour
{
    private static PersistentBGM instance;

    void Awake()
    {
        // Check if another BGM already exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Avoid duplicates
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Keep playing across scenes
    }
}

