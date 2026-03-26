using UnityEngine;

public class LiquidMaterialScroller : MonoBehaviour
{
    [Header("Scroll Settings")]
    public Vector2 scrollSpeed = new Vector2(0.1f, 0.05f);
    
    // Most liquid shaders in NSMB mods use "_MainTex" 
    // If yours is different, you can change it here in the Inspector.
    public string texturePropertyName = "_MainTex";

    private Renderer _renderer;
    private Material _material;
    private Vector2 _currentOffset;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        
        if (_renderer != null)
        {
            // We use .material (instanced) so it doesn't affect 
            // the actual project file, only this specific object.
            _material = _renderer.material;
        }
        else
        {
            Debug.LogError($"No Renderer found on {gameObject.name}. Please attach this to an object with a SpriteRenderer or MeshRenderer.");
            enabled = false;
        }
    }

    void Update()
    {
        // Calculate the movement
        _currentOffset += scrollSpeed * Time.deltaTime;

        // Apply it to the material property
        _material.SetTextureOffset(texturePropertyName, _currentOffset);
    }

    void OnDestroy()
    {
        // Clean up the instanced material when the object is destroyed
        if (_material != null)
        {
            Destroy(_material);
        }
    }
}