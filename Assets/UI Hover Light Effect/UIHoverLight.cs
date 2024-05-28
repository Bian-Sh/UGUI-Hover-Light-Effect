using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIHoverLight : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, ColorUsage(true, true)]
    private Color color = Color.white;

    [SerializeField]
    private int radius = 100;

    private float scaleFactor; // canvas .scaleFactor
    private Material material;
    private Canvas canvas;
    Coroutine coroutine_hide;
    Coroutine coroutine_show;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        scaleFactor = canvas.scaleFactor;
        var image = GetComponent<Image>();
        //create a new one as we need change color offset later but should not change the others
        material = new(image.material);
        image.material = material;
        material.SetColor("_Color", color);
        material.SetFloat("_ColorRadius", radius);
        // as canvas will change its scale when  screen's  resolution changed
        // so we apply the scale factor to the shader to keep the effect the same
        material.SetFloat("_ScaleFactor", scaleFactor); 
    }
    private void Update()
    {
        material.SetVector("_MousePos", Input.mousePosition);
        if (scaleFactor!=canvas.scaleFactor)
        {
            scaleFactor = canvas.scaleFactor;
            material.SetFloat("_ScaleFactor", scaleFactor);
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (null != coroutine_show)
        {
            StopCoroutine(coroutine_show);
        }
        if (null != coroutine_hide)
        {
            StopCoroutine(coroutine_hide);
        }
        coroutine_show = StartCoroutine(ChangeInnerColorStateAsync(0.3f, true));
    }
    private IEnumerator ChangeInnerColorStateAsync(float duration, bool show)
    {
        #region Ease
        // reference: https://github.com/setchi/EasingCore/blob/master/EasingCore.cs
        static float inCubic(float t) => t * t * t;
        static float outCubic(float t) => inCubic(t - 1f) + 1f;
        #endregion

        float time = 0;
        float start = material.GetFloat("_ColorOffset");
        float end = show ? 0 : 0.98f;    // 当 _ColorOffset = 0 显示内部颜色
        Func<float, float> ease = show ? outCubic : inCubic;
        while (time < duration)
        {
            time += Time.deltaTime;
            var p = ease(time / duration);
            material.SetFloat("_ColorOffset", Mathf.Lerp(start, end, p));
            yield return null;
        }
    }

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
        if (null != coroutine_show)
        {
            yield return coroutine_show;
        }
        coroutine_hide = StartCoroutine(ChangeInnerColorStateAsync(0.1f, false));
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 逻辑使然，只有在运行时才能设置材质球颜色和半径
        if (null != material&&Application.isPlaying)
        {
            material.SetColor("_Color", color);
            material.SetFloat("_ColorRadius", radius);
        }
    }
#endif
}
