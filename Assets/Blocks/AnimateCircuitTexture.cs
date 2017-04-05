using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateCircuitTexture : MonoBehaviour {

    public Texture2D txtr1;
    public Texture2D txtr2;
    public Texture2D txtr3;
    public Texture2D txtr4;
    public Texture2D txtr5;
    Texture2D[] textures;

    private int currTxtrIdx = 0;
    private int newTxtrIdx = 0;
    float changetime = 2.0f;

    // Use this for initialization
    void Start () {
        textures = new Texture2D[] { txtr1, txtr2, txtr3, txtr4, txtr5 };
        StartCoroutine(TextureChangeCoroutine(changetime));
    }

    // Update is called once per frame
    void Update () {
		
	}

    private IEnumerator TextureChangeCoroutine(float duration)
    {
        var mat = gameObject.GetComponent<Renderer>().material;

        currTxtrIdx = newTxtrIdx;
        newTxtrIdx = (currTxtrIdx + Random.Range(1, textures.Length)) % textures.Length;
        var newTexture = textures[newTxtrIdx];
        gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", textures[currTxtrIdx]);
        gameObject.GetComponent<Renderer>().material.SetTexture("_Texture2", newTexture);
        gameObject.GetComponent<Renderer>().material.SetFloat("_Blend", 0.0f);

        var t = 0f;
        while (t <= duration)
        {
            mat.SetFloat("_Blend", t / duration);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
        StartCoroutine(TextureChangeCoroutine(duration));
    }
    
}
