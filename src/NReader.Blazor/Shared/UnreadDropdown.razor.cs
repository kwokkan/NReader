using Microsoft.AspNetCore.Components;

namespace NReader.Blazor.Shared
{
    public partial class UnreadDropdown
    {
        [Parameter]
        public bool? Unread { get; set; }

        [Parameter]
        public EventCallback<bool?> UnreadChanged { get; set; }

        private string UnreadString
        {
            get
            {
                return Unread.ToString();
            }
            set
            {
                if (bool.TryParse(value, out var b))
                {
                    UnreadChanged.InvokeAsync(b);
                }
                else
                {
                    UnreadChanged.InvokeAsync(null);
                }
            }
        }
    }
}
