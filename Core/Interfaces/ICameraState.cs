namespace Core
{
    public interface ICameraState
    {
        CameraState TargetCameraState { get; }
        CameraState InterpolatingCameraState { get; }
    }
}