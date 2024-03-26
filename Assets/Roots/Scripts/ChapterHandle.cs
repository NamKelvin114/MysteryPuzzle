using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterHandle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtNameChapter;
    [SerializeField] private GameObject[] locks;
    [SerializeField] private GameObject[] current;
    [SerializeField] private GameObject[] pass;
    [SerializeField] private TextMeshProUGUI[] levels;
    [SerializeField] private Button[] buttons;

    [Space] [SerializeField] private int startLevel;
    
    public TextMeshProUGUI TxtNameChapter => txtNameChapter;
    public int StartLevel { get => startLevel; set => startLevel = value; }

    public void Initialize()
    {
        Refresh();

        for (int i = 0; i < buttons.Length; i++)
        {
            var j = i;
            buttons[j].onClick.RemoveAllListeners();
            buttons[j].onClick.AddListener(() => OnSelectLevelPressed(startLevel + j * Config.LevelFragment - 1));
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            var level = startLevel + i * Config.LevelFragment;

            if (Utils.MaxLevel + 1 >= level)
            {
                levels[i].SetText($"{level}");
                buttons[i].interactable = true;
                locks[i].SetActive(false);
                levels[i].gameObject.SetActive(true);
                if (Utils.CurrentLevel + 1 < level + Config.LevelFragment && Utils.CurrentLevel + 1 >= level)
                {
                    current[i].SetActive(true);
                    pass[i].SetActive(false);
                }
                else
                {
                    current[i].SetActive(false);
                    pass[i].SetActive(true);
                }
            }
            else
            {
                locks[i].SetActive(true);
                buttons[i].interactable = false;
            }
        }
    }

    private void OnSelectLevelPressed(int level)
    {
        Utils.CurrentLevel = level;
        GamePopup.Instance.Hide();
        MenuController.instance.SoundClickButton();
        //startLevel
        MenuController.instance.InternalNextLevel();
        
    }
}