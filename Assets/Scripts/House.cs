using UnityEngine;

public class House : MonoBehaviour
{ 
    public SpriteRenderer spriteRenderer { get; private set; }
    public new BoxCollider2D collider { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            this.gameObject.SetActive(false);
        }
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();

        ResetHouse();
    }

    public void ResetHouse()
    {
        gameObject.SetActive(true);
    }
}
