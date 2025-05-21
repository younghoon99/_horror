using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class WorldSoundController : UdonSharpBehaviour
{
    public AudioClip soundClip; // 재생할 사운드 파일

    public float maxDistance = 40f; // 최대 들리는 거리
    public float minDistance = 40f;  // 최소 들리는 거리
    public float fadeTime = 1f; // 페이드 아웃 시간

    private AudioSource audioSource; // AudioSource
    private bool isPlaying = false;  // 사운드가 재생 중인지 확인
    private bool isFadingOut = false;  // 페이드 아웃 상태 체크
    private bool isSoundStarted = false; // 사운드 시작 여부
    private float fadeOutTimer = 0f; // 페이드 아웃 타이머

    void Start()
    {
        // AudioSource 컴포넌트를 미리 추가해놓고 참조
        audioSource = GetComponent<AudioSource>();

        // 사운드 초기화
        audioSource.clip = soundClip;
        audioSource.loop = true;  // 반복 재생
        audioSource.spatialBlend = 1.0f; // 3D 사운드로 설정
        audioSource.minDistance = minDistance; // 최소 거리 설정
        audioSource.maxDistance = maxDistance; // 최대 거리 설정
        audioSource.rolloffMode = AudioRolloffMode.Linear; // 선형 감소 방식
        audioSource.playOnAwake = false; // 자동 재생 비활성화
    }

    void Update()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer; // 로컬 플레이어 참조

        if (localPlayer == null) return;

        // 플레이어가 지정된 범위 내에 있으면 사운드를 재생
        float distanceToPlayer = Vector3.Distance(localPlayer.GetPosition(), transform.position);

        // 사운드가 시작되지 않았고, 플레이어가 범위 내에 있으면 사운드를 시작
        if (distanceToPlayer <= maxDistance && !isPlaying && !isSoundStarted) {
            PlaySound();
            isSoundStarted = true; // 사운드가 시작되었음을 표시
        }
        // 플레이어가 범위 밖으로 나가면 사운드를 멈춤
        else if (distanceToPlayer > maxDistance && isPlaying && !isFadingOut) {
            StartFadeOut(); // 페이드 아웃 시작
        }

        // 페이드 아웃 처리
        if (fadeOutTimer > 0) {
            fadeOutTimer -= Time.deltaTime; // 타이머 감소
            audioSource.volume = Mathf.Lerp(1f, 0f, (fadeTime - fadeOutTimer) / fadeTime); // 볼륨을 서서히 감소
            if (fadeOutTimer <= 0f) {
                StopSound(); // 타이머가 0에 도달하면 소리 멈춤
            }
        }
    }

    private void PlaySound()
    {
        // 사운드가 재생 중이지 않다면만 사운드를 시작
        if (!isPlaying) {
            audioSource.Play();  // 사운드 시작
            isPlaying = true;
            isFadingOut = false;  // 페이드 아웃 상태 초기화
        }
    }

    private void StopSound()
    {
        // 사운드가 재생 중이면만 사운드를 멈춤
        if (isPlaying) {
            audioSource.Stop();  // 사운드 멈추기
            audioSource.volume = 1f; // 볼륨 복원
            isPlaying = false;
        }
    }

    private void StartFadeOut()
    {
        fadeOutTimer = fadeTime; // 페이드 아웃 타이머 설정
        isFadingOut = true; // 페이드 아웃 상태로 설정
    }

    // Gizmos를 사용하여 사운드 범위 시각화
    void OnDrawGizmosSelected()
    {
        // 색상 설정
        Gizmos.color = Color.green; // 최소 거리(짧은 거리)
        Gizmos.DrawWireSphere(transform.position, minDistance); // 최소 거리 시각화

        Gizmos.color = Color.red; // 최대 거리(긴 거리)
        Gizmos.DrawWireSphere(transform.position, maxDistance); // 최대 거리 시각화
    }

    public void StartWorldSound()
    {
        // 사운드가 아직 시작되지 않았으면 시작
        if (!isSoundStarted) {
            PlaySound();
            isSoundStarted = true;
        }
    }

    // 외부에서 월드 사운드를 멈추는 함수
    public void StopWorldSound()
    {
        StopSound();
    }
}
