using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCheck : MonoBehaviour
{
    public bool CanBuilding;
    public bool Isobstructed;
    [SerializeField]BoxCollider2D Collider;
    public void SwapColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
    public void Check(bool check)
    {
        if(check&&!Collider.IsTouchingLayers(1 << LayerMask.NameToLayer("Collectibles")| 1 << LayerMask.NameToLayer("Building")))
        {
            CanBuilding = true;
            SwapColor(Color.green);
        }
        else
        {
            CanBuilding = false;
            SwapColor(Color.red);
            SwapColorforObstructed();
        }
    }
    void SwapColorforObstructed()
    {
        RaycastHit2D[] Hits= Physics2D.BoxCastAll(transform.position, Collider.size, 0,Camera.main.transform.forward , Collider.size.x+ Collider.size.y, 1 << LayerMask.NameToLayer("Collectibles") | 1 << LayerMask.NameToLayer("Building"));
        foreach(RaycastHit2D Hit in Hits)
        {
            if (Hit.transform.GetComponent<Collection>())
            {
                Hit.transform.GetComponent<Collection>().ChangeColorTime = 0.2f;
            }
            if (Hit.transform.GetComponent<Building>())
            {
                Hit.transform.GetComponent<Building>().ChangeColorTime = 0.2f;
            }

        }
    }
}
