using System;

namespace SOSXR.TimelineExtensions
{
    /// <summary>
    ///     Behaviour data marker for the Interface track. Carries no extra data — the lifecycle callbacks are dispatched
    ///     directly by <see cref="InterfaceMixer"/> to the bound <see cref="IInterface"/> implementation.
    /// </summary>
    [Serializable]
    public class InterfaceBehaviour : Behaviour
    {
    }
}
