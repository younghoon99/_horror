using UdonSharp;
using UnityEngine;

public class LightController : UdonSharpBehaviour
{
    [Header("조명-메시 설정")]
    public GameObject[] lightObjects;      // 조명 오브젝트 배열
    public GameObject[] meshObjects;       // 메시 오브젝트 배열

    [Header("머티리얼 설정")]
    public Material emissionOnMaterial;    // 발광 켜진 상태의 머티리얼
    public Material emissionOffMaterial;   // 발광 꺼진 상태의 머티리얼
    
    private bool isBlinking = true;        // 깜빡임 여부
    private bool isCutsceneMode = false;   // 컷신 모드 여부
    private float[] blinkTimers;           // 각 쌍의 타이머
    private float[] blinkDurations;        // 각 쌍의 깜빡이는 주기
    private bool[] isLightOn;              // 각 쌍의 상태

    void Start()
    {
        // lightObjects와 meshObjects는 같은 길이여야 함
        int pairCount = lightObjects.Length;
        blinkTimers = new float[pairCount];
        blinkDurations = new float[pairCount];
        isLightOn = new bool[pairCount];

        // 각 쌍에 대한 초기 랜덤 타이밍 설정
        for (int i = 0; i < pairCount; i++)
        {
            blinkDurations[i] = Random.Range(1f, 5f);
            isLightOn[i] = false;
            // 초기 상태 설정
            UpdateLightMeshPair(i, false);
        }
    }

    void Update()
    {
        // 컷신 모드일 때는 일반 깜빡임 로직 실행하지 않음
        if (!isBlinking || isCutsceneMode) return;

        for (int i = 0; i < lightObjects.Length; i++)
        {
            blinkTimers[i] += Time.deltaTime;

            if (blinkTimers[i] >= blinkDurations[i])
            {
                blinkTimers[i] = 0f;
                blinkDurations[i] = Random.Range(1f, 5f);
                isLightOn[i] = !isLightOn[i];
                UpdateLightMeshPair(i, isLightOn[i]);
            }
        }
    }

    // 조명과 메시를 동시에 업데이트하는 메서드
    private void UpdateLightMeshPair(int index, bool state)
    {
        if (index >= lightObjects.Length) return;

        // 조명 업데이트
        if (lightObjects[index] != null)
        {
            Light light = lightObjects[index].GetComponent<Light>();
            if (light != null)
            {
                light.enabled = state;
            }
        }

        // 메시 업데이트
        if (meshObjects[index] != null)
        {
            MeshRenderer renderer = meshObjects[index].GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = state ? emissionOnMaterial : emissionOffMaterial;
            }
        }
    }

    // 컷신 모드 시작
    public void StartCutsceneMode()
    {
        isCutsceneMode = true;
        // 모든 조명 끄기
        TurnOffAllLights();
    }

    // 컷신 모드 종료
    public void EndCutsceneMode()
    {
        isCutsceneMode = false;
        // 일반 깜빡임 모드로 복귀
        ResetAllLights();
    }

    // 특정 조명만 켜기
    public void TurnOnSpecificLight(int index)
    {
        if (index >= 0 && index < lightObjects.Length)
        {
            UpdateLightMeshPair(index, true);
        }
    }
    
    // 특정 조명만 끄기
    public void TurnOffSpecificLight(int index)
    {
        if (index >= 0 && index < lightObjects.Length)
        {
            UpdateLightMeshPair(index, false);
        }
    }
    
    // 모든 조명 끄기
    public void TurnOffAllLights()
    {
        for (int i = 0; i < lightObjects.Length; i++)
        {
            UpdateLightMeshPair(i, false);
        }
    }
    
    // 모든 조명 초기 상태로 리셋
    public void ResetAllLights()
    {
        isBlinking = true;
        for (int i = 0; i < lightObjects.Length; i++)
        {
            blinkTimers[i] = 0f;
            blinkDurations[i] = Random.Range(1f, 5f);
            isLightOn[i] = false;
            UpdateLightMeshPair(i, false);
        }
    }

    public void StopBlinking()
    {
        isBlinking = false;
        
        // 모든 쌍의 조명과 메시를 끄기
        for (int i = 0; i < lightObjects.Length; i++)
        {
            UpdateLightMeshPair(i, false);
        }
    }
}