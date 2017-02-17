using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour {
    
    public const float ChangeColorDuration = 0.2f;
    public static readonly Color BaseColor = new Color(0.132f, 6.0f, 5.272f);
    public static readonly Color LockedColor = Color.red;

    public bool IsLocked;

    private Rigidbody _rigidbody;

    void Start () 
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    public void MakeFall()
    {
        StartCoroutine(MakeFallCoroutine());
    }

    private IEnumerator MakeFallCoroutine()
    {
        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        IsLocked = true;
        StartCoroutine(ChangeColorCoroutine(LockedColor, ChangeColorDuration));

        yield return new WaitForSeconds(BlockColumnManager.SlideBlockDuration);
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = Vector3.down * 0.05f;
        yield return new WaitForFixedUpdate();
        
        while (_rigidbody.velocity.y < 0)
        {
            yield return new WaitForFixedUpdate();
        }
        _rigidbody.isKinematic = true;
        transform.position = transform.position.RoundToInt();

        StartCoroutine(ChangeColorCoroutine(BaseColor, ChangeColorDuration));
        IsLocked = false;
    }

    private IEnumerator ChangeColorCoroutine(Color targetColor, float duration)
    {
        var material = new Material(GetComponent<Renderer>().sharedMaterial);
        var oldColor = material.color;
        GetComponent<Renderer>().sharedMaterial = material;

        var t = 0f;
        while (t <= duration)
        {
            var currColor = Color.Lerp(oldColor, targetColor, t / duration);
            material.SetColor("_EmissionColor", currColor);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }

        material.SetColor("_EmissionColor", targetColor);
    }
}
