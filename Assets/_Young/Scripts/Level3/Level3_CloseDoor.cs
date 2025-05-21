using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Level3_CloseDoor : UdonSharpBehaviour
{
    [Header("문과 트리거")]
    [SerializeField] private GameObject door1;
    [SerializeField] private GameObject door2;
    [SerializeField] private GameObject door3;

    [Header("사운드 설정")]
    [SerializeField] private AudioSource doorCloseSound;

    // 문의 시작 각도와 목표 각도 설정
    private readonly Vector3 door1StartRotation = new Vector3(0f, 133f, 0f);
    private readonly Vector3 door2StartRotation = new Vector3(0f, 133f, 0f);
    private readonly Vector3 door3StartRotation = new Vector3(0f, -47f, 0f);

    private readonly Vector3 door1TargetRotation = new Vector3(0f, 0f, 0f);
    private readonly Vector3 door2TargetRotation = new Vector3(0f, 0f, 0f);
    private readonly Vector3 door3TargetRotation = new Vector3(0f, -180f, 0f);

    // 문이 닫히는 시간 (초)
    private readonly float closeDuration = 0.1f;

    // 각 문의 닫힘 상태를 추적
    private bool isDoor1Closing = false;
    private bool isDoor2Closing = false;
    private bool isDoor3Closing = false;

    // 트리거 활성화 상태 추적
    private bool door1Triggered = false;
    private bool door2Triggered = false;
    private bool door3Triggered = false;

    // 사운드 재생 상태 추적
    private bool hasDoor1PlayedSound = false;
    private bool hasDoor2PlayedSound = false;
    private bool hasDoor3PlayedSound = false;

    // 경과 시간을 추적하기 위한 변수
    private float door1Timer = 0f;
    private float door2Timer = 0f;
    private float door3Timer = 0f;

    // 문이 열리는 상태를 추적
    private bool isDoor1Opening = false;
    private bool isDoor2Opening = false;
    private bool isDoor3Opening = false;

    private void Start()
    {
        // 시작할 때 각 문의 초기 각도 설정
        door1.transform.rotation = Quaternion.Euler(door1StartRotation);
        door2.transform.rotation = Quaternion.Euler(door2StartRotation);
        door3.transform.rotation = Quaternion.Euler(door3StartRotation);
    }

    public void TriggerDoor1()
    {
        if (!door1Triggered && !isDoor1Closing)
        {
            isDoor1Closing = true;
            door1Timer = 0f;
            hasDoor1PlayedSound = false;
            door1Triggered = true;
        }
    }

    public void TriggerDoor2()
    {
        if (!door2Triggered && !isDoor2Closing)
        {
            isDoor2Closing = true;
            door2Timer = 0f;
            hasDoor2PlayedSound = false;
            door2Triggered = true;
        }
    }

    public void TriggerDoor3()
    {
        if (!door3Triggered && !isDoor3Closing)
        {
            isDoor3Closing = true;
            door3Timer = 0f;
            hasDoor3PlayedSound = false;
            door3Triggered = true;
        }
    }

    public void OpenAllDoors()
    {
        // 모든 문의 트리거 상태 리셋
        door1Triggered = false;
        door2Triggered = false;
        door3Triggered = false;

        // 문들을 시작 위치로 즉시 리셋
        door1.transform.rotation = Quaternion.Euler(door1StartRotation);
        door2.transform.rotation = Quaternion.Euler(door2StartRotation);
        door3.transform.rotation = Quaternion.Euler(door3StartRotation);

        // 닫힘 상태도 리셋
        isDoor1Closing = false;
        isDoor2Closing = false;
        isDoor3Closing = false;
    }

    public void OpenDoor1()
    {
        if (!isDoor1Opening)
        {
            isDoor1Opening = true;
            door1Timer = 0f;
            door1Triggered = false;  // 트리거 상태 리셋
        }
    }

    public void OpenDoor2()
    {
        if (!isDoor2Opening)
        {
            isDoor2Opening = true;
            door2Timer = 0f;
            door2Triggered = false;  // 트리거 상태 리셋
        }
    }

    public void OpenDoor3()
    {
        if (!isDoor3Opening)
        {
            isDoor3Opening = true;
            door3Timer = 0f;
            door3Triggered = false;  // 트리거 상태 리셋
        }
    }

    private void Update()
    {
        // 문1 닫기 애니메이션
        if (isDoor1Closing)
        {
            door1Timer += Time.deltaTime;
            float t = door1Timer / closeDuration;
            if (t <= 1.0f)
            {
                Vector3 currentRotation = new Vector3(
                    Mathf.Lerp(door1StartRotation.x, door1TargetRotation.x, t),
                    Mathf.Lerp(door1StartRotation.y, door1TargetRotation.y, t),
                    Mathf.Lerp(door1StartRotation.z, door1TargetRotation.z, t)
                );
                door1.transform.rotation = Quaternion.Euler(currentRotation);
                
                if (!hasDoor1PlayedSound)
                {
                    doorCloseSound.Play();
                    hasDoor1PlayedSound = true;
                }
            }
            else
            {
                door1.transform.rotation = Quaternion.Euler(door1TargetRotation);
                isDoor1Closing = false;
            }
        }

        // 문2 닫기 애니메이션
        if (isDoor2Closing)
        {
            door2Timer += Time.deltaTime;
            float t = door2Timer / closeDuration;
            if (t <= 1.0f)
            {
                Vector3 currentRotation = new Vector3(
                    Mathf.Lerp(door2StartRotation.x, door2TargetRotation.x, t),
                    Mathf.Lerp(door2StartRotation.y, door2TargetRotation.y, t),
                    Mathf.Lerp(door2StartRotation.z, door2TargetRotation.z, t)
                );
                door2.transform.rotation = Quaternion.Euler(currentRotation);
                
                if (!hasDoor2PlayedSound)
                {
                    doorCloseSound.Play();
                    hasDoor2PlayedSound = true;
                }
            }
            else
            {
                door2.transform.rotation = Quaternion.Euler(door2TargetRotation);
                isDoor2Closing = false;
            }
        }

        // 문3 닫기 애니메이션
        if (isDoor3Closing)
        {
            door3Timer += Time.deltaTime;
            float t = door3Timer / closeDuration;
            if (t <= 1.0f)
            {
                Vector3 currentRotation = new Vector3(
                    Mathf.Lerp(door3StartRotation.x, door3TargetRotation.x, t),
                    Mathf.Lerp(door3StartRotation.y, door3TargetRotation.y, t),
                    Mathf.Lerp(door3StartRotation.z, door3TargetRotation.z, t)
                );
                door3.transform.rotation = Quaternion.Euler(currentRotation);
                
                if (!hasDoor3PlayedSound)
                {
                    doorCloseSound.Play();
                    hasDoor3PlayedSound = true;
                }
            }
            else
            {
                door3.transform.rotation = Quaternion.Euler(door3TargetRotation);
                isDoor3Closing = false;
            }
        }

        // 문1 열기 애니메이션
        if (isDoor1Opening)
        {
            door1Timer += Time.deltaTime;
            float t = door1Timer / closeDuration;
            if (t <= 1.0f)
            {
                Vector3 currentRotation = new Vector3(
                    Mathf.Lerp(door1TargetRotation.x, door1StartRotation.x, t),
                    Mathf.Lerp(door1TargetRotation.y, door1StartRotation.y, t),
                    Mathf.Lerp(door1TargetRotation.z, door1StartRotation.z, t)
                );
                door1.transform.rotation = Quaternion.Euler(currentRotation);
            }
            else
            {
                door1.transform.rotation = Quaternion.Euler(door1StartRotation);
                isDoor1Opening = false;
            }
        }

        // 문2, 문3도 동일한 패턴으로 열기 애니메이션 추가
        // ... (문2, 문3의 열기 애니메이션 코드)
    }
}