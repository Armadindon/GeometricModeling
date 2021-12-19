using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    [SerializeField]
    MeshGenerator mg;

    [SerializeField]
    Text m_iterationCount;
    [SerializeField]
    Slider m_secondsBetweenIterationsSlider;
    [SerializeField]
    Text m_secondsBetweenIterations;

    bool allowCatmullClark = false;

    public static UIManager INSTANCE { get; private set; }

    private UIManager() { }

    private void Awake()
    {
        if (INSTANCE != null && INSTANCE != this)
            Destroy(gameObject);

        INSTANCE = this;
    }

    public void setIterationCount(int iteration)
    {
        m_iterationCount.text = "Itération courante : " + iteration;
    }

    public void setTimeBetweenIterations()
    {
        m_secondsBetweenIterations.text = m_secondsBetweenIterationsSlider.value + " secondes";
    }

    public void cubeButtonClicked()
    {
        allowCatmullClark = true;
        mg.CreateCube();
    }

    public void chipsButtonClicked()
    {
        allowCatmullClark = true;
        mg.CreateChips();
    }

    public void polygonButtonClicked()
    {
        allowCatmullClark = true;
        mg.CreateRegularPolygon();
    }

    public void StartCatmullClark()
    {
        if (!allowCatmullClark) return;
        allowCatmullClark = false;
        StartCoroutine(mg.CoroutineCatmullClark(m_secondsBetweenIterationsSlider.value));
    }

    public void changeScene()
    {
        SceneManager.LoadScene(0);
    }

}
