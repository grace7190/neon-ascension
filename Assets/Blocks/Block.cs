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
        MakeFallAfterDelay(BlockColumnManager.SlideBlockDuration);
    }

    public void MakeFallAfterDelay(float delay)
    {
        StartCoroutine(MakeFallCoroutine(delay));
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

    public void AnimateBlocked()
    {
        float waitDuration = 0.5f;
        StartCoroutine(AnimateBlockedCoroutine(waitDuration));
    }

    public void AnimateDeletion()
    {
        // Cause the blocks to stop moving
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
        StartCoroutine(AnimateDeletionCoroutine(6));
    }

    private IEnumerator AnimateDeletionCoroutine(int blinkTimes)
    {
        float flashDurationUnlocked = 0.8f;
        float flashDurationLocked = 0.1f;
        float changeTime = 0.01f;

        for (int i = 0; i < blinkTimes; i++) {
            ChangeColor(LockedColor, changeTime);
            yield return new WaitForSeconds(changeTime + flashDurationLocked);
            ChangeColor(BaseColor, changeTime);
            yield return new WaitForSeconds(changeTime + flashDurationUnlocked);
            flashDurationUnlocked -= flashDurationUnlocked * 0.5f;
        }
        ChangeColor(LockedColor, changeTime);

        // Fall due to gravity
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().isTrigger = true;
    }

    private IEnumerator AnimateBlockedCoroutine(float waitDuration)
    {
        ChangeColor(LockedColor, ChangeColorDuration);
        yield return new WaitForSeconds(ChangeColorDuration + waitDuration);
        ChangeColor(BaseColor, ChangeColorDuration);
    }

    private IEnumerator MakeFallCoroutine(float duration)
    {
        Debug.Log("Called");
        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        IsLocked = true;
        ChangeColor(LockedColor, ChangeColorDuration);

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = true;

        if (duration > 0)
        {
            yield return new WaitForSeconds(duration);
        }

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
