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
    [SerializeField] Image _progressSlider;
    [SerializeField] Text _numberResources;
    [SerializeField] Image _progressIcon;
    [SerializeField] Sprite _iconResources;
    [SerializeField] Sprite _iconSoldiers;

    float _waitTime = 2f;
    float _explanationTime = 4f;

	private void Start()
	{
        EventManager.StartListening<BasicEvent>("OnNewBuilding", OnNewBuilding);
        EventManager.StartListening<BasicEvent>("OnProgressBuilding", OnProgressBuilding);
        EventManager.StartListening<BasicEvent>("OnNewExplanation", OnNewExplanation);
        EventManager.StartListening<BasicEvent>("OnProgressExtract", OnProgressExtract);
        EventManager.StartListening<BasicEvent>("OnProgressSoldier", OnProgressSomething);
        EventManager.StartListening<BasicEvent>("OnNewResources", OnNewResources);
        EventManager.StartListening<BasicEvent>("OnProgressChange", OnProgressChange);

	}

	private void OnDestroy()
	{
        EventManager.StopListening<BasicEvent>("OnNewBuilding", OnNewBuilding);
        EventManager.StopListening<BasicEvent>("OnProgressBuilding", OnProgressBuilding);
        EventManager.StopListening<BasicEvent>("OnNewExplanation", OnNewExplanation);
        EventManager.StopListening<BasicEvent>("OnProgressExtract", OnProgressExtract);
        EventManager.StopListening<BasicEvent>("OnProgressSoldier", OnProgressSomething);
        EventManager.StopListening<BasicEvent>("OnNewResources", OnNewResources);
        EventManager.StopListening<BasicEvent>("OnProgressChange", OnProgressChange);
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

    void OnProgressSomething(BasicEvent e)
    {
        ProgressSlider((float)e.Data);
    }

    void OnProgressExtract(BasicEvent e)
    {
        ProgressSlider((float)e.Data);
    }

    void ProgressSlider(float f)
    {
        _progressSlider.fillAmount = f;
    }

    void OnProgressChange(BasicEvent e)
    {
        _progressSlider.fillAmount = 0f;
        if (e.Data.ToString() == Building.SOLDIER)
        {
            _numberResources.enabled = false;
            _progressIcon.sprite = _iconSoldiers;
        }
        else
        {
            _numberResources.enabled = true;
            _progressIcon.sprite = _iconResources;
        }
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

    IEnumerator ResetBuildingsSlider()
    {
        yield return Yielders.Get(_waitTime);
        _buildingsSlider.fillAmount = 0f;
    }

    IEnumerator ResetResourcesSlider()
    {
        yield return Yielders.Get(_waitTime);
        _progressSlider.fillAmount = 0f;
    }

    IEnumerator ShowNewExplanation(string explanation)
    {
        _bottomBlock.SetActive(true);
        _explanation.text = explanation;
        yield return Yielders.Get(_explanationTime);
        _bottomBlock.SetActive(false);
    }

}
