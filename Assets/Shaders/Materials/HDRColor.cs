using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class HDRColor : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color Color = new(1.5f, 1.5f, 1.5f, 1f);
    [ColorUsage(true, true)]
    private Color _prevColor = new(1f,1f,1f);

    [Space(5)]
    [SerializeField]
    private Material _baseMaterial;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        OnParamsChanged();
    }
    private void Update()
    {
        if (_prevColor != Color)
            OnParamsChanged();
    }

    private void OnValidate()
    {
        _prevColor = Color;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.sharedMaterial.color = Color;
    }

    private void OnParamsChanged()
    {
        _prevColor = Color;
        spriteRenderer.sharedMaterial.color = Color;
    }
}