using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerAnmation : MonoBehaviour
{
    [SerializeField] Animator Animator;
    [Tooltip("0 Helmet\n1 Clothe\n2 L_Sleeve\n3 R_Sleeve\n4  L_Shoe\n5  R_Shoe")]
    [SerializeField] SpriteRenderer[] Outwear;
    public void SwapVillagerShow(VillagerApparel Apparel)
    {
        Apparel.LoadSprite(out Sprite Headwear, out Sprite Clothes, out Sprite L_Sleeve, out Sprite R_Sleeve, out Sprite L_Shoe, out Sprite R_Shoe);
        Outwear[0].sprite = Headwear;
        Outwear[1].sprite = Clothes;
        Outwear[2].sprite = L_Sleeve;
        Outwear[3].sprite = R_Sleeve;
        Outwear[4].sprite = L_Shoe;
        Outwear[5].sprite = R_Shoe;
    }
    public void ActionAnmtion(string Clip, bool IsOn)
    {
        Animator.SetBool(Clip, IsOn);
    }
}
