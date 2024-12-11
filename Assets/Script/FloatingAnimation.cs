using UnityEngine;

public class FloatingAnimation : MonoBehaviour
{
    public float amplitude = 0.5f; // Amplitudo (ketinggian naik-turun)
    public float frequency = 3f;  // Frekuensi (kecepatan naik-turun)

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPos + new Vector3(0, yOffset, 0);
    }
}
