using UnityEngine;

public class HolyWaterHostage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpecialItem"))
        {
            HolyWaterItem holy = other.GetComponent<HolyWaterItem>();
            if (holy != null)
            {
                holy.effectUsed.transform.SetParent(GameManager.instance.Root.transform, true);
                holy.effectUsed.SetActive(true);
                HostageManager.instance.OnTakeHolyWater(other.transform);
            }
        }
    }
}