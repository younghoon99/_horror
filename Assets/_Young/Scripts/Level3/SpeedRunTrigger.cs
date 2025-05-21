using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SpeedRunTrigger : UdonSharpBehaviour
{
    [Header("속도 설정")]
    public float modifiedRunSpeed = 1.7f;      // 변경할 달리기 속도
    private float originalRunSpeed;           // 원래 달리기 속도 저장
    private bool isPlayerInTrigger = false;   // 플레이어가 트리거 안에 있는지

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player == Networking.LocalPlayer)
        {
            isPlayerInTrigger = true;
            // 원래 속도 저장
            originalRunSpeed = player.GetRunSpeed();
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player == Networking.LocalPlayer)
        {
            isPlayerInTrigger = false;
            // 원래 속도로 복구
            player.SetRunSpeed(originalRunSpeed);
        }
    }

    public void Update()
    {
        if (isPlayerInTrigger)
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            if (player != null)
            {
                // 트리거 영역 안에서 달리기 속도 변경
                player.SetRunSpeed(modifiedRunSpeed);
            }
        }
    }

    // 리스폰 시 리셋을 위한 메서드
    public void ResetTrigger()
    {
        if (isPlayerInTrigger)
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            if (player != null)
            {
                player.SetRunSpeed(originalRunSpeed);
            }
        }
        isPlayerInTrigger = false;
    }
}