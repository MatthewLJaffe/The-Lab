namespace PlayerScripts
{
    public interface IInteractable
    {
        bool CanInteract { get; set; }
        void Interact();
    }
}