using UnityEngine;

public class BanNahui : MonoBehaviour
{
    [SerializeField]
    private GameObject banner;
    void Minus()
    {
        banner.SetActive(false);
    }
}
