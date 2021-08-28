using Assets.Scripts;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public RectTransform healthBar;
    private Player _player;
    void Start()
    {
        _player = FindObjectOfType<Player>();
    }


    void Update()
    {
        float healthPercent = 0;
        if (_player!=null)
        {
             healthPercent = _player.health / _player.startHealth;
        } 
        healthBar.localScale = new Vector3(healthPercent,1,1);
    }
}
