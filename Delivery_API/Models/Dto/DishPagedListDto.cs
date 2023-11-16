namespace Delivery_API.Models.Dto
{
    public class DishPagedListDto
    {
        public List<DishDto> Dishes { get; set; }

        public PageInfoModel Pagination { get; set; }
    }
}
