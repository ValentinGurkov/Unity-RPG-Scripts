namespace RPG.Saving
{
    /// <summary>
    /// Implemented by every component that desires to save its' state.
    /// </summary>
    public interface ISaveable
    {
        object CaptureState();
        void RestoreState(object state);
    }
}