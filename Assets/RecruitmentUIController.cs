using UnityEngine;
using TMPro;

public class RecruitmentUIController : MonoBehaviour
{
    public TextMeshProUGUI leftOffer;
    public TextMeshProUGUI rightOffer;

    public void SetOffers(string leftOfferText, string rightOfferText)
    {
        leftOffer.text = leftOfferText;
        rightOffer.text = rightOfferText;
    }
}
