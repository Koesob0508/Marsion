namespace Marsion
{
    public abstract class UI_Popup : UI_Base
    {
        public override void Init()
        {

        }

        public virtual void ClosePopupUI()
        {
            Close();
        }
    }
}