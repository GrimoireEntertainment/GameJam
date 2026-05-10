namespace Game.Interaction
{
    public interface IInteractable
    {
        void SetInteractActive(bool isActive);
        
        void Interact(PlayerInteractor interactor);
    }
}
