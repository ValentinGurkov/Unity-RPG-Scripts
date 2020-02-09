namespace RPG.Util {
    /// <summary>
    /// Classes that need their properties reset when the scene ends implement this.
    /// </summary>
    public interface IUnloadedSceneHandler {
        void OnSceneUnloaded();
    }
}
