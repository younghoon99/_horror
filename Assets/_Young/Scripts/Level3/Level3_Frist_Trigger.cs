using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Level3_Frist_Trigger : UdonSharpBehaviour
{
    [Header("오브젝트 설정")]
    public GameObject targetObject;          // 트리거 발동 시 비활성화할 게임 오브젝트
    
    [Header("사운드 설정")]
    public AudioSource audioSource;          // 오디오 소스
    public AudioClip triggerSound;          // 재생할 사운드
    public float soundVolume = 2f;          // 사운드 볼륨
    public bool loopSound = false;          // 사운드 반복 재생 여부
    
     [SerializeField] private float maxSoundDistance = 20f;
     
    private bool hasTriggered = false;       // 트리거가 이미 발동되었는지 확인

    void Start()
    {
        // targetObject 확인
        if (targetObject == null)
        {
            Debug.LogWarning("Level3_Frist_Trigger: targetObject가 할당되지 않았습니다.");
            audioSource.maxDistance = maxSoundDistance;
        }

        // 오디오 소스 설정
        if (audioSource != null)
        {
            audioSource.clip = triggerSound;
            audioSource.volume = soundVolume;
            audioSource.loop = loopSound;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;  // 3D 사운드로 설정
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        // 로컬 플레이어이고 아직 트리거되지 않았을 때만 실행
        if (player == Networking.LocalPlayer && !hasTriggered)
        {
            // 오브젝트 비활성화
            if (targetObject != null)
            {
                targetObject.SetActive(false);
            }

            // 사운드 재생
            if (audioSource != null && triggerSound != null)
            {
                audioSource.Play();
            }

            hasTriggered = true;
        }
    }

   
}