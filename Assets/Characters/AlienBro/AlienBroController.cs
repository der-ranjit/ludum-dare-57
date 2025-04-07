using UnityEngine;

public class AlienBroController : Enemy
{

    public float wobbleAmplitude = 0.5f; // Amplitude of the wobble (height of the up-and-down motion)
    public float wobbleFrequency = 2f; // Frequency of the wobble (speed of the up-and-down motion)

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        Attack();
        // Add wobble effect to the Y position
        float wobbleOffset = Mathf.Sin(Time.time * wobbleFrequency) * wobbleAmplitude;
        Vector3 velocity = rb.velocity;
        velocity.y = wobbleOffset;
        rb.velocity = velocity;
    }
}
