using System;

public struct CheckResult
{
    public bool PaladinType;
    public bool Level;
    public bool PaladinUpgrades;
}

public class ReqsChecker
{
    [Inject]
    public Logic Logic { get; set; }

    public bool FirstInclusionCheck(Reqs reqs)
    {
        if (reqs.PaladinType != null)
            if (!Logic.HasPaladin(reqs.PaladinType.Value))
                return false;

        if (reqs.Level > 0)
            if (Logic.Level < reqs.Level)
                return false;

        if (reqs.PaladinUpgrades > 0)
        {
            if (reqs.PaladinType == null)
                throw new Exception("You ask to FirstInclusionCheck PaladinUpgrades, but you did't select PaladinType");

            if (Logic.GetPaladin(reqs.PaladinType.Value).Upgrades[NodeType.Head]
              + Logic.GetPaladin(reqs.PaladinType.Value).Upgrades[NodeType.Chassis]
              + Logic.GetPaladin(reqs.PaladinType.Value).Upgrades[NodeType.Frame]
              < reqs.PaladinUpgrades)
            {
                return false;
            }
        }

        return true;
    }

    public CheckResult FullCheck(Reqs reqs)
    {
        CheckResult result = new CheckResult();
        if (reqs.PaladinType != null)
            result.PaladinType = Logic.HasPaladin(reqs.PaladinType.Value);

        if (reqs.Level > 0)
            result.Level = Logic.Level >= reqs.Level;

        if (reqs.PaladinUpgrades > 0)
        {
            if (reqs.PaladinType == null)
                throw new Exception("You ask to FirstInclusionCheck PaladinUpgrades, but you did't select PaladinType");

            if (result.PaladinType)
            {
                result.PaladinUpgrades = Logic.GetPaladin(reqs.PaladinType.Value).Upgrades[NodeType.Head]
                                         + Logic.GetPaladin(reqs.PaladinType.Value).Upgrades[NodeType.Chassis]
                                         + Logic.GetPaladin(reqs.PaladinType.Value).Upgrades[NodeType.Frame]
                                         >= reqs.PaladinUpgrades;
            }
            else result.PaladinUpgrades = false;
        }

        return result;
    }
}

