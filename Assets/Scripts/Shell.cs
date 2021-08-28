using System.Collections;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody MyRigidbody;
    public float forceMin;
    public float forceMax;

    private float lifetime = 4;
    private float fadetime = 2;
    void Start()
    {
        float force = Random.Range(forceMin,forceMax);
        MyRigidbody.AddForce(transform.right*force);
        MyRigidbody.AddTorque(Random.insideUnitSphere*force);

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifetime);

        float percent = 0;
        float fadeSpeed = 1 / fadetime;

        Material mat = GetComponent<Renderer>().material;
        Color initColor = mat.color;

        while (percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;

            mat.color = Color.Lerp(initColor,Color.clear, percent);
            yield return null;
        }

        Destroy(gameObject);
    }
}
