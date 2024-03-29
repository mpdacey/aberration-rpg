using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceUIController : MonoBehaviour
{
    [SerializeField] Sprite[] diceFaces;
    [SerializeField] SpriteRenderer diceFaceRenderer;
    [SerializeField] public float revealTime = 0.75f;
    [SerializeField] int numOfRolls = 4;

    public void CastFace(int chosenFace, int[] allDiceFaces)
    {
        ShowDice();
        StartCoroutine(RollFace(chosenFace, allDiceFaces));
    }

    public void UpdateFace(int newValue)
    {
        if (newValue <= 0) {
            HideDice();
            return;
        }

        diceFaceRenderer.sprite = diceFaces[newValue - 1];
    }

    private void ShowDice()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
    }

    private void HideDice() 
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    IEnumerator RollFace(int chosenFace, int[] allDiceFaces)
    {
        int randomRotationOffset = Mathf.FloorToInt(Mathf.Clamp(Random.value * 2, 0, 1.99f));
        for(int i = 0; i < numOfRolls; i++)
        {
            var randomIndex = Mathf.FloorToInt(Random.Range(1, allDiceFaces.Length) - 1);
            var selectedNumber = allDiceFaces[randomIndex];
            diceFaceRenderer.sprite = diceFaces[selectedNumber-1];
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, ((i + randomRotationOffset) % 2 == 0) ? -15 : 15));
            yield return new WaitForSeconds(revealTime / numOfRolls);
        }
        diceFaceRenderer.sprite = diceFaces[chosenFace - 1];
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
