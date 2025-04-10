namespace SOSXR.TimelineExtensions
{
    public interface IInterface
    {
        void OnClipStart();


        void OnEaseInDone();


        void ClipActive();


        void OnEaseOutStart();


        void OnClipEnd();
    }
}