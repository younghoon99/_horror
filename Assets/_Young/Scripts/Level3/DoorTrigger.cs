using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DoorTrigger : UdonSharpBehaviour
{
    [SerializeField] private Level3_CloseDoor doorController;
    [SerializeField] private int doorNumber = 1;  // 1, 2, 또는 3
    
    private bool hasTriggered = false;  // 트리거 발동 여부를 체크할 변수

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!player.isLocal || hasTriggered) return;  // 이미 트리거된 경우 무시

        hasTriggered = true;  // 트리거 상태 기록
        
        switch(doorNumber)
        {
            case 1:
                doorController.TriggerDoor1();
                break;
            case 2:
                doorController.TriggerDoor2();
                break;
            case 3:
                doorController.TriggerDoor3();
                break;
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;  // 트리거 상태 리셋
    }
}