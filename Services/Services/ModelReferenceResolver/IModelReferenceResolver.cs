using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Services.Services.ModelReferenceResolver
{
    public interface IModelReferenceResolver
    {
        bool Accept<TEntity>() where TEntity : Entity;
        List<Expression<Func<TEntity, object>>> GetExpressions<TEntity>() where TEntity : Entity;
    }

    public class ContactInfoReferenceResolver : IModelReferenceResolver
    {
        public bool Accept<TEntity>() where TEntity : Entity
        {
            return typeof(TEntity) == typeof(ContactInfo);
        }

        public List<Expression<Func<TEntity, object>>> GetExpressions<TEntity>() where TEntity : Entity
        {
            return new List<Expression<Func<TEntity, object>>>();
        }
    }

    public class CustomerDesiringReferenceResolver : IModelReferenceResolver
    {
        public bool Accept<TEntity>() where TEntity : Entity
        {
            return typeof(TEntity) == typeof(CustomerDesiring);
        }

        public List<Expression<Func<TEntity, object>>> GetExpressions<TEntity>() where TEntity : Entity
        {
            return new List<Expression<Func<TEntity, object>>>
            {
                entity => (entity as CustomerDesiring).CustomerPrefereds,
                entity => (entity as CustomerDesiring).CustomerPrefereds.Select(d => d.PreferedProduct)
            };
        }
    }

    public class CustomerPreferedInfosReferenceResolver : IModelReferenceResolver
    {
        public bool Accept<TEntity>() where TEntity : Entity
        {
            return typeof(TEntity) == typeof(CustomerPreferedInfos);
        }

        public List<Expression<Func<TEntity, object>>> GetExpressions<TEntity>() where TEntity : Entity
        {
            return new List<Expression<Func<TEntity, object>>>
            {
                entity => (entity as CustomerPreferedInfos).PreferedProduct,
                entity => (entity as CustomerPreferedInfos).CustomerDesiring
            };
        }
    }

    public class ProductReferenceResolver : IModelReferenceResolver
    {
        public bool Accept<TEntity>() where TEntity : Entity
        {
            return typeof(TEntity) == typeof(Product);
        }

        public List<Expression<Func<TEntity, object>>> GetExpressions<TEntity>() where TEntity : Entity
        {
            return new List<Expression<Func<TEntity, object>>>
            {
                entity => (entity as Product).CustomerPrefereds,
                entity => (entity as Product).ProductImages
            };
        }
    }

    public class ProductImageReferenceResolver : IModelReferenceResolver
    {
        public bool Accept<TEntity>() where TEntity : Entity
        {
            return typeof(TEntity) == typeof(ProductImage);
        }

        public List<Expression<Func<TEntity, object>>> GetExpressions<TEntity>() where TEntity : Entity
        {
            return new List<Expression<Func<TEntity, object>>>
            {
                entity => (entity as ProductImage).Product
            };
        }
    }

    public class OtherReferenceResolver : IModelReferenceResolver
    {
        public bool Accept<TEntity>() where TEntity : Entity
        {
            return true;
        }

        public List<Expression<Func<TEntity, object>>> GetExpressions<TEntity>() where TEntity : Entity
        {
            return new List<Expression<Func<TEntity, object>>>();
        }
    }
}
