using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

// ReSharper disable InconsistentNaming

[Serializable]
public class CountryCode : ScriptableObject
{
    public List<IconCountryCode> countryCodes;

    public Sprite GetIcon(string code)
    {
        Enum.TryParse(code, out ECountryCode countryCode);

        try
        {
            var index = (int) countryCode;
            var icon = countryCodes[index].icon;
            return icon;
        }
        catch (Exception)
        {
            return null;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Execute")]
    public void Execute()
    {
        string[] guids2 = AssetDatabase.FindAssets("t:texture2D", new[] {"Assets\\Roots\\Sprites\\CountryIcon"});
        for (int i = 0; i < 230; i++)
        {
            countryCodes[i].code = (ECountryCode) i;

            for (int j = 0; j < guids2.Length; j++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids2[j]);
                if (countryCodes[i].code.ToString() == Path.GetFileNameWithoutExtension(path))
                {
                    countryCodes[i].icon = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                }
            }
        }
    }
#endif
}

[Serializable]
public class IconCountryCode
{
    public ECountryCode code;
    public Sprite icon;
}

public enum ECountryCode
{
    IN,
    US,
    RU,
    ID,
    PK,
    DE,
    EG,
    PH,
    IQ,
    FR,
    JP,
    KR,
    UA,
    IR,
    GB,
    MY,
    BR,
    CA,
    UZ,
    TW,
    TR,
    BD,
    VN,
    KH,
    KZ,
    PL,
    DZ,
    ZA,
    MM,
    TH,
    AU,
    IT,
    LK,
    MA,
    MX,
    ES,
    NZ,
    CH,
    KG,
    SA,
    AR,
    BY,
    GR,
    CZ,
    HU,
    NL,
    HK,
    RO,
    CO,
    NP,
    PT,
    NO,
    PE,
    AZ,
    AE,
    AT,
    BG,
    JO,
    TN,
    TM,
    BE,
    LB,
    IL,
    VE,
    NG,
    RS,
    TJ,
    EC,
    CL,
    AF,
    SK,
    SY,
    GT,
    KE,
    YE,
    DO,
    LA,
    HR,
    SE,
    GE,
    LY,
    OM,
    MN,
    GH,
    SG,
    MD,
    PS,
    FI,
    BO,
    IE,
    AM,
    LT,
    DK,
    HN,
    SV,
    AL,
    QA,
    SI,
    CN,
    SD,
    LV,
    CR,
    UY,
    TZ,
    KW,
    MU,
    SN,
    UG,
    PA,
    JM,
    BA,
    FJ,
    PR,
    PY,
    TT,
    CY,
    ET,
    NI,
    CI,
    CM,
    HT,
    EE,
    RE,
    CD,
    SO,
    MG,
    GY,
    PG,
    ML,
    ZW,
    MK,
    BT,
    NA,
    BH,
    SR,
    XK,
    MV,
    BF,
    MT,
    BN,
    NC,
    GA,
    BJ,
    GP,
    CU,
    ME,
    TL,
    ZM,
    LU,
    BW,
    MZ,
    TG,
    GN,
    AO,
    PF,
    BZ,
    IS,
    MR,
    NE,
    TO,
    RW,
    MO,
    EH,
    BS,
    BB,
    DJ,
    KI,
    CV,
    GU,
    LC,
    MQ,
    WS,
    SL,
    SZ,
    MW,
    SB,
    KY,
    FM,
    CW,
    JE,
    GF,
    SC,
    YT,
    AG,
    GL,
    BI,
    LR,
    GM,
    VC,
    TD,
    CG,
    VI,
    CF,
    AS,
    PW,
    AD,
    GQ,
    MH,
    KM,
    GD,
    GI,
    LS,
    VU,
    MS,
    SS,
    DM,
    AW,
    LI,
    SX,
    BQ,
    BM,
    MF,
    SM,
    ER,
    ST,
    CK,
    KN,
    GW,
    VG,
    TV,
    TC,
    NR,
    FO,
    GG,
    MP,
    NU,
    AI,
    BL,
    MC,
    AX,
}