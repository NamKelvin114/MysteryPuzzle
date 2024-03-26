using UnityEngine;

public class FlagScrollData
{
    public readonly Sprite iconCountry;
    public readonly string nameCountry;
    public readonly string countryCode;

    public FlagScrollData(Sprite iconCountry, string nameCountry, string countryCode)
    {
        this.iconCountry = iconCountry;
        this.nameCountry = nameCountry;
        this.countryCode = countryCode;
    }
}