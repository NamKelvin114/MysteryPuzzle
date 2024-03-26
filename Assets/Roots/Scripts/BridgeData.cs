#pragma warning disable 649
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pancake.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using Worldreaver.Root.Attribute;
using Worldreaver.Utility;

/// <summary>
/// 
/// </summary>
public class BridgeData : Singleton<BridgeData>
{
    [ReadOnly] public LevelMap previousLevelLoaded = null;
    [ReadOnly] public LevelMap nextLevelLoaded = null;
    [ReadOnly] public BaseRoom menuRoomPrefab = null;
    [ReadOnly] public BaseRoom currentRoomPrefab = null;
    [ReadOnly] public BaseRoom newRoomPrefab;
    [ReadOnly] public bool fromLoading = true;
    [ReadOnly] public bool isReplay;
    public TaskConfig TaskConfig;
    private string _country = "";
    public List<string> BannedUsers { get; set; } = new List<string>();

    public Country CountryData { get; set; } = null;
    public int[] CacheLevels { get; set; } = null;

    public Action showUpdatePopupAction;

    private void Start()
    {
        CheckCacheLevel();
    }

    public void CheckCacheLevel()
    {
        if (Utils.CurrentLevel > Config.MaxLevelCanReach)
        {
            CacheLevels = new int[Config.MaxLevelWithOutTutotial];

            for (int i = 0; i < Config.MaxLevelWithOutTutotial; i++)
            {
                CacheLevels[i] = Utils.GetCacheLevelIndex(i);
            }
        }
    }

    /// <summary>
    /// return level prefab and real level index
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <returns></returns>
    public async UniTask<(GameObject, int)> GetLevel(int levelIndex)
    {
        void MakeCacheLevel()
        {
            var tempList = new List<int>();
            for (int i = 0; i < Config.MaxLevelCanReach; i++)
            {
                if (Config.LevelSkips.Exists(i)) continue;
                tempList.Add(i);
            }
            //tempList.Shuffle();

            CacheLevels = new int[Config.MaxLevelWithOutTutotial];

            for (int i = 0; i < tempList.Count; i++)
            {
                Utils.SetCacheLevelIndex(i, tempList[i]);
                CacheLevels[i] = tempList[i];
            }
        }

        if (levelIndex > Config.MaxLevelCanReach - 1)
        {
            var temp = (levelIndex - Config.MaxLevelCanReach) % (Config.MaxLevelWithOutTutotial - 1);

            if (Data.CountPlayLevel >= Config.MaxLevelWithOutTutotial)
            {
                MakeCacheLevel();
                Data.CountPlayLevel = 0;
            }
            else
            {
                if (CacheLevels == null || CacheLevels.Length == 0 || CacheLevels.Count(_ => _ == 0) > 0 ||
                    CacheLevels.Length <= temp || CacheLevels[temp] == 0)
                {
                    MakeCacheLevel();
                }
            }

            var obj = await Addressables.LoadAssetAsync<GameObject>(string.Format(Constants.LEVEL_FORMAT,
                CacheLevels[temp] + 1));
            return (obj, CacheLevels[temp]);
        }

        var levelObject =
            await Addressables.LoadAssetAsync<GameObject>(string.Format(Constants.LEVEL_FORMAT, levelIndex + 1));

        return (levelObject, levelIndex);
    }

    public async UniTask<GameObject> GetHardLevel(int hardLevel)
    {
        return await Addressables.LoadAssetAsync<GameObject>(string.Format(Constants.HARD_LEVEL_FORMAT, hardLevel + 1));
    }

    public async UniTask<GameObject> GetRoom(int index)
    {
        return await Addressables.LoadAssetAsync<GameObject>(string.Format(Constants.ROOM_FORMAT, index + 1));
    }

    public async UniTask<GameObject> GetJigsawLevel(int index)
    {
        return await Addressables.LoadAssetAsync<GameObject>(string.Format(Constants.LEVEL_JIGSAW_FORMAT, index + 1));
    }

