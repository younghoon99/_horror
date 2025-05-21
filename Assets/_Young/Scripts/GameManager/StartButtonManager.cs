using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StartButtonManager : UdonSharpBehaviour
{
    // 인스펙터에서 할당할 변수들
    [Header("버튼 설정")]
    public StartButton[] startButtons;        // 게임에서 사용할 모든 시작 버튼들의 배열
    
    [Header("텔레포트 위치 설정")]
    public Transform[] targetPositions;       // 각 버튼에 대응되는 텔레포트 위치들
    
    [Header("사운드 설정")]
    public WorldSoundController worldSoundController;    // 게임 사운드를 제어하는 컨트롤러

    // 각 버튼이 클릭되었을 때 호출되는 메서드
    // buttonIndex: 클릭된 버튼의 인덱스 (0부터 시작)
    public void HandleButtonClick(int buttonIndex)
    {
        // 로컬 플레이어 정보 가져오기
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        
        // 유효성 검사: 플레이어가 없거나 잘못된 인덱스인 경우 실행 중단
        if (localPlayer == null || buttonIndex >= targetPositions.Length) return;

        // 해당 버튼에 연결된 위치로 플레이어 텔레포트
        localPlayer.TeleportTo(
            targetPositions[buttonIndex].position,   // 목적지 위치
            targetPositions[buttonIndex].rotation    // 목적지에서의 회전값
        );
        
        // 텔레포트 후 사운드 재생
        worldSoundController.StartWorldSound();
    }
}