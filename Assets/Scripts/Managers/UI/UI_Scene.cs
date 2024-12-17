namespace Marsion
{
    public class UI_Scene : UI_Base
    {
        public override void Init()
        {
            Managers.Instance.UI.SetCanvas(gameObject, false);
        }
    }
}