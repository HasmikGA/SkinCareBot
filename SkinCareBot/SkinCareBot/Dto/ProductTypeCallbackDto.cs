using SkinCareBot.Entities;

namespace SkinCareBot.Dto
{
    internal class ProductTypeCallbackDto : CallbackDto
    {
        public ProductType ProductType { get; set; }

        public static new ProductTypeCallbackDto FromString(string input)
        {
            var inputs = input.Split('|');
            var success = Enum.TryParse(inputs[1], out ProductType productType);
            var result = new ProductTypeCallbackDto
            {
                Action = inputs[0],
                ProductType = productType
            };

            return result;
        }

        public override string ToString()
        {
            return $"{base.ToString()}|{ProductType}";
        }
    }
}
