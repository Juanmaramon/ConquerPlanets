using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour 
{
    [SerializeField] Text _numberBuilds;
    [SerializeField] Image _buildingsSlider;

    float sliderResetTime = 2f;

	private void Start()
	{
        EventManager.StartListening<BasicEvent>("OnNewBuilding", OnNewBuilding);
        EventManager.StartListening<BasicEvent>("OnProgressBuilding", OnProgressBuilding);
	}

    void OnNewBuilding(BasicEvent e)
    {
        int currentBuildings = (int)e.Data;
        _numberBuilds.text = currentBuildings.ToString();
        StartCoroutine(ResetBuildingsSlider());
    }

    void OnProgressBuilding(BasicEvent e)
    {
        _buildingsSlider.fillAmount = (float)e.Data;        
    }

    IEnumerator ResetBuildingsSlider()
    {
        yield return Yielders.Get(sliderResetTime);
        _buildingsSlider.fillAmount = 0f;
    }
}
