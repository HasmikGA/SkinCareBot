using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBot.Dto
{
    internal class CallbackDto
    {
        public string Action { get; set; }

        public static CallbackDto FromString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException(nameof(input));
            }

            var inputs = input.Split('|');

            var callback = new CallbackDto() { Action = inputs[0] };

            return callback;
        }

        public override string ToString()
        {
            return this.Action;
        }
    }
}
