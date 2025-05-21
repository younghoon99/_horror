using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class CutsceneController : UdonSharpBehaviour
{
    //적용 잘됐나
    [Header("컷신 기본 설정")]
    public Camera cutsceneCamera;           // 컷신용 카메라
    public float cinematicDuration = 20f;   // 전체 컷신 지속 시간
    
    [Header("컷신 이벤트 컨트롤러")]
    public LightController lightController;  // 조명 컨트롤러
    public AudioSource sound1;              // 2-4초 구간용 사운드
    public AudioSource sound2;              // 4-6초 구간용 사운드
    public AudioSource sound3;              // 6-8초 구간용 사운드
    public AudioSource clapSound;           // 네 번째 귀신 박수 소리 추가
    public AudioSource finalSound;          // 마지막 귀신 등장 시 사운드
    public GameObject[] ghostObjects;        // 귀신 오브젝트들
    
    [Header("컷신용 조명 설정")]
    public int[] cutsceneLightIndexes;      // 컷신에서 사용할 조명 인덱스들
    public Light finalPointLight;           // 마지막에 비출 PointLight
    
    [Header("문 컨트롤러")]
    public Level3_CloseDoor doorController;  // 문 컨트롤러 참조 추가
    public DoorTrigger[] doorTriggers;  // DoorTrigger 배열 추가
    
    private bool isPlaying = false;
    private float timer = 0f;
    private VRCPlayerApi localPlayer;
    private int currentPhase = 0;           // 현재 페이즈
    private float nextEventTime = 0f;       // 다음 이벤트 시간
    private bool isFlickering = false;      // 깜빡임 상태
    private float flickerTimer = 0f;        // 깜빡임 타이머
    private Animator[] ghostAnimators;  // 귀신들의 Animator 컴포넌트 배열
    private bool hasTriggered = false;  // 트리거 발동 여부를 체크할 변수 추가

    void Start()
    {
        if(cutsceneCamera != null)
            cutsceneCamera.enabled = false;
        
        if(finalPointLight != null)
        {
            finalPointLight.gameObject.SetActive(false);
            finalPointLight.enabled = false;
        }
        
        if(ghostObjects == null)
        {
            Debug.LogError("[CutsceneController] Ghost Objects array is not assigned!");
        }
        else
        {
            for(int i = 0; i < ghostObjects.Length; i++)
            {
                if(ghostObjects[i] == null)
                {
                    Debug.LogWarning($"[CutsceneController] Ghost Object at index {i} is null!");
                }
            }
        }
            
        localPlayer = Networking.LocalPlayer;
        
        // 귀신 애니메이터 초기화
        if(ghostObjects != null)
        {
            ghostAnimators = new Animator[ghostObjects.Length];
            for(int i = 0; i < ghostObjects.Length; i++)
            {
                if(ghostObjects[i] != null)
                {
                    ghostAnimators[i] = ghostObjects[i].GetComponent<Animator>();
                }
            }
        }
    }
    
    void Update()
    {
        if (!isPlaying) return;
        
        timer += Time.deltaTime;

        switch (currentPhase)
        {
            case 0: // 시작 - 3초 암전
                lightController.TurnOffAllLights();
                SetAllGhostsActive(false);
                if(finalPointLight != null)
                {
                    finalPointLight.gameObject.SetActive(false);
                    finalPointLight.enabled = false;
                }
                currentPhase++;
                nextEventTime = timer + 3f;  // 3초 암전
                break;

            case 1: // 첫 번째 귀신 (Run) - 3초 암전 후 시작
                if (timer < nextEventTime)  // 3초 암전이 끝나기 전
                {
                    break;  // 암전 시간 동안은 아무것도 하지 않음
                }
                
                if (timer >= nextEventTime + 2f)  // 귀신 등장 후 2초 지나면
                {
                    lightController.TurnOffSpecificLight(cutsceneLightIndexes[0]);
                    if(ghostObjects != null && ghostObjects.Length > 0)
                    {
                        ghostObjects[0].SetActive(false);
                    }
                    currentPhase++;
                    nextEventTime = timer + 2f;
                }
                else  // 암전 끝나고 귀신 등장
                {
                    lightController.TurnOnSpecificLight(cutsceneLightIndexes[0]);
                    if(ghostObjects != null && ghostObjects.Length > 0)
                    {
                        ghostObjects[0].SetActive(true);
                        if(ghostAnimators[0] != null)
                        {
                            ghostAnimators[0].SetTrigger("Run");
                        }
                    }
                    if(sound1 != null && !sound1.isPlaying) 
                    {
                        sound1.time = 0f;
                        sound1.Play();
                    }
                }
                break;

            case 2: // 두 번째 귀신 (Run)
                if (timer >= nextEventTime)
                {
                    lightController.TurnOffSpecificLight(cutsceneLightIndexes[1]);
                    if(ghostObjects != null && ghostObjects.Length > 1)
                    {
                        ghostObjects[1].SetActive(false);
                    }
                    currentPhase++;
                    nextEventTime = timer + 2f;
                }
                else if (timer >= nextEventTime - 2f)
                {
                    lightController.TurnOnSpecificLight(cutsceneLightIndexes[1]);
                    if(ghostObjects != null && ghostObjects.Length > 1)
                    {
                        ghostObjects[1].SetActive(true);
                        if(ghostAnimators[1] != null)
                        {
                            ghostAnimators[1].SetTrigger("Run");
                        }
                    }
                    if(sound2 != null && !sound2.isPlaying)
                    {
                        sound2.time = 0f;
                        sound2.Play();
                    }
                }
                break;

            case 3: // 세 번째 귀신 (Run)
                if (timer >= nextEventTime)
                {
                    lightController.TurnOffSpecificLight(cutsceneLightIndexes[2]);
                    if(ghostObjects != null && ghostObjects.Length > 2)
                    {
                        ghostObjects[2].SetActive(false);
                    }
                    currentPhase++;
                    nextEventTime = timer + 2f;
                }
                else if (timer >= nextEventTime - 2f)
                {
                    lightController.TurnOnSpecificLight(cutsceneLightIndexes[2]);
                    if(ghostObjects != null && ghostObjects.Length > 2)
                    {
                        ghostObjects[2].SetActive(true);
                        if(ghostAnimators[2] != null)
                        {
                            ghostAnimators[2].SetTrigger("Run");
                        }
                    }
                    if(sound3 != null && !sound3.isPlaying)
                    {
                        sound3.time = 0f;
                        sound3.Play();
                    }
                }
                break;

            case 4: // 네 번째 귀신 (Clap)
                if (timer >= nextEventTime)
                {
                    lightController.TurnOffSpecificLight(cutsceneLightIndexes[3]);
                    if(ghostObjects != null && ghostObjects.Length > 3)
                    {
                        ghostObjects[3].SetActive(false);
                    }
                    currentPhase++;
                    nextEventTime = timer + 2f;
                }
                else if (timer >= nextEventTime - 2f)
                {
                    lightController.TurnOnSpecificLight(cutsceneLightIndexes[3]);
                    if(ghostObjects != null && ghostObjects.Length > 3)
                    {
                        ghostObjects[3].SetActive(true);
                        if(ghostAnimators[3] != null)
                        {
                            ghostAnimators[3].SetTrigger("Clap");
                        }
                    }
                    if(clapSound != null && !clapSound.isPlaying) 
                    {
                        clapSound.time = 0f;
                        clapSound.Play();
                    }
                }
                break;

            case 5: // 다섯 번째 귀신 (Look)
                if (timer >= nextEventTime)
                {
                    lightController.TurnOffSpecificLight(cutsceneLightIndexes[4]);
                    if(ghostObjects != null && ghostObjects.Length > 4)
                    {
                        ghostObjects[4].SetActive(false);
                    }
                    currentPhase++;
                    nextEventTime = timer + 2f;
                    isFlickering = true;
                    flickerTimer = 0f;
                }
                else if (timer >= nextEventTime - 2f)
                {
                    lightController.TurnOnSpecificLight(cutsceneLightIndexes[4]);
                    if(ghostObjects != null && ghostObjects.Length > 4)
                    {
                        ghostObjects[4].SetActive(true);
                        if(ghostAnimators[4] != null)
                        {
                            ghostAnimators[4].SetTrigger("Look");
                        }
                    }
                }
                break;

            case 6: // 깜빡임 구간 (3초)
                if (timer >= nextEventTime)
                {
                    isFlickering = false;
                    lightController.TurnOffAllLights();
                    SetAllGhostsActive(false);
                    currentPhase++;
                    nextEventTime = timer + 3f;
                }
                else if (isFlickering)
                {
                    flickerTimer += Time.deltaTime;
                    if (flickerTimer >= 0.2f)  // 깜빡임 간격 더 빠르게
                    {
                        flickerTimer = 0f;
                        for (int i = 0; i < cutsceneLightIndexes.Length; i++)
                        {
                            if (Random.value > 0.5f)
                            {
                                lightController.TurnOnSpecificLight(cutsceneLightIndexes[i]);
                            }
                            else
                            {
                                lightController.TurnOffSpecificLight(cutsceneLightIndexes[i]);
                            }
                        }
                    }
                }
                break;

            case 7: // 마지막 귀신 (Scream)
                if (timer >= nextEventTime)
                {
                    lightController.TurnOffAllLights();
                    SetAllGhostsActive(false);
                    if(finalPointLight != null) 
                    {
                        finalPointLight.gameObject.SetActive(true);
                        finalPointLight.enabled = true;
                    }
                    if(ghostObjects != null && ghostObjects.Length > 5)
                    {
                        ghostObjects[5].SetActive(true);
                        if(ghostAnimators[5] != null)
                        {
                            ghostAnimators[5].SetTrigger("Scream");
                        }
                    }
                    if(finalSound != null)
                    {
                        finalSound.Play();
                    }
                    currentPhase++;
                    nextEventTime = timer + 3f;
                }
                break;

            case 8: // 마지막 이벤트 종료
                if (timer >= nextEventTime)
                {
                    if(finalPointLight != null)
                    {
                        finalPointLight.enabled = false;
                        finalPointLight.gameObject.SetActive(false);
                    }
                    SetAllGhostsActive(false);
                    if(finalSound != null)
                    {
                        finalSound.Stop();
                    }
                    currentPhase++;
                    nextEventTime = timer + 1f; // 1초 더 대기
                }
                break;

            case 9: // 최종 종료 (1초 대기 후)
                if (timer >= nextEventTime)
                {
                    EndCinematic();
                }
                break;
        }
    }
    
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player == Networking.LocalPlayer && !hasTriggered)  // 트리거되지 않은 경우에만 실행
        {
            hasTriggered = true;  // 트리거 상태 기록
            StartCinematic();
        }
    }
    
    public void StartCinematic()
    {
        if (isPlaying) return;
        
        // 모든 사운드 초기화
        if(sound1 != null) sound1.Stop();
        if(sound2 != null) sound2.Stop();
        if(sound3 != null) sound3.Stop();
        if(clapSound != null) clapSound.Stop();
        if(finalSound != null) finalSound.Stop();
        
        isPlaying = true;
        timer = 0f;
        currentPhase = 0;
        nextEventTime = 0f;
        isFlickering = false;
        flickerTimer = 0f;
        
        // 초기 상태 설정
        if(finalPointLight != null)
        {
            finalPointLight.gameObject.SetActive(false);
            finalPointLight.enabled = false;
        }
        if(finalSound != null)
        {
            finalSound.Stop();
        }
        SetAllGhostsActive(false);
        
        // 플레이어 이동 제한
        if(localPlayer != null)
        {
            localPlayer.Immobilize(true);
        }
        
        // 컷신 카메라 활성화
        if(cutsceneCamera != null)
        {
            cutsceneCamera.enabled = true;
        }

        // LightController를 컷신 모드로 전환
        if(lightController != null)
        {
            lightController.StartCutsceneMode();
        }
    }
    
    public void EndCinematic()
    {
        isPlaying = false;
        
        // 플레이어 이동 제한 해제
        if(localPlayer != null)
        {
            localPlayer.Immobilize(false);
        }
        
        // 컷신 카메라 비활성화
        if(cutsceneCamera != null)
        {
            cutsceneCamera.enabled = false;
        }
        
        // LightController를 일반 모드로 복귀
        if(lightController != null)
        {
            lightController.EndCutsceneMode();
        }
        
        // 모든 귀신 숨기기
        SetAllGhostsActive(false);
        
        // Point Light 끄기
        if(finalPointLight != null)
        {
            finalPointLight.enabled = false;
            finalPointLight.gameObject.SetActive(false);
        }

        // 마지막 사운드 정지
        if(finalSound != null)
        {
            finalSound.Stop();
        }

        // 모든 문 열기
        if(doorController != null)
        {
            doorController.OpenAllDoors();
        }
    }
    
    private void SetAllGhostsActive(bool active)
    {
        if (ghostObjects == null) return;
        
        for(int i = 0; i < ghostObjects.Length; i++)
        {
            if(ghostObjects[i] != null)
                ghostObjects[i].SetActive(active);
        }
    }
}