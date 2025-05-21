using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DoorController : UdonSharpBehaviour
{
    public GameObject door; // 작동할 문 오브젝트
    public AudioClip doorCloseSound; // 문이 닫힐 때 재생할 사운드
    public float doorCloseTime = 2f; // 문이 닫히는 시간 (초)
    public float closedRotationY = 0f; // 문이 닫힌 상태의 y 축 회전값
    public float openRotationY = 133f; // 문이 열린 상태의 y 축 회전값

    public bool isDoorClosed = false; // 문이 닫혔는지 여부
    private AudioSource audioSource; // AudioSource 컴포넌트

    void Start()
    {
        // 문 오브젝트에서 AudioSource 가져오기
        if (door != null) {
            audioSource = door.GetComponent<AudioSource>();
            if (audioSource == null) {
                Debug.LogError("문 오브젝트에 AudioSource 컴포넌트가 없습니다. 추가해주세요.");
            }
        }
        else {
            Debug.LogError("Door 변수에 문 오브젝트가 연결되지 않았습니다.");
        }
    }

    void Update()
    {
        // 문이 닫히는 애니메이션 처리 (y축 회전값 변경)
        if (isDoorClosed && door != null) {
            Vector3 currentRotation = door.transform.eulerAngles;
            currentRotation.y = Mathf.Lerp(currentRotation.y, closedRotationY, Time.deltaTime / doorCloseTime);
            door.transform.eulerAngles = currentRotation;
        }
    }

    // 버튼을 눌렀을 때 실행되는 메서드
    public override void Interact()
    {
        if (door != null && !isDoorClosed) // 문이 닫히지 않았을 때만 동작
        {
            isDoorClosed = true;
            PlayDoorCloseSound();
        }
    }

    // 문이 닫힐 때 소리 재생
    private void PlayDoorCloseSound()
    {
        // AudioSource가 있고 사운드가 설정되어 있으면 재생
        if (audioSource != null && doorCloseSound != null) {
            audioSource.clip = doorCloseSound;
            audioSource.Play();
        }
    }
}
