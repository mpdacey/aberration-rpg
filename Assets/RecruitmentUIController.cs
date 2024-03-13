using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecruitmentUIController : MonoBehaviour
{
    public TextMeshProUGUI leftOffer;
    public TextMeshProUGUI rightOffer;
    public Button defaultOfferButton;

    private void OnEnable()
    {
        defaultOfferButton.Select();
    }

    public void SetOffers(string leftOfferText, string rightOfferText)
    {
        leftOffer.text = leftOfferText;
        rightOffer.text = rightOfferText;
    }

    private void Update()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject == null)
            defaultOfferButton.Select();
    }
}
