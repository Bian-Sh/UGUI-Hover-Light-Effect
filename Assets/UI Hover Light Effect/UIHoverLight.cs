using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIHoverLight : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Material material;
    private void Start()
    {
        var image = GetComponent<Image>();
        //create a new one as we need change color offset later but should not change the others
        material = new(image.material);
        image.material = material;
    }
    private void Update() => material.SetVector("_MousePos", Input.mousePosition);

    Coroutine coroutine;
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (null != coroutine)
        {
            StopCoroutine(coroutine);
        }
        if (null != coroutine_hide) 
        {
            StopCoroutine(coroutine_hide);
        }
        coroutine = StartCoroutine(ChangeInnerColorStateAsync(0.3f, true));
    }

    // 当 _ColorOffset = 0 显示内部颜色
    private IEnumerator ChangeInnerColorStateAsync(float duration, bool show)
    {
        #region Ease
        static float inCubic(float t) => t * t * t;
        static float outCubic(float t) => inCubic(t - 1f) + 1f;
        #endregion

        float time = 0;
        float start = material.GetFloat("_ColorOffset");
        float end = show ? 0 : 0.98f;
        Func<float, float> ease = show ? outCubic : inCubic;
        while (time < duration)
        {
            time += Time.deltaTime;
            var p = ease(time / duration);
            material.SetFloat("_ColorOffset", Mathf.Lerp(start, end, p));
            yield return null;
        }
    }


    Coroutine coroutine_hide;
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (null != coroutine_hide)
        {
            StopCoroutine(coroutine_hide);
        }
        coroutine_hide = StartCoroutine(HideInnerColorAsync());
    }

    private IEnumerator HideInnerColorAsync()
    {
        //等待ShowInnerColorAsync结束
        if (null != coroutine)
        {
            yield return coroutine;
        }
        coroutine_hide = StartCoroutine(ChangeInnerColorStateAsync(0.1f, false));
    }
}
