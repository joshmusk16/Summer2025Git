using UnityEngine;

public class DamageNumberManager : MonoBehaviour
{
    public GameObject damageNumber;
    public HurtBox hurtBox;
    public float verticalSpawnOffset;

    void Start()
    {
        hurtBox.OnHit += SpawnDamageNumber;
    }

    public void SpawnDamageNumber(HitInfo info)
    {
        GameObject temp = Instantiate(damageNumber, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        temp.GetComponent<DamageNumber>().parent = gameObject;
        temp.GetComponent<DamageNumber>().verticalSpawnOffset = verticalSpawnOffset;
        temp.GetComponent<DamageNumber>().InstantiateDamageNumber(info.damage);
    }

    void OnDestroy()
    {
        hurtBox.OnHit -= SpawnDamageNumber;
    }
}
