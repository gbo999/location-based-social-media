using RSG;

namespace Yamanas.Infrastructure.Popups
{
    public interface IPromisePopup<T>
    {
      void  SetPromise(Promise<T> dataPromise);
        
    }
}