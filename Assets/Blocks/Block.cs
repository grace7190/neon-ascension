using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour {
    
    public const float ChangeColorDuration = 0.2f;

    public static readonly Color NeutralColor = new Color(3f, 3f, 3f);
    public static readonly Color BlueColor = new Color(0.132f, 6.0f, 5.272f);
    public static readonly Color PurpleColor = new Color(6.0f, 0.132f, 5.272f);
    public static readonly Color LockedColor = new Color(6, 0.6f, 0.0f);

    public Color BaseColor = NeutralColor;
    public bool IsLocked;

    private const int CastMask = 1 << Layers.Player;
    private const float CastRadius = 0.1f;
    private IEnumerator _colorChangeCoroutine;
    private Rigidbody _rigidbody;


    void Start ()
    {
        Initialize();
    }

    public void Initialize()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void MakeFallImmediately()
    {
        StartCoroutine(MakeFallCoroutine(0));
    }

    public void MakeFallAfterSlideBlockDelay()
    {
        StartCoroutine(MakeFallCoroutine(BlockColumnManager.SlideBlockDuration));
    }

    public GameObject GetPlayerInDirection(Vector3 direction)
    {
        var colliders =
            Physics.OverlapSphere(transform.position + direction,
                                  CastRadius,
                                  CastMask,
                                  QueryTriggerInteraction.Ignore);

        foreach (Collider collider in colliders)
        {
            if (collider.tag == Tags.Player) {
                return collider.gameObject;
            }
        }

        return null;
    }

    private IEnumerator MakeFallCoroutine(float duration)
    {
        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        IsLocked = true;
        ChangeColor(LockedColor, ChangeColorDuration);


        yield return new WaitForSeconds(duration);
        _rigidbody.isKinematic = false;
        yield return new WaitForFixedUpdate();
        
        while (_rigidbody.velocity.y < 0)
        {
            yield return new WaitForFixedUpdate();
        }
        _rigidbody.isKinematic = true;
        transform.position = transform.position.RoundToInt();

        ChangeColor(BaseColor, ChangeColorDuration);
        IsLocked = false;
    }

    private void ChangeColor(Color targetColor, float duration)
    {
        if (_colorChangeCoroutine != null)
        {
            StopCoroutine(_colorChangeCoroutine);
        }
        _colorChangeCoroutine = ChangeColorCoroutine(targetColor, duration);
        StartCoroutine(_colorChangeCoroutine);
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
            material.SetColor("_Color", currColor);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }

        material.SetColor("_Color", targetColor);
    }
}
