using Assets.Scripts;
using UnityEngine;

public class OnDeathUI : MonoBehaviour
{
    private Player mudak;

    public GameObject GhoulUI;

    void Start() {
        mudak = FindObjectOfType<Player>();
        mudak.OnDeath += onGameOver;
    }

    void onGameOver()
    {
        GhoulUI.SetActive(true);
    }

}
