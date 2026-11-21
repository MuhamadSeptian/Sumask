using System.Collections.Generic;

public class GainShieldGA : GameAction
{
    public List<CombatantView> Targets { get; private set; }
    public int ShieldAmount { get; private set; }

    public GainShieldGA(List<CombatantView> targets, int shieldAmount)
    {
        Targets = targets;
        ShieldAmount = shieldAmount;
    }
}
