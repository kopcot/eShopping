using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Core.Specs
{
    public class ImageFileDirectorySpecParams
    {
        public SortingType? Sorting { get; set; } = SortingType.NoSorting;
        public enum SortingType : ushort
        {
            NoSorting,
            DescendingById,
            AscendingById,
            DescendingByDirectory,
            AscendingByDirectory
        }
    }
}
