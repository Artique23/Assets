using UnityEngine;

public class SceneAudioController : MonoBehaviour
{
    [SerializeField] private bool disableCarAudio = true;

    void Start()
    {
        if (disableCarAudio && CarSoundManager.Instance != null)
        {
            CarSoundManager.Instance.StopAllCarSounds();

            Destroy(CarSoundManager.Instance.gameObject);
        }
    }
}
