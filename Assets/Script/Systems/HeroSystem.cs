using System.Collections;
using UnityEngine;

public class HeroSystem : Singleton<HeroSystem>
{
    [field: SerializeField] public HeroView HeroView { get; private set; }

    void OnEnable()
    {
        ActionSystem.AttachPerformer<RemoveAllShieldsGA>(RemoveAllShieldsPerformer);
        ActionSystem.AttachPerformer<GainShieldGA>(GainShieldPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<RemoveAllShieldsGA>(RemoveAllShieldsPerformer);
        ActionSystem.DetachPerformer<GainShieldGA>(GainShieldPerformer);
    }

    public void Setup(HeroData heroData)
    {
        HeroView.Setup(heroData);
    }

    private IEnumerator RemoveAllShieldsPerformer(RemoveAllShieldsGA ga)
    {
        HeroView.RemoveAllShield();
        yield break;
    }

    private IEnumerator GainShieldPerformer(GainShieldGA ga)
    {
        foreach (var target in ga.Targets)
        {
            if (target != null)
            {
                target.GainShield(ga.ShieldAmount);
            }
        }
        yield break;
    }
}
