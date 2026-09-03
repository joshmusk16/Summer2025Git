using UnityEngine;

public class ShopManager : MonoBehaviour
{

private ShopTransition shopTransition;

private void FindDependencies()
{
    if(shopTransition == null) shopTransition = FindObjectOfType<ShopTransition>();
} 



}
