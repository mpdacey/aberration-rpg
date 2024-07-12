using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cryptemental.Manual
{
    public class ManualUIController : MonoBehaviour
    {
        public Transform pageMarkerColumn;

        {
        }

        {

        }

        public void UpdatePageMarker(int siblingIndex, ManualController.ManualPage manualPage)
        {
            Transform child = pageMarkerColumn.GetChild(siblingIndex);
            child.GetComponentInChildren<TMP_Text>().text = manualPage.name;
            child.GetComponentInChildren<Image>().sprite = manualPage.content;
        }
    }
}