using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Services.Interfaces;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Utilities
{
    public static class SelectListExtenstions
    {
        public static async Task<SelectList> GetComboboxSelectListItems(this IProductTypeService productTypeService, bool getNoneValue = false)
        {
            var items = await productTypeService.GetComboboxListAsync();
            if(getNoneValue)
                items.Insert(0, new Shared.Utilities.IdLabel { Id = null, Label = Resources.Resource.Label_Common_PleaseSelectItem });
            return new SelectList(items, "Id", "Label");
        }

        public static async Task<SelectList> GetComboboxSelectListItems(this ITagService tagService, bool getNoneValue = false)
        {
            var items = await tagService.GetComboboxListAsync();
            if (getNoneValue)
                items.Insert(0, new Shared.Utilities.IdLabel { Id = null, Label = Resources.Resource.Label_Common_PleaseSelectItem });
            return new SelectList(items, "Id", "Label");
        }

        public static async Task<SelectList> GetComboboxSelectListItems(this ISizeService sizeService, bool getNoneValue = false)
        {
            var items = await sizeService.GetComboboxListAsync();
            if (getNoneValue)
                items.Insert(0, new Shared.Utilities.IdLabel { Id = null, Label = Resources.Resource.Label_Common_PleaseSelectItem });
            return new SelectList(items, "Id", "Label");
        }

        //public static async Task<SelectList> GetComboboxSelectListItems(this ICategoryService categoryService, bool getNoneValue = false)
        //{
        //    var items = await categoryService.GetAssignableToProductComboboxListAsync();
        //    if (getNoneValue)
        //        items.Insert(0, new Shared.Utilities.IdLabel { Id = null, Label = Resources.Resource.Label_Common_PleaseSelectItem });
        //    return new SelectList(items, "Id", "Label");
        //}
        public static async Task<SelectList> GetComboboxSelectListItems(this IMaterialService materialService, bool getNoneValue = false)
        {
            var items = await materialService.GetComboboxListAsync();
            if (getNoneValue)
                items.Insert(0, new Shared.Utilities.IdLabel { Id = null, Label = Resources.Resource.Label_Common_PleaseSelectItem });
            return new SelectList(items, "Id", "Label");
        }
    }
}
