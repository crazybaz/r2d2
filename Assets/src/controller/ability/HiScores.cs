namespace Paladin.Controller.Ability
{
    public class HiScores : AbilityBase
    {
        private float multiplier;

        public override void Prepare()
        {
            multiplier = GetOption<float>("multiplier");
        }

        public override void Activate()
        {
            //GameController.ScoreMultiplier = GetOption<float>("multiplier");
            Paladin.Model.BuffModule.ScoreMultiplier += multiplier;
        }

        /*public override void Deactivate() // TODO: имплементить
        {
            Paladin.Model.BuffModule.ScoreMultiplier -= multiplier;
            //GameController.ScoreMultiplier = 1;
        }*/
    }
}