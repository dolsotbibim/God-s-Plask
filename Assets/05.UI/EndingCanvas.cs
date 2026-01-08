using TMPro;
using UnityEngine;
using DG.Tweening;
public class EndingCanvas : MonoBehaviour
{
    public TextMeshProUGUI text;
    public RectTransform rectTransform;
    public int SpawnedObjectCount = 0;
    public int MergedCount = 0;
    public int DecomposedCount = 0;

    public int FailCount = 0;
    public int SuccessCount = 0;
    public int CosmicOreCount = 0;

    private void OnEnable()
    {
        for (int i = 0; i < Spawner.Instance.objs.Count; i++)
        {
            Spawner.Instance.objs[i].gameObject.SetActive(false);
        }
        SoundManager.Instance.BGMAudioSource.clip = SoundManager.Instance.BGM[1];
        SoundManager.Instance.BGMAudioSource.Play();
        rectTransform.DOAnchorPos(new Vector2(0, 1585), 60);
        float playTime = UIManager.Instance.timer;
        int hours = Mathf.FloorToInt(playTime / 3600f);
        int minutes = Mathf.FloorToInt((playTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);
        SpawnedObjectCount = DataManager.GetIntData("SpawnedObjectCount", 0);
        MergedCount = DataManager.GetIntData("MergedObjectCount", 0);
        FailCount = DataManager.GetIntData("FailCount", 0);
        SuccessCount = DataManager.GetIntData("SuccessCount", 0);
        CosmicOreCount = DataManager.GetIntData("CosmicOreCount", 0);

        text.text = string.Format("Play time: {0:00}:{1:00}:{2:00}", hours, minutes, seconds) + "\n\n\n" +
            "Spawnd Objects: " + SpawnedObjectCount + "\n\n" +
            "Merged Objects: " + MergedCount + "\n\n\n" +
            "Spawn Success: " + SuccessCount + "\n\n" +
            "Spawn Fail: " + FailCount + "\n\n" +
            "Collected Cosmic Ore: " + CosmicOreCount + "\n\n\n\n\n" +
            "Thank You For Playing!";

    }
}
