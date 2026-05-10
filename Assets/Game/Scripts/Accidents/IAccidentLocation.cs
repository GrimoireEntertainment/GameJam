namespace Game.Accidents
{
    public interface IAccidentLocation
    {
        string AccidentTypeId { get; }
        string LocationId { get; }
        string ActiveInstanceId { get; }
        bool IsActive { get; }

        bool Matches(string accidentTypeId, string locationId);
        void Activate(ActiveAccident accident);
        void Deactivate(ActiveAccident accident);
    }
}
