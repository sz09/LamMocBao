namespace LamMocBaoWeb.ViewModels
{
    public class CommonDataTransfer
    {
        public object Data { get; set; }
        public T To<T>()
        {
            return (T)Data;
        }
    }
}
