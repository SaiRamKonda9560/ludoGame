using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class textTmpAndImageEffects : MonoBehaviour
{
    public Color colorA;
    public Color colorB;
    public float speed = 1.0f;
    public EffectType EffectType;
    public float time;
    public bool onEnableResetTime;
    private void OnEnable()
    {
        if (onEnableResetTime)
        {
            time = 0f;
        }
    }
    void FixedUpdate()
    {
        time += Time.deltaTime;
        if(time >= 0)
        {
            var Image = GetComponent<Image>();
            var TMP_Text = GetComponent<TMP_Text>();
            if (Image || TMP_Text)
            {
                switch (EffectType)
                {
                    case EffectType.normalLerp:
                        if (Image)
                        {
                            Image.color = Color.Lerp(colorA, colorB, Mathf.Clamp01(time * speed));
                        }
                        if (TMP_Text)
                        {
                            TMP_Text.color = Color.Lerp(colorA, colorB, Mathf.Clamp01(time * speed));
                        }
                        break;
                    case EffectType.Repeat:
                        if (Image)
                        {
                            Image.color = Color.Lerp(colorA, colorB, Mathf.Repeat(time * speed, 1f));
                        }
                        if (TMP_Text)
                        {
                            TMP_Text.color = Color.Lerp(colorA, colorB, Mathf.Repeat(time * speed, 1f));
                        }
                        break;
                    case EffectType.pingPong:
                        if (Image)
                        {
                            Image.color = Color.Lerp(colorA, colorB, Mathf.PingPong(time * speed, 1f));
                        }
                        if (TMP_Text)
                        {
                            TMP_Text.color = Color.Lerp(colorA, colorB, Mathf.PingPong(time * speed, 1f));
                        }
                        break;
                }

            }
        }

    }
}
public enum EffectType
{
    normalLerp, pingPong, Repeat 
}