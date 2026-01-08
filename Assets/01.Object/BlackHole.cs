using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class BlackHole : MonoBehaviour
{
    public Transform targetObject;
    public Camera targetCamera;

    public Transform target;
    public List<Transform> first = new List<Transform>();
    public Transform second;
    public RectTransform[] uiElements;
    public static BlackHole instance;
    private float growthStep = 1f; // 흡수 시마다 커질 양
    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        // 기존 리스트 초기화
        first.Clear();
        CameraLookController.instance.isShaking = true;
        SoundManager.Instance.PlaySFX(18, 0.8f, false);
        SoundManager.Instance.PlaySFX(19, 0.3f, false);
        Spawner.Instance.SpawnPanel.StopAllCoroutines();
        Object[] rbs = FindObjectsByType<Object>(FindObjectsSortMode.None);
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].PreventingChange = true;
            rbs[i].GetComponent<Rigidbody>().isKinematic = true;
            first.Add(rbs[i].transform);
        }
        
        PlaySequence();
    }

    public void PlaySequence()
    {
        Sequence mainSeq = DOTween.Sequence();
        mainSeq.AppendInterval(1f);
        mainSeq.AppendCallback(() => StartPersistentShake(0.1f, 0.1f));
        mainSeq.AppendInterval(1f);

        // 1단계: 약한 흔들림
        foreach (var obj in first)
        {
            mainSeq.Join(obj.DOMove(target.position, 1f).SetEase(Ease.InQuad));
            mainSeq.Join(obj.DOScale(Vector3.zero, 1f).OnComplete(() => GrowTarget(0.25f)));
        }

        mainSeq.AppendInterval(0.5f);

        // 2단계: 중간 흔들림
        mainSeq.AppendCallback(() => StartPersistentShake(0.3f, 0.3f));
        mainSeq.Append(second.DOMove(target.position, 1f).SetEase(Ease.InQuad));
        mainSeq.Join(second.DOScale(Vector3.zero, 1f).OnComplete(() => GrowTarget(0.5f)));

        mainSeq.AppendInterval(0.3f);

        // 3단계: 강한 흔들림 + UI 동시 시작/지속시간 차이
        mainSeq.AppendCallback(() => StartPersistentShake(0.5f, 0.6f));
        float startTime = mainSeq.Duration();
        for (int i = 0; i < uiElements.Length; i++)
        {
            float duration = 0.5f + (i * 0.2f);

            mainSeq.Insert(startTime,
                uiElements[i].DOMove(target.position, duration).SetEase(Ease.InQuad));

            mainSeq.Insert(startTime,
                uiElements[i].DOScale(Vector3.zero, duration).SetEase(Ease.InQuad)
                .OnComplete(() => GrowTarget(0.5f)));
        }

        // 마지막: 카메라 Orthographic Size 축소 연출

        // 모든 연출 종료 후 흔들림 정지
        mainSeq.OnComplete(() =>
        {
            StopShaking();
            Camera.main.transform.localPosition = new Vector3(0.1781f, 0, -21.5f);
            target.transform.localScale = Vector3.one * 13;
            EndingCanvas.gameObject.SetActive(true);
        });
    }
    public Canvas EndingCanvas;
    private void GrowTarget(float value)
    {
        // 타겟 오브젝트의 스케일을 조금씩 키움
        target.DOScale(target.localScale + new Vector3(growthStep, growthStep, growthStep) * value, 0.1f);
    }

    private Tween currentObjShake;
    private Tween currentCamShake;

    private void StartPersistentShake(float objStrength, float camStrength)
    {
        currentObjShake.Kill(true);
        currentCamShake.Kill(true);

        currentObjShake = target.DOShakePosition(0.5f, objStrength).SetLoops(-1);
        currentCamShake = targetCamera.transform.DOShakePosition(0.5f, camStrength).SetLoops(-1);
    }

    private void StopShaking()
    {
        currentObjShake.Kill(true);
        currentCamShake.Kill(true);
    }
}