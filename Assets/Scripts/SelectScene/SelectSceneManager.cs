using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectSceneManager : MonoBehaviour
{
    [SerializeField] private ItemSelect[] listItemSelect;

    private void Awake()
    {
        for(int i = 0; i<SaveLoadData.Instance.listLevels.listLevelDetails.Count; i++)
        {
            listItemSelect[i].SetLock(SaveLoadData.Instance.listLevels.listLevelDetails[i].isLock, SaveLoadData.Instance.listLevels.listLevelDetails[i].levelId);
        }
    }

    public void OnBack()
    {
        SceneManager.LoadScene("TopScene");
    }
}
