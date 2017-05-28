using System.Collections.Generic;
using Paladin.View;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace Paladin.Model
{
    public class BossModel
    {
        [Inject]
        public Logic Logic { get; set; }

        public ushort Stage;

        private IEventDispatcher viewDispatcher;
        private List<BossElements> stages;

        public void Init(IEventDispatcher dispatcher, List<BossElements> stages)
        {
            viewDispatcher = dispatcher;
            Stage = 0;
            this.stages = stages;
        }

        public void BossElementDestroyed()
        {
            List<BossPawn> pawns = stages[Stage].Elements;
            ushort elementsCount = (ushort)pawns.Count;
            for (int i = 0; i < pawns.Count; i++)
            {
                if (pawns[i].GetComponent<BossPawnMediator>().Model.Health <= 0)
                    elementsCount--;
            }

            if (elementsCount == 0)
            {
                Stage ++;

                if (stages.Count > Stage)
                    InitStage();
                else // все стадии пройдены
                    Destroy();
            }
        }

        public void InitStage()
        {
            viewDispatcher.Dispatch(ModelEvent.BOSS_STAGE_INIT);
        }

        private void Destroy()
        {
            viewDispatcher.Dispatch(ModelEvent.DESTROY);
        }
    }
}