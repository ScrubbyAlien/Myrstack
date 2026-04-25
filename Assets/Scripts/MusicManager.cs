using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    
    void Start()
    {
        if (instance) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
