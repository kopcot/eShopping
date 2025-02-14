using Catalog.Core.Entities;
using eShopping.Client.Pages;
using System.ComponentModel.DataAnnotations;

namespace eShopping.Client.Model
{
    public class ModelProductCreate : IBaseModel<ModelProductCreate, Product>
    {
        private readonly Product _product;
        private readonly IEnumerable<ProductBrand> _productBrands;
        private readonly IEnumerable<ProductType> _productTypes ;
        private readonly IEnumerable<ImageFileDirectory> _imageFileDirectories;
        public ModelProductCreate(
            Product product, 
            IEnumerable<ProductBrand> productBrands, 
            IEnumerable<ProductType> productTypes , 
            IEnumerable<ImageFileDirectory> imageFileDirectories)
        {
            _product = product;
            _productBrands = productBrands;
            _productTypes = productTypes;
            _imageFileDirectories = imageFileDirectories;
        }

        public Product ToDomain()
        {
            return _product;
        }

        [Required(ErrorMessage = "Incorrect name")]
        [MinLength(3, ErrorMessage = "Name too short")]
        public string Name
        {
            get => _product.Name;
            set => _product.Name = value;
        }
        public string? Summary 
        {
            get => _product.Summary;
            set => _product.Summary = value;
        }
        public string? Description 
        {
            get => _product.Description;
            set => _product.Description = value;
        }
        [Required(ErrorMessage = "Incorrect price")]
        [DataType(DataType.Currency)]
        public decimal Price 
        {
            get => _product.Price;
            set => _product.Price = value;
        }
        public string? ImageFileName 
        {
            get => _product.ImageFileName;
            set => _product.ImageFileName = value;
        }
        //[Required]
        //[Range(1, int.MaxValue, ErrorMessage = "Incorrect image file Id")]
        public int ImageDirectoryId => _product.ImageFileID ?? 0;
        public string ImageDirectory
        {
            get => _product.ImageFileDirectory?.Directory ?? string.Empty;
            set
            {
                _product.ImageFileDirectory = _imageFileDirectories.FirstOrDefault(x => x.Directory == value) ?? new ImageFileDirectory();
                _product.ImageFileID = _product.ImageFileDirectory.Id;
            }
        }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Incorrect product brand Id")]
        public int BrandId => _product.BrandID;
        [Required(ErrorMessage = "Incorrect product brand")]
        [MinLength(3, ErrorMessage = "Brand name too short")]
        public string? BrandName
        {
            get => _product.Brand.Name;
            set
            {
                _product.Brand = _productBrands.FirstOrDefault(x => x.Name == value) ?? new ProductBrand();
                _product.BrandID = _product.Brand.Id;
            }
        }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Incorrect product type Id")]
        public int TypeId => _product.TypeID;
        [Required(ErrorMessage = "Incorrect product type")]
        [MinLength(3, ErrorMessage = "Type name too short")]
        public string? TypeName
        {
            get => _product.Type.Name;
            set
            {
                _product.Type = _productTypes.FirstOrDefault(x => x.Name == value) ?? new ProductType();
                _product.TypeID = _product.Type.Id;
            }
        }
    }
}
