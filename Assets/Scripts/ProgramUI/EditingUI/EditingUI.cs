using UnityEngine;

public class EditingUI : MonoBehaviour
{
    private readonly Vector2 elasticLerpStartScale = new(1, 1);
    private readonly Vector2 elasticLerpEndScale  = new(1.2f, 1.2f);
    private const float elasticLerpDuration = 0.8f;

    [Header("Animation Data")]
    public Sprite[] animSprites;
    public float[] animFrames;

    private LerpUIHandler lerpHandler;
    private CustomAnimator animator;
    private SpriteRenderer spriteRenderer;

    [HideInInspector] public ProgramInputManager inputManager;

    void Start()
    {
        inputManager = FindObjectOfType<ProgramInputManager>();

        lerpHandler = gameObject.GetComponent<LerpUIHandler>();
        animator = gameObject.GetComponent<CustomAnimator>();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        if (inputManager != null)
        {
            inputManager.OnSlowModeEnter += StartEditingAnimation;
            inputManager.OnSlowModeExit += StopEditingAnimation;
        }
    }

    public void StartEditingAnimation()
    {
        spriteRenderer.enabled = true;
        animator.PlayAnimation(animSprites, animFrames, ProgramType.Other, true);
        lerpHandler.ElasticScaleLerp(elasticLerpStartScale, elasticLerpEndScale, elasticLerpDuration);
    }

    public void StopEditingAnimation()
    {
        animator.StopAnimation();
        spriteRenderer.enabled = false;
        lerpHandler.StopElasticLerp();
        gameObject.transform.localScale = elasticLerpStartScale;
    }

    void OnDestroy()
    {
        if (inputManager != null)
        {
            inputManager.OnSlowModeEnter -= StartEditingAnimation;
            inputManager.OnSlowModeExit -= StopEditingAnimation;
        }
    }
}
