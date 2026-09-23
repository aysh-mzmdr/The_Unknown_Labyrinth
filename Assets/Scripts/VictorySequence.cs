using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VictorySequence : MonoBehaviour
{
    public MonoBehaviour playerController;
    public float fireworksDuration = 5f;
    public AudioSource musicSource;
    public AudioClip wonMusicClip;
    public Material fireworkMaterial;

    private bool triggered = false;

    private GameObject wonPlayerObject;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc == null) return;

        triggered = true;
        wonPlayerObject = other.gameObject;

        if (playerController != null) playerController.enabled = false;
        FreezeToIdle();
        PlayWonMusic();

        StartCoroutine(RunVictory());
    }

    void FreezeToIdle()
    {
        if (wonPlayerObject == null) return;

        AudioSource src = wonPlayerObject.GetComponentInParent<AudioSource>();
        if (src != null) src.Stop();

        Animator anim = wonPlayerObject.GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.SetBool("IsMoving", false);
            anim.Play("Standing Idle", 0, 0f);
        }
    }

    void PlayWonMusic()
    {
        if (musicSource == null || wonMusicClip == null) return;
        musicSource.Stop();
        musicSource.clip = wonMusicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    IEnumerator RunVictory()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return null;
        if (playerController != null) playerController.enabled = false;
        FreezeToIdle();

        RectTransform textRect = CreateVictoryUI();
        StartCoroutine(PulseText(textRect));

        Vector3 center = transform.position + Vector3.up * 1.6f;
        float elapsed = 0f;
        while (elapsed < fireworksDuration)
        {
            Vector3 pos = center + new Vector3(Random.Range(-2f, 2f), Random.Range(0.3f, 2.2f), Random.Range(-2f, 2f));
            SpawnFirecracker(pos);
            float wait = Random.Range(0.2f, 0.45f);
            elapsed += wait;
            yield return new WaitForSeconds(wait);
        }
    }

    RectTransform CreateVictoryUI()
    {
        GameObject canvasGO = new GameObject("VictoryCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject textGO = new GameObject("YouWonText", typeof(RectTransform));
        textGO.transform.SetParent(canvasGO.transform, false);
        RectTransform rt = textGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(1500f, 300f);

        Text text = textGO.AddComponent<Text>();
        text.text = "YOU WON!!!!";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 130;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(1f, 0.85f, 0.2f);
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        Outline outline = textGO.AddComponent<Outline>();
        outline.effectColor = new Color(0.4f, 0.1f, 0f, 1f);
        outline.effectDistance = new Vector2(4f, -4f);

        Shadow shadow = textGO.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.6f);
        shadow.effectDistance = new Vector2(5f, -7f);

        return rt;
    }

    IEnumerator PulseText(RectTransform rt)
    {
        float t = 0f;
        while (rt != null)
        {
            t += Time.deltaTime * 3f;
            float scale = 1f + Mathf.Sin(t) * 0.08f;
            rt.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }

    void SpawnFirecracker(Vector3 pos)
    {
        Color fireColor = RandomFireworkColor();

        GameObject fx = new GameObject("Firecracker");
        fx.transform.position = pos;

        ParticleSystem ps = fx.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.playOnAwake = false;
        var main = ps.main;
        main.duration = 1f;
        main.loop = false;
        main.startLifetime = 0.8f;
        main.startSpeed = 4f;
        main.startSize = 0.4f;
        main.gravityModifier = 1f;
        main.startColor = new ParticleSystem.MinMaxGradient(fireColor);
        main.stopAction = ParticleSystemStopAction.Destroy;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 40) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        var colOverLife = ps.colorOverLifetime;
        colOverLife.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colOverLife.color = grad;

        ParticleSystemRenderer psr = fx.GetComponent<ParticleSystemRenderer>();
        Material mat = fireworkMaterial;
        if (mat == null)
        {
            Shader shader = Shader.Find("Sprites/Default");
            if (shader != null) mat = new Material(shader);
        }
        if (mat != null) psr.sharedMaterial = mat;

        ps.Play();

        Light flash = fx.AddComponent<Light>();
        flash.type = LightType.Point;
        flash.color = fireColor;
        flash.range = 6f;
        flash.intensity = 4f;
        StartCoroutine(FadeOutLight(flash));

        Destroy(fx, 2f);
    }

    IEnumerator FadeOutLight(Light l)
    {
        float dur = 0.4f;
        float t = 0f;
        float startIntensity = l.intensity;
        while (t < dur)
        {
            t += Time.deltaTime;
            if (l == null) yield break;
            l.intensity = Mathf.Lerp(startIntensity, 0f, t / dur);
            yield return null;
        }
    }

    Color RandomFireworkColor()
    {
        Color[] palette = new Color[] {
            new Color(1f, 0.2f, 0.2f), new Color(1f, 0.85f, 0.2f), new Color(0.3f, 1f, 0.4f),
            new Color(0.3f, 0.6f, 1f), new Color(1f, 0.4f, 0.9f), new Color(1f, 1f, 1f)
        };
        return palette[Random.Range(0, palette.Length)];
    }
}
