using AutoMapper;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.Models.Appointment;
using LamMocBaoWeb.Models.Products;
using LamMocBaoWeb.ViewModels;
using Services.Services;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LamMocBao.Mappers
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            WriteToRead();
            ReadToWrite();
        }

        void WriteToRead()
        {
            CreateMap<Shared.Models.ProductImage, ProductImageViewModel>();
            CreateMap<Shared.Models.Product, EditProductViewModel>()
                .ForMember(d => d.Images, e => e.MapFrom(x => x.ProductImages))
                .ForMember(d => d.Infomations, e => e.MapFrom(x => string.Join(Environment.NewLine, StringUtilities.Decombined(x.FormattedInfomations))))
                .ForMember(d => d.SizeIds, e => e.MapFrom(x => x.ProductSizes.Select(y => y.SizeId)))
                .ForMember(d => d.TagIds, e => e.MapFrom(x => x.ProductTags.Where(d => !d.ProductTypeId.HasValue).Select(y => y.TagId)))
                .ForMember(d => d.CategoryIds, e => e.MapFrom(x => x.ProductCategories.Select(y => y.CategoryId)))
                .ForMember(d => d.ProductTypeTagIds, e => e.MapFrom(x => x.ProductTags.Where(d => d.ProductTypeId.HasValue).Select(y => y.TagId)))
                .ForMember(d => d.MaterialIds, e => e.MapFrom(x => x.ProductMaterials.Select(y => y.MaterialId)))
                .ForMember(d => d.SellingPrice, e => e.MapFrom(x => x.SellingPrice.ToString().Replace(",00", string.Empty)))
                .ForMember(d => d.PurchasingPrice, e => e.MapFrom(x => x.PurchasingPrice.ToString().Replace(",00", string.Empty)))
                .ForMember(d => d.PriceBySizes, e => e.MapFrom(x => x.ProductSizes.Select(d => new { d.SizeId, d.SellingPrice })
                                                                                  .ToDictionary(d => d.SizeId, e => e.SellingPrice)))
                .ReverseMap();
            CreateMap<UpdateProductModel, EditProductViewModel>()
                .ForMember(d => d.ProductTypeTagIds, e => e.MapFrom(x => x.ProductTypeTagIds.Values))
                .ForMember(d => d.TagIds, e => e.MapFrom(x => x.TagIds.Values))
                .ForMember(d => d.SizeIds, e => e.MapFrom(x => x.SizeIds.Values))
                .ForMember(d => d.MaterialIds, e => e.MapFrom(x => x.MaterialIds.Values));

            CreateMap<Shared.Models.ProductType, EditProductTypeViewModel>()
                .ForMember(d => d.TagIds, e => e.MapFrom(s => s.ProductTypeTags.Select(d => d.TagId)))
                .ForMember(d => d.TagIds, e => e.MapFrom(s => s.ProductTypeTags
                                                               .Select(d => KeyValuePair.Create(d.SequenceNumber, d.TagId))));
                //.ForMember(d => d.ProductTypeTagStrs, e => e.MapFrom(s => string.Join(", ",  s.ProductTypeTags.Select(r => r.Tag.Name))));
            CreateMap<Shared.Models.ProductType, ProductTypeViewModel>();
            CreateMap<UpdateProductTypeModel, EditProductTypeViewModel>();

            CreateMap<Shared.Models.ProductSubType, EditProductSubTypeViewModel>();
            CreateMap<Shared.Models.ProductSubType, ProductSubTypeViewModel>();
            CreateMap<UpdateProductTypeModel, EditProductSubTypeViewModel>();

            CreateMap<Shared.Models.Category, EditCategoryViewModel>();
            CreateMap<Shared.Models.Category, CategoryViewModel>();
            CreateMap<UpdateCategoryModel, EditCategoryViewModel>();

            CreateMap<Shared.Models.Tag, EditTagViewModel>();
            CreateMap<UpdateTagModel, EditTagViewModel>();

            CreateMap<Shared.Models.PromotionMode, PromotionMode>();
            CreateMap<Shared.Models.Promotion, EditPromotionViewModel>();
            CreateMap<UpdatePromotionModel, EditPromotionViewModel>();

            CreateMap<Shared.Models.Size, EditSizeViewModel>();
            CreateMap<UpdateSizeModel, EditSizeViewModel>();
            CreateMap<Shared.Models.Appointment, ViewAppointmentViewModel>();

            CreateMap<Shared.Models.PaymentMethod, PaymentMethod>();
            CreateMap<Shared.Models.OrderStatus, OrderStatus>();
            CreateMap<Shared.Models.Customer, CustomerViewModel>();

            CreateMap<Shared.Models.Order, ViewOrderViewModel>()
                .ForMember(d => d.OrderStatus, e => e.MapFrom(s => s.Status));
            CreateMap<Shared.Models.OrderDetail, OrderDetailModel>()
                .ForMember(s => s.Name, d => d.MapFrom(s => s.Product.Name))
                .ForMember(s => s.SizeUnit, e => e.MapFrom(r => r.ProductSize.Size.Unit))
                .ForMember(s => s.SizeNumber, e => e.MapFrom(r => r.ProductSize.Size.Number))
                .ForMember(s => s.Price, e => e.MapFrom(r => r.ProductSize.SellingPrice ?? r.Product.SellingPrice ?? 0));
            CreateMap<Shared.Models.DeliveryAddress, DeliveryAddressViewModel>();


            CreateMap<Shared.Models.Material, EditMaterialViewModel>();
            CreateMap<UpdateMaterialModel, EditMaterialViewModel>();
            CreateMap<Shared.Models.Material, MaterialViewModel>();

            CreateMap<Shared.Models.NewsPaperPost, EditNewsPaperPostViewModel>()
                .ForMember(d => d.ImagePreview, d => d.MapFrom(e => e.UploadedImage.Url));
            CreateMap<UpdateNewsPaperPostModel, EditNewsPaperPostViewModel>();
            
            CreateMap<Shared.Models.Knowledge, EditKnowledgeViewModel>()
                .ForMember(d => d.ImagePreview, d => d.MapFrom(e => e.UploadedImage.Url));
            CreateMap<Shared.Models.Knowledge, KnowledgeViewModel>()
                .ForMember(d => d.ImagePreviews, d => d.MapFrom(e => e.UploadedImage.UrlPreview));
            CreateMap<UpdateKnowledgeModel, EditKnowledgeViewModel>();

            CreateMap<Shared.Models.PublishedKnowledge, EditKnowledgeViewModel>()
                .ForMember(d => d.ImagePreview, d => d.MapFrom(e => e.UploadedImage.Url));
            CreateMap<Shared.Models.PublishedKnowledge, KnowledgeViewModel>()
                .ForMember(d => d.ImagePreviews, d => d.MapFrom(e => e.UploadedImage.UrlPreview));
            CreateMap<UpdateKnowledgeModel, EditKnowledgeViewModel>();
            
            CreateMap<Shared.Models.CustomerComment, EditCustomerCommentViewModel>()
                .ForMember(d => d.ImagePreview, d => d.MapFrom(e => e.UploadedImage.Url));
            CreateMap<UpdateCustomerCommentModel, EditCustomerCommentViewModel>();

            CreateMap<Shared.Models.SystemSetting, EditSystemSettingViewModel>();
            CreateMap<UpdateSystemSettingModel, EditSystemSettingViewModel>();

            CreateMap<Shared.Models.Product, Product>()
                .ForMember(d => d.ImageUrls, e => e.MapFrom(x => x.ProductImages.Select(d => d.Url)))
                .ForMember(d => d.Images, e => e.MapFrom(x => x.ProductImages))
                .ForMember(d => d.ShortInfomations, e => e.MapFrom(x => StringUtilities.Decombined(x.FormattedInfomations)))
                .ReverseMap()
                .ForMember(d => d.FormattedInfomations, e => e.MapFrom(x => StringUtilities.Combined(x.ShortInfomations)));

            CreateMap<Shared.Models.CustomerDesiring, CustomerDesiringViewModel>();
            CreateMap<Services.Services.ProductStock, LamMocBaoWeb.ViewModels.ProductStock>();
            CreateMap<Services.Services.ProductSizeStock, LamMocBaoWeb.ViewModels.ProductSizeStock>();


            CreateMap<Shared.Models.ProductTag, TagViewModel>();
            CreateMap<Shared.Models.Tag, TagViewModel>();
            CreateMap<Shared.Models.ProductMaterial, MaterialViewModel>()
                .ForMember(d => d.Name, e => e.MapFrom(s => s.Material.Name))
                .ForMember(d => d.Id, e => e.MapFrom(s => s.Id));
            CreateMap<Shared.Models.ProductSize, StockViewModel>()
                .ForMember(d => d.Number, e => e.MapFrom(s => s.Size.Number))
                .ForMember(d => d.Unit, e => e.MapFrom(s => s.Size.Unit));
            CreateMap<Shared.Models.Product, ProductViewModel>()
                .ForMember(d => d.ImageUrls, e => e.MapFrom(x => x.ProductImages.Select(d => d.Url)))
                .ForMember(d => d.Images, e => e.MapFrom(x => x.ProductImages))
                .ForMember(d => d.Tags, e => e.MapFrom(x => x.ProductTags.Select(d => d.Tag)))
                .ForMember(d => d.Materials, e => e.MapFrom(x => x.ProductMaterials))
                .ForMember(d => d.SupportedSizes, e => e.MapFrom(x => x.ProductSizes))
                .ForMember(d => d.PriceBySizes, e => e.MapFrom(x => x.ProductSizes.Select(d => new SizePriceModel {  SizeId = d.Id, SellingPrice = d.SellingPrice})))
                .ForMember(d => d.ShortInfomations, e => e.MapFrom(x => x.FormattedInfomations));
        }

        void ReadToWrite()
        {
            CreateMap<CreateProductModel, Shared.Models.Product>()
                .ForMember(d => d.SellingPrice, e => e.Ignore())
                .ForMember(d => d.PurchasingPrice, e => e.Ignore())
                .ForMember(d => d.FormattedInfomations, e => e.MapFrom(s => s.Infomations));
            CreateMap<UpdateProductModel, Shared.Models.Product>()
                .ForMember(d => d.SellingPrice, e => e.Ignore())
                .ForMember(d => d.PurchasingPrice, e => e.Ignore())
                .ForMember(d => d.ProductTags, e => new List<Shared.Models.ProductTag>())
                .ForMember(d => d.ProductSizes, e => new List<Shared.Models.ProductSize>())
                .ForMember(d => d.ProductImages, e => new List<Shared.Models.ProductImage>())
                .ReverseMap();
            CreateMap<CreateProductTypeModel, Shared.Models.ProductType>();
            CreateMap<UpdateProductTypeModel, Shared.Models.ProductType>()
                .ReverseMap();
            CreateMap<CreateCategoryModel, Shared.Models.Category>();
            CreateMap<UpdateCategoryModel, Shared.Models.Category>()
                .ReverseMap();

            CreateMap<CreateTagModel, Shared.Models.Tag>();
            CreateMap<UpdateTagModel, Shared.Models.Tag>()
                .ReverseMap();

            CreateMap<PromotionMode, Shared.Models.PromotionMode>();
            CreateMap<CreatePromotionModel, Shared.Models.Promotion>();
            CreateMap<UpdatePromotionModel, Shared.Models.Promotion>()
                .ReverseMap();

            CreateMap<AppointmentModel, Shared.Models.Appointment>();
            CreateMap<CreateSizeModel, Shared.Models.Size>();
            CreateMap<UpdateSizeModel, Shared.Models.Size>()
                .ReverseMap();

            CreateMap<CreateMaterialModel, Shared.Models.Material>();
            CreateMap<UpdateMaterialModel, Shared.Models.Material>()
                .ReverseMap();

            CreateMap<CreateNewsPaperPostModel, Shared.Models.NewsPaperPost>();
            CreateMap<UpdateNewsPaperPostModel, Shared.Models.NewsPaperPost>()
                .ReverseMap();

            CreateMap<CreateKnowledgeModel, Shared.Models.Knowledge>();
            CreateMap<UpdateKnowledgeModel, Shared.Models.Knowledge>()
                .ReverseMap();

            CreateMap<CreateCustomerCommentModel, Shared.Models.CustomerComment>();
            CreateMap<UpdateCustomerCommentModel, Shared.Models.CustomerComment>()
                .ReverseMap();

            CreateMap<UpdateSystemSettingModel, Shared.Models.SystemSetting>()
                .ReverseMap();
            CreateMap<DeliveryAddressModel, Shared.Models.DeliveryAddress>();

            CreateMap<AdvisingModel, Shared.Models.CustomerDesiring>()
                .ForMember(d => d.CustomerAddress, e => e.MapFrom(s => s.Address))
                .ForMember(d => d.CustomerPhoneNumber, e => e.MapFrom(s => s.PhoneNumber))
                .ForMember(d => d.CustomerEmail, e => e.MapFrom(s => s.Email))
                .ForMember(d => d.Birthday, e => e.MapFrom(s => s.Birthday ?? DateTime.MinValue));
        }
    }
}