    public void StartDetectCountry()
    {
        if (!string.IsNullOrEmpty(Data.UserCountryCode))
        {
            return;
        }

        Observable.FromCoroutine(DetectCountry).Subscribe().AddTo(this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator DetectCountry()
    {
        var request = UnityWebRequest.Get("https://extreme-ip-lookup.com/json");
        request.chunkedTransfer = false;
        yield return request.Send();

        if (request.isNetworkError)
        {
            Debug.Log("error : " + request.error);
        }
        else
        {
            if (request.isDone)
            {
                CountryData = JsonUtility.FromJson<Country>(request.downloadHandler.text);
            }
        }
    }

    public bool IsUserBanned()
    {
        return BannedUsers.Exists(Data.UserName);
    }

    public string GetCountryCode()
    {
        if (!string.IsNullOrEmpty(Data.UserCountryCode))
        {
            return Data.UserCountryCode;
        }

        if (!string.IsNullOrEmpty(_country)) return _country;
        _country = Instance.CountryData?.countryCode;
        if (string.IsNullOrEmpty(_country) || !countryCodes.Exists(_country))
        {
            _country = "US";
        }

        Data.UserCountryCode = _country;
        return _country;
    }

    public readonly string[] countryCodes =
    {
/* "Afghanistan*/ "AF",
/* "Aland Islands*/ "AX",
/* "Albania*/ "AL",
/* "Algeria*/ "DZ",
/* "American Samoa*/ "AS",
/* "Andorra*/ "AD",
/* "Angola*/ "AO",
/* "Anguilla*/ "AI",
/* "Antigua and Barbuda*/ "AG",
/* "Argentina*/ "AR",
/* "Armenia*/ "AM",
/* "Aruba*/ "AW",
/* "Australia*/ "AU",
/* "Austria*/ "AT",
/* "Azerbaijan*/ "AZ",
/* "Bahamas*/ "BS",
/* "Bahrain*/ "BH",
/* "Bangladesh*/ "BD",
/* "Barbados*/ "BB",
/* "Belarus*/ "BY",
/* "Belgium*/ "BE",
/* "Belize*/ "BZ",
/* "Benin*/ "BJ",
/* "Bermuda*/ "BM",
/* "Bhutan*/ "BT",
/* "Bolivia, Plurinational State of*/ "BO",
/* "Bonaire, Sint Eustatius and Saba*/ "BQ",
/* "Bosnia and Herzegovina*/ "BA",
/* "Botswana*/ "BW",
/* "Brazil*/ "BR",
/* "Brunei Darussalam*/ "BN",
/* "Bulgaria*/ "BG",
/* "Burkina Faso*/ "BF",
/* "Burundi*/ "BI",
/* "Cambodia*/ "KH",
/* "Cameroon*/ "CM",
/* "Canada*/ "CA",
/* "Cape Verde*/ "CV",
/* "Cayman Islands*/ "KY",
/* "Central African Republic*/ "CF",
/* "Chad*/ "TD",
/* "Chile*/ "CL",
/* "China*/ "CN",
/* "Colombia*/ "CO",
/* "Comoros*/ "KM",
/* "Congo*/ "CG",
/* "Congo, The Democratic Republic of The*/ "CD",
/* "Cook Islands*/ "CK",
/* "Costa Rica*/ "CR",
/* "Cote D'ivoire*/ "CI",
/* "Croatia*/ "HR",
/* "Cuba*/ "CU",
/* "Curacao*/ "CW",
/* "Cyprus*/ "CY",
/* "Czech Republic*/ "CZ",
/* "Denmark*/ "DK",
/* "Djibouti*/ "DJ",
/* "Dominica*/ "DM",
/* "Dominican Republic*/ "DO",
/* "Ecuador*/ "EC",
/* "Egypt*/ "EG",
/* "El Salvador*/ "SV",
/* "Equatorial Guinea*/ "GQ",
/* "Eritrea*/ "ER",
/* "Estonia*/ "EE",
/* "Ethiopia*/ "ET",
/* "Faroe Islands*/ "FO",
/* "Fiji*/ "FJ",
/* "Finland*/ "FI",
/* "France*/ "FR",
/* "French Guiana*/ "GF",
/* "French Polynesia*/ "PF",
/* "Gabon*/ "GA",
/* "Gambia*/ "GM",
/* "Georgia*/ "GE",
/* "Germany*/ "DE",
/* "Ghana*/ "GH",
/* "Gibraltar*/ "GI",
/* "Greece*/ "GR",
/* "Greenland*/ "GL",
/* "Grenada*/ "GD",
/* "Guadeloupe*/ "GP",
/* "Guam*/ "GU",
/* "Guatemala*/ "GT",
/* "Guernsey*/ "GG",
/* "Guinea*/ "GN",
/* "Guinea-Bissau*/ "GW",
/* "Guyana*/ "GY",
/* "Haiti*/ "HT",
/* "Honduras*/ "HN",
/* "Hong Kong*/ "HK",
/* "Hungary*/ "HU",
/* "Iceland*/ "IS",
/* "India*/ "IN",
/* "Indonesia*/ "ID",
/* "Iran, Islamic Republic of*/ "IR",
/* "Iraq*/ "IQ",
/* "Ireland*/ "IE",
/* "Israel*/ "IL",
/* "Italy*/ "IT",
/* "Jamaica*/ "JM",
/* "Japan*/ "JP",
/* "Jersey*/ "JE",
/* "Jordan*/ "JO",
/* "Kazakhstan*/ "KZ",
/* "Kenya*/ "KE",
/* "Kiribati*/ "KI",
/* "Korea, Republic of*/ "KR", "XK",
/* "Kuwait*/ "KW",
/* "Kyrgyzstan*/ "KG",
/* "Lao People's Democratic Republic*/ "LA",
/* "Latvia*/ "LV",
/* "Lebanon*/ "LB",
/* "Lesotho*/ "LS",
/* "Liberia*/ "LR",
/* "Libya*/ "LY",
/* "Liechtenstein*/ "LI",
/* "Lithuania*/ "LT",
/* "Luxembourg*/ "LU",
/* "Macao*/ "MO",
/* "Macedonia, The Former Yugoslav Republic of*/ "MK",
/* "Madagascar*/ "MG",
/* "Malawi*/ "MW",
/* "Malaysia*/ "MY",
/* "Maldives*/ "MV",
/* "Mali*/ "ML",
/* "Malta*/ "MT",
/* "Marshall Islands*/ "MH",
/* "Martinique*/ "MQ",
/* "Mauritania*/ "MR",
/* "Mauritius*/ "MU",
/* "Mayotte*/ "YT",
/* "Mexico*/ "MX",
/* "Micronesia, Federated States of*/ "FM",
/* "Moldova, Republic of*/ "MD",
/* "Monaco*/ "MC",
/* "Mongolia*/ "MN",
/* "Montenegro*/ "ME",
/* "Montserrat*/ "MS",
/* "Morocco*/ "MA",
/* "Mozambique*/ "MZ",
/* "Myanmar*/ "MM",
/* "Namibia*/ "NA",
/* "Nauru*/ "NR",
/* "Nepal*/ "NP",
/* "Netherlands*/ "NL",
/* "New Caledonia*/ "NC",
/* "New Zealand*/ "NZ",
/* "Nicaragua*/ "NI",
/* "Niger*/ "NE",
/* "Nigeria*/ "NG",
/* "Niue*/ "NU",
/* "Northern Mariana Islands*/ "MP",
/* "Norway*/ "NO",
/* "Oman*/ "OM",
/* "Pakistan*/ "PK",
/* "Palau*/ "PW",
/* "Palestinian Territory, Occupied*/ "PS",
/* "Panama*/ "PA",
/* "Papua New Guinea*/ "PG",
/* "Paraguay*/ "PY",
/* "Peru*/ "PE",
/* "Philippines*/ "PH",
/* "Poland*/ "PL",
/* "Portugal*/ "PT",
/* "Puerto Rico*/ "PR",
/* "Qatar*/ "QA",
/* "Reunion*/ "RE",
/* "Romania*/ "RO",
/* "Russian Federation*/ "RU",
/* "Rwanda*/ "RW",
/* "Saint Barthelemy*/ "BL",
/* "Saint Kitts and Nevis*/ "KN",
/* "Saint Lucia*/ "LC",
/* "Saint Martin (French Part)*/ "MF",
/* "Saint Vincent and The Grenadines*/ "VC",
/* "Samoa*/ "WS",
/* "San Marino*/ "SM",
/* "Sao Tome and Principe*/ "ST",
/* "Saudi Arabia*/ "SA",
/* "Senegal*/ "SN",
/* "Serbia*/ "RS",
/* "Seychelles*/ "SC",
/* "Sierra Leone*/ "SL",
/* "Singapore*/ "SG",
/* "Sint Maarten (Dutch Part)*/ "SX",
/* "Slovakia*/ "SK",
/* "Slovenia*/ "SI",
/* "Solomon Islands*/ "SB",
/* "Somalia*/ "SO",
/* "South Africa*/ "ZA",
/* "South Sudan*/ "SS",
/* "Spain*/ "ES",
/* "Sri Lanka*/ "LK",
/* "Sudan*/ "SD",
/* "Suriname*/ "SR",
/* "Swaziland*/ "SZ",
/* "Sweden*/ "SE",
/* "Switzerland*/ "CH",
/* "Syrian Arab Republic*/ "SY",
/* "Taiwan*/ "TW",
/* "Tajikistan*/ "TJ",
/* "Tanzania, United Republic of*/ "TZ",
/* "Thailand*/ "TH",
/* "Timor-leste*/ "TL",
/* "Togo*/ "TG",
/* "Tonga*/ "TO",
/* "Trinidad and Tobago*/ "TT",
/* "Tunisia*/ "TN",
/* "Turkey*/ "TR",
/* "Turkmenistan*/ "TM",
/* "Turks and Caicos Islands*/ "TC",
/* "Tuvalu*/ "TV",
/* "Uganda*/ "UG",
/* "Ukraine*/ "UA",
/* "United Arab Emirates*/ "AE",
/* "United Kingdom*/ "GB",
/* "United States*/ "US",
/* "Uruguay*/ "UY",
/* "Uzbekistan*/ "UZ",
/* "Vanuatu*/ "VU",
/* "Venezuela, Bolivarian Republic of*/ "VE",
/* "Viet Nam*/ "VN",
/* "Virgin Islands, British*/ "VG",
/* "Virgin Islands, U.S.*/ "VI",
/* "Western Sahara*/ "EH",
/* "Yemen*/ "YE",
/* "Zambia*/ "ZM",
/* "Zimbabwe*/ "ZW",
    };

    public readonly string[] countryName =
    {
        "Afghanistan", //"AF",
        "Aland Islands", //"AX",
        "Albania", //"AL",
        "Algeria", //"DZ",
        "American Samoa", //"AS",
        "Andorra", //"AD",
        "Angola", //"AO",
        "Anguilla", //"AI",
// "Antarctica", //"AQ", 
        "Antigua and Barbuda", //"AG",
        "Argentina", //"AR",
        "Armenia", //"AM",
        "Aruba", //"AW",
        "Australia", //"AU",
        "Austria", //"AT",
        "Azerbaijan", //"AZ",
        "Bahamas", //"BS",
        "Bahrain", //"BH",
        "Bangladesh", //"BD",
        "Barbados", //"BB",
        "Belarus", //"BY",
        "Belgium", //"BE",
        "Belize", //"BZ",
        "Benin", //"BJ",
        "Bermuda", //"BM",
        "Bhutan", //"BT",
        "Bolivia, Plurinational State of", //"BO",
        "Bonaire, Sint Eustatius and Saba", //"BQ",
        "Bosnia and Herzegovina", //"BA",
        "Botswana", //"BW",
// "Bouvet Island", //"BV",
        "Brazil", //"BR",
// "British Indian Ocean Territory", //"IO",
        "Brunei Darussalam", //"BN",
        "Bulgaria", //"BG",
        "Burkina Faso", //"BF",
        "Burundi", //"BI",
        "Cambodia", //"KH",
        "Cameroon", //"CM",
        "Canada", //"CA",
        "Cape Verde", //"CV",
        "Cayman Islands", //"KY",
        "Central African Republic", //"CF",
        "Chad", //"TD",
        "Chile", //"CL",
        "China", //"CN",
// "Christmas Island", //"CX",
// "Cocos (Keeling) Islands", //"CC",
        "Colombia", //"CO",
        "Comoros", //"KM",
        "Congo", //"CG",
        "Democratic Republic of the Congo", //"CD",
        "Cook Islands", //"CK",
        "Costa Rica", //"CR",
        "Cote D'ivoire", //"CI",
        "Croatia", //"HR",
        "Cuba", //"CU",
        "Curacao", //"CW",
        "Cyprus", //"CY",
        "Czech Republic", //"CZ",
        "Denmark", //"DK",
        "Djibouti", //"DJ",
        "Dominica", //"DM",
        "Dominican Republic", //"DO",
        "Ecuador", //"EC",
        "Egypt", //"EG",
        "El Salvador", //"SV",
        "Equatorial Guinea", //"GQ",
        "Eritrea", //"ER",
        "Estonia", //"EE",
        "Ethiopia", //"ET",
// "Falkland Islands (Malvinas)", //"FK",
        "Faroe Islands", //"FO",
        "Fiji", //"FJ",
        "Finland", //"FI",
        "France", //"FR",
        "French Guiana", //"GF",
        "French Polynesia", //"PF",
// "French Southern Territories", //"TF",
        "Gabon", //"GA",
        "Gambia", //"GM",
        "Georgia", //"GE",
        "Germany", //"DE",
        "Ghana", //"GH",
        "Gibraltar", //"GI",
        "Greece", //"GR",
        "Greenland", //"GL",
        "Grenada", //"GD",
        "Guadeloupe", //"GP",
        "Guam", //"GU",
        "Guatemala", //"GT",
        "Guernsey", //"GG",
        "Guinea", //"GN",
        "Guinea-Bissau", //"GW",
        "Guyana", //"GY",
        "Haiti", //"HT",
// "Heard Island and Mcdonald Islands", //"HM",
// "Holy See (Vatican City State)", //"VA",
        "Honduras", //"HN",
        "Hong Kong", //"HK",
        "Hungary", //"HU",
        "Iceland", //"IS",
        "India", //"IN",
        "Indonesia", //"ID",
        "Islamic Republic of Iran", //"IR",
        "Iraq", //"IQ",
        "Ireland", //"IE",
// "Isle of Man", //"IM",
        "Israel", //"IL",
        "Italy", //"IT",
// "Ivory Coast", //"IV",
        "Jamaica", //"JM",
        "Japan", //"JP",
        "Jersey", //"JE",
        "Jordan", //"JO",
        "Kazakhstan", //"KZ",
        "Kenya", //"KE",
        "Kiribati", //"KI",
// "Korea, Democratic People's Republic of", //"KP",
        "Republic of Korea", //"KR",
        "Kosovo", //"XK"
        "Kuwait", //"KW",
        "Kyrgyzstan", //"KG",
        "Lao People's Democratic Republic", //"LA",
        "Latvia", //"LV",
        "Lebanon", //"LB",
        "Lesotho", //"LS",
        "Liberia", //"LR",
        "Libya", //"LY",
        "Liechtenstein", //"LI",
        "Lithuania", //"LT",
        "Luxembourg", //"LU",
        "Macao", //"MO",
        "The Former Yugoslav Republic of Macedonia", //"MK",
        "Madagascar", //"MG",
        "Malawi", //"MW",
        "Malaysia", //"MY",
        "Maldives", //"MV",
        "Mali", //"ML",
        "Malta", //"MT",
        "Marshall Islands", //"MH",
        "Martinique", //"MQ",
        "Mauritania", //"MR",
        "Mauritius", //"MU",
        "Mayotte", //"YT",
        "Mexico", //"MX",
        "Federated States of Micronesia", //"FM",
        "Republic of Moldova", //"MD",
        "Monaco", //"MC",
        "Mongolia", //"MN",
        "Montenegro", //"ME",
        "Montserrat", //"MS",
        "Morocco", //"MA",
        "Mozambique", //"MZ",
        "Myanmar", //"MM",
        "Namibia", //"NA",
        "Nauru", //"NR",
        "Nepal", //"NP",
        "Netherlands", //"NL",
        "New Caledonia", //"NC",
        "New Zealand", //"NZ",
        "Nicaragua", //"NI",
        "Niger", //"NE",
        "Nigeria", //"NG",
        "Niue", //"NU",
// "Norfolk Island", //"NF",
        "Northern Mariana Islands", //"MP",
        "Norway", //"NO",
        "Oman", //"OM",
        "Pakistan", //"PK",
        "Palau", //"PW",
        "Palestinian Territory, Occupied", //"PS",
        "Panama", //"PA",
        "Papua New Guinea", //"PG",
        "Paraguay", //"PY",
        "Peru", //"PE",
        "Philippines", //"PH",
// "Pitcairn", //"PN",
        "Poland", //"PL",
        "Portugal", //"PT",
        "Puerto Rico", //"PR",
        "Qatar", //"QA",
        "Reunion", //"RE",
        "Romania", //"RO",
        "Russian Federation", //"RU",
        "Rwanda", //"RW",
        "Saint Barthelemy", //"BL",
// "Saint Helena Ascension and Tristan Da Cunha", //"SH",
        "Saint Kitts and Nevis", //"KN",
        "Saint Lucia", //"LC",
        "Saint Martin (French Part)", //"MF",
// "Saint Pierre and Miquelon", //"PM",
        "Saint Vincent and The Grenadines", //"VC",
        "Samoa", //"WS",
        "San Marino", //"SM",
        "Sao Tome and Principe", //"ST",
        "Saudi Arabia", //"SA",
        "Senegal", //"SN",
        "Serbia", //"RS",
        "Seychelles", //"SC",
        "Sierra Leone", //"SL",
        "Singapore", //"SG",
        "Sint Maarten (Dutch Part)", //"SX",
        "Slovakia", //"SK",
        "Slovenia", //"SI",
        "Solomon Islands", //"SB",
        "Somalia", //"SO",
        "South Africa", //"ZA",
// "South Georgia and The South Sandwich Islands", //"GS",
        "South Sudan", //"SS",
        "Spain", //"ES",
        "Sri Lanka", //"LK",
        "Sudan", //"SD",
        "Suriname", //"SR",
        "Swaziland", //"SZ",
        "Sweden", //"SE",
        "Switzerland", //"CH",
        "Syrian Arab Republic", //"SY",
        "Taiwan", //"TW",
        "Tajikistan", //"TJ",
        "United Republic of Tanzania", //"TZ",
        "Thailand", //"TH",
        "Timor-leste", //"TL",
        "Togo", //"TG",
// "Tokelau", //"TK",
        "Tonga", //"TO",
        "Trinidad and Tobago", //"TT",
        "Tunisia", //"TN",
        "Turkey", //"TR",
        "Turkmenistan", //"TM",
        "Turks and Caicos Islands", //"TC",
        "Tuvalu", //"TV",
        "Uganda", //"UG",
        "Ukraine", //"UA",
        "United Arab Emirates", //"AE",
        "United Kingdom", //"GB",
        "United States", //"US",
// "United States Minor Outlying Islands", //"UM",
        "Uruguay", //"UY",
        "Uzbekistan", //"UZ",
        "Vanuatu", //"VU",
        "Bolivarian Republic of Venezuela", //"VE",
        "Viet Nam", //"VN",
        "Virgin Islands, British", //"VG",
        "Virgin Islands, U.S.", //"VI",
// "Wallis and Futuna", //"WF",
        "Western Sahara", //"EH",
        "Yemen", //"YE",
        "Zambia", //"ZM",
        "Zimbabwe", //"ZW",
    };

    public string GetCountryName(string code)
    {
        for (int i = 0; i < countryCodes.Length; i++)
        {
            if (countryCodes[i] == code)
            {
                return countryName[i];
            }
        }

        return "";
    }
}