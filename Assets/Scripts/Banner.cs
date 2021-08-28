using UnityEngine;
using UnityEngine.UI;

public class Banner : MonoBehaviour
{
    public GameObject newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    [SerializeField]
    private Spawner _spawner;

    void Start()
    {

        _spawner.OnNewWave += OnNewWave;
    }

   


    void OnNewWave(int waveNumber)
    {
        newWaveBanner.SetActive(true);
        int sukaBol = _spawner.waves[waveNumber - 1].enemyCount;
        string[] Nums = {"One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten"};
        newWaveTitle.text = "-Wave " + Nums[waveNumber - 1] + "-";
        newWaveEnemyCount.text = "Enemies: " + sukaBol.ToString();
    }
}
