using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public float SFXMult;
    public float MusicMult;
    public AudioSource BGM;

    public void Awake()
    {
        if (Instance == null)
        {
            SFXMult = 1.0f;
            MusicMult = 1.0f;
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else {
            Destroy(this);
        }
    }
}
