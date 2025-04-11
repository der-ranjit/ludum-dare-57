using UnityEngine;

public class SpriteSyncer : MonoBehaviour
{
    private SpriteRenderer original;
    private SpriteRenderer clone;

    void Start()
    {
        // original = transform.parent.GetComponentInParent<SpriteRenderer>();
        original = transform.parent.GetComponentInParent<SpriteRenderer>();
        clone = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (!original || !clone) { return; }

        clone.sprite = original.sprite;
        clone.flipX = original.flipX;
        clone.flipY = original.flipY;
        clone.color = original.color;
    }
}
