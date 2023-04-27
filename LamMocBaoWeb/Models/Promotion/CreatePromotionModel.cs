namespace LamMocBaoWeb.Models
{
    public class CreatePromotionModel: PromotionModel
    {
        public CreatePromotionModel()
        {
            IsActive = true;
            PromotionMode = PromotionMode.Manual;
        }
    }
}
