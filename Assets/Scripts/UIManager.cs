using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    TextMeshProUGUI PressSpaceToStart;
    TextMeshProUGUI Time;
    TextMeshProUGUI Gamma;
    TextMeshProUGUI RemainObserve;
    TextMeshProUGUI Goal;

    // Start is called before the first frame update
    void Start()
    {
        PressSpaceToStart = GameObject.Find("PressSpaceToStart").GetComponent<TextMeshProUGUI>();
        Time = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();
        Gamma = GameObject.Find("Gamma").GetComponent<TextMeshProUGUI>();
        RemainObserve = GameObject.Find("RemainObserve").GetComponent<TextMeshProUGUI>();
        Goal = GameObject.Find("GoalMessage").GetComponent<TextMeshProUGUI>();
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Initialize()
    {
        ShowPressSpaceToStart();
        HideFinishMessage();
        HideRemainObserve();
    }

    public void UpdateTime()
    {
        Time.text = "Time: " + GameManager.CurrentTime.ToString("#.###");
    }
    public void UpdateGamma()
    {
        Gamma.text = "Gamma: " + Player.Gamma.ToString("#.###");
    }

    public void HideFinishMessage()
    {
        Goal.text = "";
    }

    public void ShowFinishMessage()
    {
        if (Player.CurrentState == Player.State.Goal) Goal.text = "Goal!";
        else if (Player.CurrentState == Player.State.GameOver) Goal.text = "Failed...";
    }

    public void HideRemainObserve()
    {
        RemainObserve.text = "";
    }

    public void ShowRemainObserve()
    {
        RemainObserve.text = "Observe: " + Player.RemainObserve.ToString();
    }

    public void ShowPressSpaceToStart()
    {
        // PressSpaceToStart.text = "Press Space To Start";
        PressSpaceToStart.text = "";
    }

    public void HidePressSpaceToStart()
    {
        PressSpaceToStart.text = "";
    }
}
