using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Data/ContentWinJigsawData")]
public class ContentWinJigsaw : ScriptableObject
{
    public List<SetUpContent> setUpContent;
}
[Serializable]
public class SetUpContent
{
    public ETpyeContent eTpyeContent;
    public string ContentText;
}
public enum ETpyeContent
{
    Contentlevel5,
    Contentlevel10,
    Contentlevel15,
    Contentlevel20,
    Contentlevel25,
    Contentlevel30,
    Contentlevel35,
    Contentlevel40,
    Contentlevel45,
    Contentlevel50,
    Contentlevel55,
    Contentlevel60,
    Contentlevel70,
    Contentlevel75,
    Contentlevel80,
    Contentlevel85,
    Contentlevel90,
    Contentlevel95,

}
