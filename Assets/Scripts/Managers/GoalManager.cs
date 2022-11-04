using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public int goalsNeeded;
    public int goalsAchieved;
    public Sprite goalSprite;
    public string matchValue;
}

public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;

    private void Start()
    {
        SetupGoals();
    }

    private void SetupGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform, false);
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.sprite = levelGoals[i].goalSprite;
            panel.str = $"0/{levelGoals[i].goalsNeeded}";

            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform, false);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.sprite = levelGoals[i].goalSprite;
            panel.str = $"0/{levelGoals[i].goalsNeeded}";
        }
    }

    public void UpdateGoals()
    {
        int goalsCompleted = 0;
        for (int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].text.text = $"{levelGoals[i].goalsAchieved} / {levelGoals[i].goalsNeeded}";
            if (levelGoals[i].goalsAchieved >= levelGoals[i].goalsNeeded)
            {
                goalsCompleted++;
                currentGoals[i].text.text = $"{levelGoals[i].goalsNeeded} / {levelGoals[i].goalsNeeded}";
            }
        }

        if (goalsCompleted >= levelGoals.Length)
            Debug.Log("Win");
    }

    public void CompareGoal(string goalToCompare)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (goalToCompare == levelGoals[i].matchValue)
                levelGoals[i].goalsAchieved++;
        }
    }
}
