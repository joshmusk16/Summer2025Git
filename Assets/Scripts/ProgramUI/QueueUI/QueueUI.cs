using UnityEngine;

public class QueueUI : MonoBehaviour
{
    private ProgramType queueType = ProgramType.Attack;

    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite activeAttackSprite;
    [SerializeField] private Sprite activeDefenseSprite;

    private SpriteRenderer sr;

    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    public void SetActive()
    {
        if(queueType == ProgramType.Attack)
        {
            sr.sprite = activeAttackSprite;   
        }
        else if(queueType == ProgramType.Defense)
        {
            sr.sprite = activeDefenseSprite;
        }
    }

    public void SetInactive()
    {
        sr.sprite = inactiveSprite;
    }
}
