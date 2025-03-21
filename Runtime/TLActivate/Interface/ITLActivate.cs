using UnityEngine;


namespace SOSXR.TimelineExtensions
{
    public interface ITLActivate
    {
        [ContextMenu(nameof(TLActivate))]
        void TLActivate();
    }
}