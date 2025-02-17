using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Building
{
    [Header("商品項目")]
    public List<Cargo> ShopCargo = new List<Cargo>();
    bool HavePlayer;
    public void Update()
    {
        if (IsOpen())
        {
            if (HavePlayer)
            {
                OpenShop();
            }
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HavePlayer = true;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HavePlayer = false;
        }
    }
    void OpenShop()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            GM.UIManger.UI_interface(GM.ShopManger.ShopUI);
            GM.ShopManger.LoadCommodityData(this);
        }
    }

}
[System.Serializable]
public class Cargo
{
    public Item item;
    public int Cost;
}

