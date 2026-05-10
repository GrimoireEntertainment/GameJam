using Game.Interaction;

public interface IHoldInteractable
{
    float HoldDuration { get; }

    bool CanHoldInteract(PlayerInteractor interactor);

    void StartHold(PlayerInteractor interactor);

    void ProcessHold(PlayerInteractor interactor, float progress);

    void CompleteHold(PlayerInteractor interactor);

    void CancelHold(PlayerInteractor interactor);
}