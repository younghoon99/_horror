using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StartButton : UdonSharpBehaviour
{
    [Header("버튼 설정")]
    public int buttonIndex;                  // 이 버튼의 고유 인덱스
    public StartButtonManager buttonManager;  // 버튼 매니저 참조

    // VRChat에서 플레이어가 버튼을 클릭했을 때 호출
    public override void Interact()
    {
        // 버튼 매니저를 통해 클릭 이벤트 처리
        buttonManager.HandleButtonClick(buttonIndex);
    }
}