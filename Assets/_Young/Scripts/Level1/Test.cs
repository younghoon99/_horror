using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Test : UdonSharpBehaviour
{
    public DoorController doorController; // DoorController 참조 추가

    private void Start()
    {
        Collider collider = gameObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player == Networking.LocalPlayer)
        {
            // 트리거에 들어왔을 때 문 닫기 실행
            if (doorController != null && !doorController.isDoorClosed)
            {
                doorController.Interact();
                Debug.Log("트리거에 의해 문이 닫힙니다!");
            }
        }
    }

    
}