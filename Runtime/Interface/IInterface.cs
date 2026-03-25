namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Contract for objects controlled by the Interface Timeline track.
    ///     Implement this on any MonoBehaviour you want to drive from a <see cref="InterfaceTrack"/> clip.
    ///     Each method maps to a phase of the clip lifecycle.
    /// </summary>
    public interface IInterface
    {
        /// <summary>Called once when the clip starts playing (at the very beginning).</summary>
        public void OnClipStart();

        /// <summary>Called once when ease-in is complete.</summary>
        public void OnEaseInDone();

        /// <summary>Called every frame while the clip is active.</summary>
        public void ClipActive();

        /// <summary>Called once when ease-out begins.</summary>
        public void OnEaseOutStart();

        /// <summary>Called once when the clip ends.</summary>
        public void OnClipEnd();
    }
}
