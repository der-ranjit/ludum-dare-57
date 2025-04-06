using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Position Wobble")]
    [SerializeField] private float positionWobbleAmount = 0.1f;
    [SerializeField] private float positionWobbleSpeed = 2.0f;
    
    [Header("Intensity Wobble")]
    [SerializeField] private float intensityWobbleAmount = 0.2f;
    [SerializeField] private float intensityWobbleSpeed = 1.5f;
    private float baseIntensity;
    
    [Header("Range Wobble")]
    [SerializeField] private float rangeWobbleAmount = 0.5f;
    [SerializeField] private float rangeWobbleSpeed = 1.0f;
    private float baseRange;
    
    [Header("Wobble Complexity")]
    [SerializeField] private int wobbleSinCount = 3;
    [SerializeField] private float wobblePhaseShift = 0.5f;
    
    private Light pointLight;
    private Vector3 originalPosition;
    
    private void Awake()
    {
        pointLight = GetComponent<Light>();
        if (pointLight == null)
        {
            Debug.LogError("LightFlicker script must be attached to a GameObject with a Light component!");
            enabled = false;
            return;
        }
        
        originalPosition = transform.localPosition;
        
        // Initialize base values if not set
        baseIntensity = pointLight.intensity;
    
        baseRange = pointLight.range;
    }
    
    private void Update()
    {
        float time = Time.time;
        
        // Update position
        if (positionWobbleAmount > 0)
        {
            Vector3 wobbleOffset = new Vector3(
                GetWobble(time, positionWobbleSpeed, 0) * 0.5f,
                GetWobble(time, positionWobbleSpeed * 1.3f, 1) * 2f,
                GetWobble(time, positionWobbleSpeed * 1.543f, 2) * 0.5f
            ) * positionWobbleAmount;
            
            transform.localPosition = originalPosition + wobbleOffset;
        }
        
        // Update intensity
        if (intensityWobbleAmount > 0)
        {
            float intensityWobble = GetWobble(time, intensityWobbleSpeed, 3);
            pointLight.intensity = baseIntensity + (intensityWobble * intensityWobbleAmount);
        }
        
        // Update range
        if (rangeWobbleAmount > 0)
        {
            float rangeWobble = GetWobble(time, rangeWobbleSpeed, 4);
            pointLight.range = baseRange + (rangeWobble * rangeWobbleAmount);
        }
    }
    
    // The wobble function that combines multiple sine waves to get a value between -1 and 1
    private float GetWobble(float time, float speed, int seedOffset)
    {
        float wobble = 0f;
        float amplitude = 1f;
        
        for (int i = 0; i < wobbleSinCount; i++)
        {
            float frequency = speed * (1f + i * 0.7f);
            float phase = seedOffset + i * wobblePhaseShift;
            wobble += Mathf.Sin(time * frequency + phase) * (amplitude / wobbleSinCount);
        }
        
        return wobble;
    }
    
    // Reset the light properties when the script is disabled
    private void OnDisable()
    {
        if (pointLight != null)
        {
            transform.localPosition = originalPosition;
            pointLight.intensity = baseIntensity;
            pointLight.range = baseRange;
        }
    }
}