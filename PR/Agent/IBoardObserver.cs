using System.Threading.Tasks;

namespace Agent
{
    public interface IBoardObserver
    {
        Task BoardUpdated();
    }
}