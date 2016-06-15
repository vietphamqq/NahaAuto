namespace NahaAuto.Code
{
    public interface ITaskRunner<T>
    {
        void DoTask(T model);
    }
}