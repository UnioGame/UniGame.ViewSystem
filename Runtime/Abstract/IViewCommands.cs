namespace UniGame.ViewSystem.Runtime
{
    using Cysharp.Threading.Tasks;

    public interface IViewCommands
    {
        void  Destroy();
        
        IView Show();
        UniTask<IView> ShowAsync();
        void Hide();

        void  Close();
        UniTask CloseAsync();
    }
}