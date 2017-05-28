using System.Collections.Generic;

namespace Paladin.Model
{
    public class BossConfig
    {
        public PawnGroupType Group;
        public uint Level;
        public Dictionary<PawnType, PawnConfig> Pawns;

        public BossConfig(JSONObject def)
        {
            Group = DefinitionManager.EnumValue<PawnGroupType>(def.GetField("id").str);
            Level = DefinitionManager.GetUint(def, "level");
            Pawns = new Dictionary<PawnType, PawnConfig>();
        }

        public void UpdatePawns(List<JSONObject> pawns)
        {
            foreach (var pawn in pawns)
            {
                var bossPawnType = DefinitionManager.EnumValue<PawnType>(pawn.GetField("type").str);
                pawn.SetField("group", Group.ToString());
                Pawns[bossPawnType] = new PawnConfig(pawn);
            }
        }
    }
}