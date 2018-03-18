using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour 
{
    [SerializeField] Text _numberBuilds;
    [SerializeField] Image _buildingsSlider;
    [SerializeField] GameObject _bottomBlock;
    [SerializeField] Text _explanation;
    [SerializeField] Image _extractSlider;
    [SerializeField] Text _numberResources;

    float _waitTime = 2f;
    float _explanationTime = 4f;

	private void Start()
	{
        EventManager.StartListening<BasicEvent>("OnNewBuilding", OnNewBuilding);
        EventManager.StartListening<BasicEvent>("OnProgressBuilding", OnProgressBuilding);
        EventManager.StartListening<BasicEvent>("OnNewExplanation", OnNewExplanation);
        EventManager.StartListening<BasicEvent>("OnProgressExtract", OnProgressExtract);
        EventManager.StartListening<BasicEvent>("OnNewResources", OnNewResources);
	}

	private void OnDestroy()
	{
        EventManager.StopListening<BasicEvent>("OnNewBuilding", OnNewBuilding);
        EventManager.StopListening<BasicEvent>("OnProgressBuilding", OnProgressBuilding);
        EventManager.StopListening<BasicEvent>("OnNewExplanation", OnNewExplanation);
        EventManager.StartListening<BasicEvent>("OnProgressExtract", OnProgressExtract);
        EventManager.StartListening<BasicEvent>("OnNewResources", OnNewResources);
	}

	void OnNewBuilding(BasicEvent e)
    {
        int currentBuildings = (int)e.Data;
        _numberBuilds.text = currentBuildings.ToString();
        StartCoroutine(ResetBuildingsSlider());
    }

    void OnNewResources(BasicEvent e)
    {
        int currentResources = (int)e.Data;
        _numberResources.text = currentResources.ToString();
        StartCoroutine(ResetResourcesSlider());       
    }

    void OnProgressBuilding(BasicEvent e)
    {
        _buildingsSlider.fillAmount = (float)e.Data;        
    }

    void OnProgressExtract(BasicEvent e)
    {
        _extractSlider.fillAmount = (float)e.Data;
    }

    IEnumerator ResetBuildingsSlider()
    {
        yield return Yielders.Get(_waitTime);
        _buildingsSlider.fillAmount = 0f;
    }

    IEnumerator ResetResourcesSlider()
    {
        yield return Yielders.Get(_waitTime);
        _extractSlider.fillAmount = 0f;
    }

    void OnNewExplanation(BasicEvent e)
    {
        var explanation = e.Data.ToString();
        if (!string.IsNullOrEmpty(explanation))
        {
            StartCoroutine(ShowNewExplanation(explanation));
        }
        else
        {
            _explanation.text = "";
            _bottomBlock.SetActive(false); 
        }
    }

    IEnumerator ShowNewExplanation(string explanation)
    {
        _bottomBlock.SetActive(true);
        _explanation.text = explanation;
        yield return Yielders.Get(_explanationTime);
        _bottomBlock.SetActive(false);
    }

}
