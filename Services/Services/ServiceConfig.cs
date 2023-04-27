using Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Services.Services
{
    public interface IServiceConfig
    {
        public string AzureBlobConnectionString { get; }
        public string AzureBlobContainer { get; }
        public string ConnectionString { get; }
        public string SqliteConnectionString { get; }
        public string Currency_Symbol { get; set; }
        public decimal FilterProductPriceUpTo { get; set; }
        public int FilterProductStep { get; set; }
        public int NumerSuggestProductInPageByLinkName { get; set; }
        public int NumerDisplayProducts { get; }
        public int NumerSuggestKnowledges { get; }
        public int NumerHomepageKnowledges { get; }
        public int RandomRatio { get; }
        public bool IsTestMode { get; }
        public LMB LMB { get; }
        public ContactInfos ContactInfos { get; set; }
        public PaymentInfos PaymentInfos { get; set; }
        public OrderSuccessInfos OrderSuccessInfos { get; }
        public HompageContent HompageContent { get; }
        public Dictionary<string, string> InterestServices { get; }
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Indexes { get; }
        public int OnTrendInDays { get; }
        public int NumberOfItemsOnTrend { get; set; }
        public int AutoHighlightItemsInDays { get; set; }
        public string PingEndpoint { get; set; }
        public int RareItemShowWarningBelow { get; set; }
        public string HostWebRootPath { get; set; }
        public string LmbFiles { get; set; }

        public List<ProductFilterByFengShui> ShowOnHomepageFengshuis { get; set; }
    }

    public class ServiceConfig : IServiceConfig
    {
        public string AzureBlobConnectionString { get; set; }

        public string AzureBlobContainer { get; set; }
        public string ConnectionString { get; set; }
        public string Currency_Symbol { get; set; }
        public bool IsTestMode { get; set; }
        public LMB LMB { get; set; }

        public ContactInfos ContactInfos { get; set; } = new ContactInfos();
        public PaymentInfos PaymentInfos { get; set; } = new PaymentInfos();
        public OrderSuccessInfos OrderSuccessInfos { get; set; } = new OrderSuccessInfos();

        public string SqliteConnectionString { get; set; } = "LamMocBao.db";
        public HompageContent HompageContent { get; set; }
        public decimal FilterProductPriceUpTo { get; set; }
        public int FilterProductStep { get; set; }
        public int NumerSuggestProductInPageByLinkName { get; set; }
        public int NumerDisplayProducts { get; set; }
        public int NumerSuggestKnowledges { get; }
        public int NumerHomepageKnowledges { get; set; }
        public int RandomRatio { get; }
        public Dictionary<string, string> InterestServices { get; set; }

        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Indexes { get; set; }

        public int OnTrendInDays { get; set; }
        public int NumberOfItemsOnTrend { get; set; }
        public int AutoHighlightItemsInDays { get; set; }
        public List<ProductFilterByFengShui> ShowOnHomepageFengshuis { get; set; }
        public string PingEndpoint { get; set; }
        public int RareItemShowWarningBelow { get ; set; }
        public string HostWebRootPath { get; set; }
        public string LmbFiles { get; set; }
    }

    public class ProductFilterByFengShui
    {
        public OriginalType OriginalType { get; set; }
        public string Name { get; set; }

        public Guid Id { get; set; }
    }

    public class LMB
    {
        public string JwtIssuer { get; set; }
        public string SecurityKey { get; set; }
        public TimeSpan CacheExpireTime { get; set; }
        public int ExpireAfterDays { get; set; }
        public int ExpireAfterHours { get; set; }
        public bool EnableCache { get; set; }
    }

    public class ContactInfos
    {
        public string ContactAddress { get; set; }
        public string ContactPhoneNumbers { get; set; } 
        public string Email { get; set; }
        public string WorkingTime { get; set; }
        public string GoogleMapFrameUrl { get; set; }
        public string Facebook { get; set; }
        public string FacebookName { get; set; }
        public string Youtube { get; set; }
        public string Instagram { get; set; }
    }


    public class PaymentInfos
    {
        public string BankAccount { get; set; }
        public string CardHolderName { get; set; }
        public string BankName { get; set; }
    }
    public class OrderSuccessInfos
    {
        public string InstructionsForUseAndMaintenanceContent { get; set; }
        public string AfterSellPolicyContent { get; set; }
        public string WarrantyAndReturnPolicyContent { get; set; }
    }

    public class HompageContent
    {
        public string PersonalizedProductDesignContent { get; set; }
        public string AnalysisOfFateContent { get; set; }
        public string CollectRareItemsContent { get; set; }
    }
}
