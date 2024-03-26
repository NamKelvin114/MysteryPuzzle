using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if (other.CompareTag("arrow"))
        {
            other.gameObject.SetActive(false);
            GameManager.instance.targetCollects.Remove(other.transform);
        }

        if (other.CompareTag("Trap_Other"))
        {
            other.gameObject.SetActive(false);
        }
    }
}