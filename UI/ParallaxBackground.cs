using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{

    [SerializeField] private Vector2 ParallaxEffectMultiplier;
    [SerializeField] private float Speed;
    private Transform CameraTransform;
    private Vector3 LastCameraPosition;
    private float TextureUnitSizeX;
    private float TextureUnitSizeY;
    private void Start()
    {
        CameraTransform = Camera.main.transform;
        LastCameraPosition = CameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        TextureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        TextureUnitSizeY = texture.height / sprite.pixelsPerUnit;
    }
    private void FixedUpdate()
    {
        transform.Translate(Speed * Time.fixedDeltaTime * Vector3.left, Space.World);
    }
    private void LateUpdate()
    {
        Vector3 DeltaMovement = CameraTransform.position - LastCameraPosition;
        transform.position +=new Vector3(DeltaMovement.x* ParallaxEffectMultiplier.x, DeltaMovement.y* ParallaxEffectMultiplier.y);
        LastCameraPosition = CameraTransform.position;
        if (Mathf.Abs(CameraTransform.position.x - transform.position.x) >= TextureUnitSizeX)
        {
            float offsetPostionX = (CameraTransform.position.x - transform.position.x) % TextureUnitSizeX;
            transform.position = new Vector3(CameraTransform.position.x+ offsetPostionX, transform.position.y);
        }
        if (Mathf.Abs(CameraTransform.position.y - transform.position.y) >= TextureUnitSizeY)
        {
            float offsetPostionY = (CameraTransform.position.y - transform.position.y) % TextureUnitSizeY;
            transform.position = new Vector3(transform.position.x , CameraTransform.position.y+ offsetPostionY);
        }
    }
}
