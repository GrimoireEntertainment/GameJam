using System;

namespace Game.Accidents
{
    [Serializable]
    public class LevelAccidentScheduleEntry
    {
        public AccidentDefinition Accident;
        public float TriggerTime;
        public string LocationId;
    }
}
