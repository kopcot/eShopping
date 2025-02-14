using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Core.Responses
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class ApiResponse<T>
    {
        public int Id { get; set; }
        public string User { get; set; }
        public T ResultValue { get; set; }
        public static ApiResponse<T> CreateApiResponse(T resultValue, string? user, int id = -999)
        {
            ArgumentNullException.ThrowIfNull(user, nameof(user));

            return new ApiResponse<T>()
            {
                Id = id,
                User = user,
                ResultValue = resultValue,
            };
        }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
