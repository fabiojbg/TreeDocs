using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Blazor.Components
{
    public class AppComponentBase : ComponentBase
    {
        [Parameter]
        public Action<object> OnCreated { get; set; }

        [Parameter]
        public Action<object> OnRendered { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if( firstRender)
                OnCreated?.Invoke(this);

            OnRendered?.Invoke(this);

            base.OnParametersSet();
        }



    }
}
