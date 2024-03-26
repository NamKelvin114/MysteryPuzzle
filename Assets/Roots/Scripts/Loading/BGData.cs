using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BG Data")]
public class BGData : ScriptableObject
{
    public List<BgData> listBGData;
}

[Serializable]
public class BgData
{
    public MapType MapType;
    public List<Sprite> listSpriteBg;
}
