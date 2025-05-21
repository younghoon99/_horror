using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FootstepSound : UdonSharpBehaviour
{
    public AudioClip footstepSound; // 발자국 소리 파일
    public float footstepInterval = 0.3f; // 발자국 소리 간격 (초)
    public float maxPlayTime = 1.5f; // 발자국 소리 최대 재생 시간 (초)

    private AudioSource audioSource; // 발자국 소리를 재생할 AudioSource
    private float nextFootstepTime; // 다음 발자국 소리가 재생될 시간
    private float footstepStartTime; // 발자국 소리 시작 시간
    private bool isPlayingFootstep; // 발자국 소리 재생 상태
    private VRCPlayerApi localPlayer; // Local Player 참조

    void Start()
    {
        // Local Player 가져오기
        localPlayer = Networking.LocalPlayer;

        // AudioSource 컴포넌트 가져오기 (미리 추가된 AudioSource 사용)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) {
            Debug.LogError("AudioSource 컴포넌트가 없습니다. 오브젝트에 AudioSource를 추가하세요.");
            return;
        }

        // AudioSource 설정
        audioSource.clip = footstepSound;
        audioSource.loop = false; // 반복 재생 비활성화
        audioSource.spatialBlend = 0.5f; // 3D 사운드 활성화
        audioSource.playOnAwake = false; // 자동 재생 비활성화
    }

    void Update()
    {
        if (localPlayer == null) return; // Local Player가 없으면 종료

        // 플레이어가 이동 중인지 확인
        if (IsPlayerMoving() && Time.time >= nextFootstepTime && !isPlayingFootstep) {
            PlayFootstepSound();
            nextFootstepTime = Time.time + footstepInterval; // 다음 발자국 시간 업데이트
        }

        // 발자국 소리가 재생 중이고, 최대 재생 시간을 초과했으면 중지
        if (isPlayingFootstep && Time.time - footstepStartTime >= maxPlayTime) {
            StopFootstepSound();
        }
    }

    private bool IsPlayerMoving()
    {
        // 플레이어의 속도를 확인하여 이동 여부 반환
        return localPlayer.GetVelocity().magnitude > 0.1f; // 속도가 0.1 이상일 경우 이동 중으로 간주
    }

    private void PlayFootstepSound()
    {
        if (footstepSound != null && audioSource != null) {
            audioSource.clip = footstepSound;
            audioSource.time = 0.0f; // 재생 시작 시간 초기화
            audioSource.Play();
            footstepStartTime = Time.time; // 시작 시간 기록
            isPlayingFootstep = true; // 재생 상태 설정
        }
    }

    private void StopFootstepSound()
    {
        // 소리가 끝날 때까지 기다리고, 자연스럽게 종료되도록 처리
        if (audioSource.isPlaying) {
            // 소리가 끝날 때까지 기다리기 전에 직접 Stop하지 않음
            audioSource.Stop(); // 재생 중지
            isPlayingFootstep = false; // 재생 상태 해제
        }
    }
}
