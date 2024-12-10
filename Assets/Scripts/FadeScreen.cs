using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    public bool fadeOnStart = true;
    public float fadeDuration = 2;
    public Color fadeColor;
    private Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        if (fadeOnStart)
        {
            FadeIn();
        }
    }
    public void FadeIn()
    {
        Fade(1,0);
    }
    public void FadeOut()
    {
        Fade(0,1);
    }
   public void Fade(float alphaIn, float alphaOut)
   {
    StartCoroutine(FadeRoutine(alphaIn,alphaOut));
   }

   public IEnumerator FadeRoutine(float alphaIn, float alphaOut)
   {
    //starts w/ timer of 0
    float timer = 0;
    //once timer is bigger than fade duration it will exit loop
    while(timer <= fadeDuration)
    {
        Color newColor = fadeColor;
        newColor.a = Mathf.Lerp(alphaIn, alphaOut,timer/fadeDuration);
        rend.material.SetColor("_BaseColor", newColor);
        //every frame increases timer by delta time
        timer += Time.deltaTime;
        yield return null;
    }
        Color newColor2 = fadeColor;
        newColor2.a = alphaOut;
        rend.material.SetColor("_BaseColor", newColor2);
   }
}
