namespace Laba_1
{
    public interface ISaveList<T>
    {
        T Load(string path);
        void Save(T data, string path);
    }
}