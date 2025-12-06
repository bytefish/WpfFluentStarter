// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfFluentStarter.Models
{
    /// <summary>
    /// A model representing a furniture item.
    /// </summary>
    public partial class FurnitureItem : ObservableObject
    {
        /// <summary>
        /// Gets or sets the name of the furniture item.
        /// </summary>
        [ObservableProperty]
        public string _name = "Unknown";

        /// <summary>
        /// Gets or sets the X coordinate of the furniture item.
        /// </summary>
        [ObservableProperty]
        public double _X;

        /// <summary>
        /// Gets or sets the Y coordinate of the furniture item.
        /// </summary>
        [ObservableProperty]
        public double _y;

        /// <summary>
        /// Gets or sets the Color of the furniture item.
        /// </summary>
        [ObservableProperty]
        public string _colorHex = "#CCCCCC";
    }
}
