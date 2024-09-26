using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    private int currentStep;
    [SerializeField] private RangerBackground rangerCheck;
    [SerializeField] private TextMeshProUGUI currentTutorialText;

    [SerializeField] private GameObject store, tech, village;
    private bool hasOpenedStore, hasOpenedTech, hasOpenedVillage;
   
    [System.Serializable]
    public struct TutorialStep
    {
        [TextArea(15, 20)]
        public string currentText;
        public UnityEvent unityEvents;
    }

    [SerializeField] private TutorialStep[] steps;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("TutorialCompleted") == 1)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            #region checks
            if (currentStep == 1)
            {
                if (!rangerCheck.ranger)
                {
                    return;
                }
            }

            if (currentStep == 3)
            {
                if (!hasOpenedStore)
                {
                    return;
                }

                if (store.activeInHierarchy)
                {
                    return;
                }
            }

            if (currentStep == 4)
            {
                if (!hasOpenedTech)
                {
                    return;
                }

                if (tech.activeInHierarchy)
                {
                    return;
                }
            }

            if (currentStep == 5)
            {
                if (!hasOpenedVillage)
                {
                    return;
                }

                if (village.activeInHierarchy)
                {
                    return;
                }
            }

            #endregion checks 
            //^Lot of checks so you can't skip tutorial

            if (currentStep >= steps.Length - 1)
            {
                gameObject.SetActive(false);
                PlayerPrefs.SetInt("TutorialCompleted", 1);
                return;
            }

            currentStep++;

            currentTutorialText.text = steps[currentStep].currentText;
            steps[currentStep].unityEvents.Invoke();
        }


        if (store.activeInHierarchy && !hasOpenedStore)
        {
            hasOpenedStore = true;
        }

        if (tech.activeInHierarchy && !hasOpenedTech)
        {
            hasOpenedTech = true;
        }

        if (village.activeInHierarchy && !hasOpenedVillage)
        {
            hasOpenedVillage = true;
        }
    }
}