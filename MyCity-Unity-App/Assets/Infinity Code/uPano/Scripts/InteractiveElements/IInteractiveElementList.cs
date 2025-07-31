/*           INFINITY CODE           */
/*     https://infinity-code.com     */

namespace InfinityCode.uPano.InteractiveElements
{
    public interface IInteractiveElementList
    {
        int Count { get; }

        InteractiveElement GetItemAt(int index);
    }
}