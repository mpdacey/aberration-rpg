using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitmentController : MonoBehaviour
{
    private enum RecruitmentState
    {
        Pending,
        OfferA,
        OfferB,
        Decline
    }

    private RecruitmentState state;

    public IEnumerator PitchRecruitment(FormationScriptableObject currentFormation)
    {
        state = RecruitmentState.Pending;

        while (state == RecruitmentState.Pending)
        {
            yield return null;
        }
    }

    public void SetRecruitmentState(int value) =>
        state = (RecruitmentState)value;
}
