using UnityEngine;

public class AutoBrightnessMatch : MonoBehaviour
{
    [Header("Targeting")]
    public SpriteRenderer targetRenderer;
    
    [Header("Settings")]
    [Tooltip("How much brighter (0-255) than the environment should this be?")]
    public float brightnessOffset = 40f;
    
    // Internal conversion to 0.0 - 1.0 range
    private float normalizedOffset => brightnessOffset / 255f;

    void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (targetRenderer == null) return;

        // 1. Get the current ambient lighting color
        // This detects the global light level of your scene
        Color ambient = RenderSettings.ambientLight;

        // 2. Calculate the "Brightness" (Luminance)
        float currentBrightness = (ambient.r + ambient.g + ambient.b) / 3f;

        // 3. Apply the offset
        float targetVal = Mathf.Clamp01(currentBrightness + normalizedOffset);

        // 4. Update the sprite color while keeping its original hue
        // This ensures if the sprite is green, it stays green but gets brighter
        Color spriteCol = targetRenderer.color;
        targetRenderer.color = new Color(
            Mathf.Clamp01(spriteCol.r + (targetVal - currentBrightness)),
            Mathf.Clamp01(spriteCol.g + (targetVal - currentBrightness)),
            Mathf.Clamp01(spriteCol.b + (targetVal - currentBrightness)),
            spriteCol.a // Keep original transparency
        );
    }
}