using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OrigolTrigger : UdonSharpBehaviour
{
    [Header("오르골 사운드 설정")]
    [SerializeField] private AudioSource origolSound;
    [SerializeField] private float maxSoundDistance = 20f;
    [SerializeField] private float volume = 1f;

    private void Start()
    {
        if (origolSound != null)
        {
            origolSound.spatialBlend = 1f;
            origolSound.minDistance = 1f;
            origolSound.maxDistance = maxSoundDistance;
            origolSound.rolloffMode = AudioRolloffMode.Linear;
            origolSound.volume = volume;
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal && origolSound != null)
        {
            origolSound.Play();
        }
    }
}