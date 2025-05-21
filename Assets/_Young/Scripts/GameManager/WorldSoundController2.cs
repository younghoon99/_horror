using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class WorldSoundController2 : UdonSharpBehaviour  // 클래스 이름을 파일 이름과 동일하게 변경
{
    [Header("사운드 설정")]
    public AudioClip soundClip;              
    public float playbackVolume = 2f;        // 볼륨을 2로 변경

    [Header("거리 설정")]
    public float effectiveRadius = 40f;      

    private AudioSource audioSource;         
    private bool isPlaying = false;          
    private bool isSoundStarted = false;     

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        audioSource.clip = soundClip;
        audioSource.loop = true;                         
        audioSource.spatialBlend = 1.0f;                
        audioSource.volume = playbackVolume;             // 단순히 playbackVolume 사용
        
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = effectiveRadius;       
        audioSource.minDistance = effectiveRadius;       
        
        audioSource.playOnAwake = false;                
        audioSource.dopplerLevel = 0f;                  
    }

    void Update()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer == null) return;

        float distanceToPlayer = Vector3.Distance(localPlayer.GetPosition(), transform.position);

        if (distanceToPlayer <= effectiveRadius && !isPlaying && !isSoundStarted)
        {
            PlaySound();
            isSoundStarted = true;
        }
        else if (distanceToPlayer > effectiveRadius && isPlaying)
        {
            StopSound();
        }
    }

    public void PlaySound()
    {
        if (!isPlaying)
        {
            audioSource.Play();
            isPlaying = true;
        }
    }

    public void StopSound()
    {
        if (isPlaying)
        {
            audioSource.Stop();
            isPlaying = false;
        }
    }

    public void StartSound()
    {
        if (!isSoundStarted)
        {
            PlaySound();
            isSoundStarted = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, effectiveRadius);
    }
    //테스토로 글쓰쓰가
    //테스트2
    //테스트3
}