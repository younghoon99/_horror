using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class JumpscareController : UdonSharpBehaviour
{
    [Header("UI 설정")]
    public Canvas jumpscareCanvas;
    public Image jumpscareImage;
    public float imageDuration = 1f;     // 이미지 표시 총 시간
    public float fadeInDuration = 0.1f;  // 페이드 인에 걸리는 시간 (더 빠르게 수정)
    public float fadeOutDuration = 0.2f; // 페이드 아웃에 걸리는 시간

    [Header("사운드 설정")]
    public AudioSource jumpscareSound;
    public float soundDuration = 5f;     // 사운드 재생 시간

    [Header("트리거 조건")]
    public GameObject triggerButton;     // FreezePlayerTrigger에서 활성화되는 버튼

    private bool isJumpscareActive = false;
    private float timer = 0f;
    private bool hasTriggered = false;

    void Start()
    {
        // 시작 시 초기화
        if (jumpscareCanvas != null)
        {
            jumpscareCanvas.gameObject.SetActive(false);
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player == Networking.LocalPlayer && !hasTriggered && triggerButton != null && triggerButton.activeSelf)
        {
            TriggerJumpscare();
            hasTriggered = true;
        }
    }

    void Update()
    {
        if (isJumpscareActive)
        {
            timer += Time.deltaTime;
            
            if (jumpscareImage != null)
            {
                float alpha = 0f;
                
                // 페이드 인 (빠르게)
                if (timer <= fadeInDuration)
                {
                    alpha = timer / fadeInDuration;
                }
                // 완전히 보이는 구간
                else if (timer <= imageDuration - fadeOutDuration)
                {
                    alpha = 1f;
                }
                // 페이드 아웃
                else if (timer <= imageDuration)
                {
                    float fadeOutProgress = (timer - (imageDuration - fadeOutDuration)) / fadeOutDuration;
                    alpha = 1f - fadeOutProgress;
                }
                
                // 알파값 적용
                Color imageColor = jumpscareImage.color;
                imageColor.a = Mathf.Clamp01(alpha);
                jumpscareImage.color = imageColor;
                
                // 이미지 시간이 끝나면 캔버스 비활성화
                if (timer >= imageDuration)
                {
                    jumpscareCanvas.gameObject.SetActive(false);
                }
            }

            // 사운드 지속 시간이 지나면 종료
            if (timer >= soundDuration)
            {
                EndJumpscare();
            }
        }
    }

    private void TriggerJumpscare()
    {
        Debug.Log("점프스케어 트리거 발동"); // 디버그용

        isJumpscareActive = true;
        timer = 0f;

        // 캔버스 활성화 및 이미지 초기화
        if (jumpscareCanvas != null)
        {
            jumpscareCanvas.gameObject.SetActive(true);
            
            if (jumpscareImage != null)
            {
                // 시작 시 완전히 투명하게 설정
                Color imageColor = jumpscareImage.color;
                imageColor.a = 0f;
                jumpscareImage.color = imageColor;
            }
        }

        // 사운드 재생
        if (jumpscareSound != null)
        {
            jumpscareSound.Play();
        }
    }

    private void EndJumpscare()
    {
        isJumpscareActive = false;

        // 캔버스 비활성화
        if (jumpscareCanvas != null)
        {
            jumpscareCanvas.gameObject.SetActive(false);
        }

        // 사운드 정지
        if (jumpscareSound != null && jumpscareSound.isPlaying)
        {
            jumpscareSound.Stop();
        }
    }
}