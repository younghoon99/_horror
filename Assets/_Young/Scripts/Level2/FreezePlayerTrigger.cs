using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;  // UI 컴포넌트 사용을 위해 추가

public class FreezePlayerTrigger : UdonSharpBehaviour
{
    [Header("사운드 설정")]
    public AudioSource firstFreezeSound;     // 플레이어가 처음 고정될 때 재생될 사운드
    public AudioSource secondFreezeSound;    // 일정 시간 후 재생될 두 번째 사운드
    public WorldSoundController worldSoundController;  // 기존 월드 사운드 제어용 컨트롤러

    [Header("오브젝트 설정")]
    public GameObject mirrorObject;          // 플레이어 고정 시 활성화될 거울 오브젝트
    public GameObject buttonToActivate;      // 시퀀스 종료 후 활성화될 버튼

    [Header("UI 설정")]
    public Canvas messageCanvas;             // 메시지를 표시할 캔버스
    public Text messageText;                 // 표시할 텍스트 컴포넌트
    public float textDisplayDuration = 3f;   // 텍스트 표시 지속 시간
    public float fadeInDuration = 0.5f;      // 텍스트 페이드 인 시간
    public float fadeOutDuration = 0.5f;     // 텍스트 페이드 아웃 시간

    [Header("시간 설정")]
    public float freezeDuration = 10f;       // 플레이어가 고정될 총 시간
    public float secondSoundDelay = 5f;      // 두 번째 사운드가 재생될 때까지의 지연 시간
    public float rotationDuration = 2f;      // 180도 회전에 걸리는 시간

    // 내부 상태 변수들
    private bool isPlayerFrozen = false;     // 플레이어가 현재 고정된 상태인지
    private float freezeTimer = 0f;          // 고정 상태 경과 시간
    private Vector3 frozenPosition;          // 플레이어가 고정될 위치
    private bool hasTriggered = false;       // 이 트리거가 이미 발동되었는지
    private bool isSecondSoundPlayed = false;// 두 번째 사운드가 재생되었는지
    private bool isRotating = false;         // 현재 회전 중인지
    private float rotationTimer = 0f;        // 회전 진행 시간
    private Quaternion startRotation;        // 회전 시작 각도
    private Quaternion targetRotation;       // 목표 회전 각도

    // 텍스트 관련 변수들
    private bool isShowingText = false;      // 텍스트가 현재 표시 중인지
    private float textTimer = 0f;            // 텍스트 표시 타이머

    void Start()
    {
        // 시작 시 필요한 오브젝트들 비활성화
        if (mirrorObject != null) mirrorObject.SetActive(false);
        if (buttonToActivate != null) buttonToActivate.SetActive(false);
        if (messageCanvas != null) 
        {
            messageCanvas.gameObject.SetActive(false);
        }
        if (messageText != null)
        {
            Color textColor = messageText.color;
            textColor.a = 0f;
            messageText.color = textColor;
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player == Networking.LocalPlayer && !isPlayerFrozen && !hasTriggered)
        {
            hasTriggered = true;
            FreezePlayer();
        }
    }

    void Update()
    {
        if (isPlayerFrozen)
        {
            VRCPlayerApi localPlayer = Networking.LocalPlayer;
            if (localPlayer != null)
            {
                if (isRotating)
                {
                    rotationTimer += Time.deltaTime;
                    float rotationProgress = rotationTimer / rotationDuration;
                    
                    if (rotationProgress <= 1f)
                    {
                        Quaternion currentRotation = Quaternion.Lerp(startRotation, targetRotation, rotationProgress);
                        localPlayer.TeleportTo(frozenPosition, currentRotation);
                    }
                    else
                    {
                        isRotating = false;
                        localPlayer.TeleportTo(frozenPosition, targetRotation);
                    }
                }
                else
                {
                    localPlayer.TeleportTo(frozenPosition, localPlayer.GetRotation());
                }
            }

            freezeTimer += Time.deltaTime;

            if (!isSecondSoundPlayed && freezeTimer >= secondSoundDelay)
            {
                if (secondFreezeSound != null)
                {
                    secondFreezeSound.Play();
                }
                isSecondSoundPlayed = true;
            }

            if (freezeTimer >= freezeDuration)
            {
                UnfreezePlayer();
            }
        }

        // 텍스트 페이드 효과 처리
        if (isShowingText)
        {
            textTimer += Time.deltaTime;
            
            if (messageText != null)
            {
                float alpha = 0f;
                
                // 페이드 인
                if (textTimer <= fadeInDuration)
                {
                    alpha = textTimer / fadeInDuration;
                }
                // 완전히 보이는 구간
                else if (textTimer <= textDisplayDuration - fadeOutDuration)
                {
                    alpha = 1f;
                }
                // 페이드 아웃
                else if (textTimer <= textDisplayDuration)
                {
                    alpha = 1f - ((textTimer - (textDisplayDuration - fadeOutDuration)) / fadeOutDuration);
                }
                
                // 알파값 적용
                Color textColor = messageText.color;
                textColor.a = Mathf.Clamp01(alpha);
                messageText.color = textColor;
                
                // 텍스트 표시 시간이 끝나면 캔버스 비활성화
                if (textTimer >= textDisplayDuration)
                {
                    messageCanvas.gameObject.SetActive(false);
                    isShowingText = false;
                }
            }
        }
    }

    private void FreezePlayer()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer == null) return;

        frozenPosition = localPlayer.GetPosition();
        startRotation = localPlayer.GetRotation();
        targetRotation = startRotation * Quaternion.Euler(0, 180f, 0);

        isRotating = true;
        rotationTimer = 0f;
        isPlayerFrozen = true;
        freezeTimer = 0f;
        isSecondSoundPlayed = false;

        if (worldSoundController != null) worldSoundController.StopWorldSound();
        if (firstFreezeSound != null) firstFreezeSound.Play();
        if (mirrorObject != null) mirrorObject.SetActive(true);
    }

    private void UnfreezePlayer()
    {
        isPlayerFrozen = false;
        isRotating = false;

        // 사운드 정리
        if (firstFreezeSound != null) firstFreezeSound.Stop();
        if (secondFreezeSound != null) secondFreezeSound.Stop();
        if (worldSoundController != null) worldSoundController.StartWorldSound();

        // 오브젝트 상태 변경
        if (mirrorObject != null) mirrorObject.SetActive(false);
        if (buttonToActivate != null) buttonToActivate.SetActive(true);

        // 텍스트 표시 시작
        ShowMessage();
    }

    // 메시지 표시 시작
    private void ShowMessage()
    {
        if (messageCanvas != null && messageText != null)
        {
            messageCanvas.gameObject.SetActive(true);
            Color textColor = messageText.color;
            textColor.a = 0f;
            messageText.color = textColor;
            
            isShowingText = true;
            textTimer = 0f;
        }
    }
}