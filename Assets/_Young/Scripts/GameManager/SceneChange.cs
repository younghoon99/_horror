using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SceneChange : UdonSharpBehaviour
{
    [Header("��털 설정")]
    [SerializeField] private string worldID = "wrld_xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";  // 목적지 월드 ID
    [SerializeField] private GameObject portalPrefab;  // VRChat 포털 프리팹
    private GameObject spawnedPortal;  // 생성된 포털

    public GameObject potalLocation;

    public override void Interact()
    {
        if (spawnedPortal != null)
        {
            // 이미 포털이 존재하면 제거
            Destroy(spawnedPortal);
            spawnedPortal = null;
            return;
        }

        // 포털 생성
        if (portalPrefab != null)
        {
            // 포털 생성 및 위치 설정
            spawnedPortal = VRCInstantiate(portalPrefab);
            spawnedPortal.transform.position = potalLocation.transform.position;
            spawnedPortal.transform.rotation = potalLocation.transform.rotation;

            // 포털 컴포넌트 가져오기
            VRC_PortalMarker portal = spawnedPortal.GetComponent<VRC_PortalMarker>();
            if (portal != null)
            {
                // 포털 속성 설정
                portal.roomId = worldID;
            }
        }
    }
}
