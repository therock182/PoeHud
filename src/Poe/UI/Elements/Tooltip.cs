namespace PoeHUD.Poe.UI.Elements
{
    public class Tooltip : Element
    {
        public Element Contents
        {
            get
            {
                return base.GetChildFromIndices(new[]
                {
                    0,
                    1
                });
            }
        }
    }
}