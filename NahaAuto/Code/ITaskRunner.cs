using System;
using System.Threading.Tasks;

namespace NahaAuto.Code
{
    public interface ITaskRunner<T>
    {
        Task DoTask(T model);

        void Process(Action<Status> process);
    }
}