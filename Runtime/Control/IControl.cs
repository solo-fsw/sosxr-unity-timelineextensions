namespace SOSXR.TimelineExtensions
{
    public interface IControl
    {
        void OnClipStart();


        void OnEaseInDone();


        void ClipActive();


        void OnEaseOutStart();


        void OnClipEnd();
    }
}