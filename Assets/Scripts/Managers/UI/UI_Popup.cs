namespace Marsion
{
    public class UI_Popup : UI_Base
    {
        public override void Init()
        {
            Managers.Instance.UI.SetCanvas(gameObject, true);
        }

        public virtual void ClosePopupUI()
        {
            Managers.Instance.UI.ClosePopupUI(this);
        }
    }
}