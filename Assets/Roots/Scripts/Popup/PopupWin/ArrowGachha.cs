using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ArrowGachha : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float rotationSpeed = .5f;
    public bool isMoving = true;
    private bool rotationDirection = false;
    private float nowZRotation;
    private void OnEnable()
    {
        isMoving = true;
        Vector3 newRotation = new Vector3(transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y, 0);
        transform.rotation= Quaternion.Euler(newRotation);
        rotationDirection = false;
        nowZRotation = 0;
    }

    private void Update()
    {
        if (isMoving)
        {
            if (!rotationDirection)
            {
                
                nowZRotation += Time.deltaTime * rotationSpeed;
                if (nowZRotation >= 90f)
                {
                    nowZRotation = Math.Min(nowZRotation, 90f);
                    rotationDirection = !rotationDirection;
                }
                float newZRotation = nowZRotation < 0 ? nowZRotation : nowZRotation + 360f ;
                Vector3 newRotation = new Vector3(transform.rotation.eulerAngles.x, 
                    transform.rotation.eulerAngles.y, newZRotation);
                transform.rotation= Quaternion.Euler(newRotation);
            }
            else
            {
                nowZRotation -= Time.deltaTime * rotationSpeed;
                if (nowZRotation <= -90f)
                {
                    nowZRotation = Math.Max(nowZRotation, -90f);
                    rotationDirection = !rotationDirection;
                }
                float newZRotation = nowZRotation < 0 ? nowZRotation : nowZRotation + 360f ;
                Vector3 newRotation = new Vector3(transform.rotation.eulerAngles.x,
                    transform.rotation.eulerAngles.y, newZRotation);
                transform.rotation= Quaternion.Euler(newRotation);
            }
        }
    }
    

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("ArenaItemGaccha"))
        {
            var bonus = col.gameObject.GetComponent<ArenaItem>();
            int bonusReward = bonus.MultiBonus;
            Observer.UpdateBonusAdsButton?.Invoke(bonusReward);
            bonus.ChangeColorWhenEnterColider();
        }   
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ArenaItemGaccha"))
        {
            var bonus = other.gameObject.GetComponent<ArenaItem>();
            bonus.ChangeColorWhenExitColider();
        }   
    }
}
