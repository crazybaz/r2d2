namespace Paladin.Controller.Ability
{
    public class Bankir : AbilityBase
    {
        private float multiplier;

        public override void Prepare()
        {
            multiplier = GetOption<float>("multiplier");
        }

        public override void Activate()
        {
            //MasterPattern.COIN_PRICE_MODIFIER = GetOption<float>("multiplier");
            Paladin.Model.BuffModule.CoinMultiplier += multiplier;
        }

        /*public override void Deactivate()
        {
            //MasterPattern.COIN_PRICE_MODIFIER = 1.0f;
            Paladin.Model.BuffModule.CoinMultiplier -= multiplier;
        }*/
    }
}