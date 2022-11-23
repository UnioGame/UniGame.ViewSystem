namespace UniGame.ViewSystem.Runtime
{
    public interface IViewCommands
    {
        void  Destroy();
        void  Close();
        IView Show();
        void Hide();
    }
}