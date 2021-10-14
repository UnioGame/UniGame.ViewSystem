namespace UniModules.UniGame.UISystem.Runtime.Abstract
{
    public interface IViewCommands
    {
        void  Destroy();
        void  Close();
        IView Show();
        IView Hide();
    }
}