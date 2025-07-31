using System;

namespace Yamanas.Infrastructure.Popups
{
    public interface IPopup<T>
    {
        void SetData(T data);
    }
}