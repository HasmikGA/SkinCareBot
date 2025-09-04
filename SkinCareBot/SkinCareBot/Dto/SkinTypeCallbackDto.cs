using SkinCareBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Dto
{
    internal class SkinTypeCallbackDto: CallbackDto
    {
        public SkinType SkinType { get; set; }
        public static new SkinTypeCallbackDto FromString(string input)
        {
            var inputs = input.Split('|');
            var success = Enum.TryParse(inputs[1], out SkinType skinType);
            var result = new SkinTypeCallbackDto
            {
                Action = inputs[0],
                SkinType = skinType
            };

            return result;
        }

        public override string ToString()
        {
            return $"{base.ToString()}|{SkinType}";
        }
    }
}
