using System;

namespace NahaAuto.Code
{
    public interface ITaskRunner<T>
    {
        void DoTask(T model);

        void Process(Action<Status> process);
    }
}