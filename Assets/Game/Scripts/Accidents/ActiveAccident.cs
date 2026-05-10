namespace Game.Accidents
{
    public class ActiveAccident
    {
        public ActiveAccident(string instanceId, AccidentDefinition definition, string locationId)
        {
            InstanceId = instanceId;
            Definition = definition;
            LocationId = locationId;
        }

        public string InstanceId { get; }
        public AccidentDefinition Definition { get; }
        public string LocationId { get; }
        public string TypeId => Definition != null ? Definition.Id : string.Empty;
    }
}
