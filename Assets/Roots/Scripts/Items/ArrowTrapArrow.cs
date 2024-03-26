using UnityEngine;

public class ArrowTrapArrow : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if (other.CompareTag("BodyPlayer") || other.CompareTag("Enemy") || other.CompareTag("Hostage") || other.CompareTag("Wolf"))
        {
            gameObject.SetActive(false);
        }
    }
}