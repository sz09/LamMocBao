using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("Categories")]
    public class Category : Entity, IEntity
    {
        [StringLength(200)]
        public string Name { get; set; }
        public int HomePageSequenceNumber { get; set; }
        public int FilterSequenceNumber { get; set; }
        public string LinkName { get; set; }
        public string DisplayImageUrl { get; set; }
        public string Description { get; set; }
        public bool ShowOnHomePage { get; set; }
        public bool ShowOnFilter { get; set; }
        public bool AssignableToProduct { get; set; }
        public CategoryGroup Group { get; set; }
        public OriginalType? OriginalType { get; set; }
    }

    public enum OriginalType
    {
        [Description("Trầm hương")]
        TramHuong = 1,
        [Description("San hô")]
        SanHo = 2,
        [Description("Tử đàn")]
        TuDan = 3,
        [Description("Huyết long")]
        HuyetLong = 4
    }

    [Serializable]
    [Table("ProductCategories")]
    public class ProductCategory : Entity, IEntity
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }

    public enum CategoryGroup
    {
        [Description("Vật phẩm theo ngũ hành")]
        VatPhamTheoNguHanh = 0,
        [Description("Vật phẩm theo chất liệu")]
        VatPhamTheoChatLieu = 1,
        [Description("Vật phẩm sưu tầm")]
        VatPhamSuuTam = 2,
        [Description("Nhẫn")]
        Nhan = 3,
        [Description("Vòng cổ")]
        VongCo = 4,
        [Description("Tượng Phật")]
        TuongPhat = 5,
        [Description("Linh thú")]
        LinhThu = 6,
        [Description("Treo xe")]
        TreoXe = 7,
        [Description("Xông trầm")]
        XongTram = 8,
        [Description("Charm vàng 24K")]
        CharmVang24K = 9
    }
}
