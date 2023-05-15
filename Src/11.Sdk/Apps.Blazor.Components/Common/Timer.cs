using Microsoft.AspNetCore.Components;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Apps.Blazor.Components.Common
{
    public class Timer : ComponentBase , IDisposable
    {
        private System.Threading.Timer _timer;
        private bool _enabled;

        [Parameter]
        public double Period { get; set; }

        [Parameter]
        public bool FireOnce { get; set; }

        [Parameter]
        public EventCallback OnTick { get; set; }

        [Parameter]
        public bool Enabled {
            get { return _enabled; }
            set {
                if (value != this._enabled)
                {
                    this._enabled = value;
                    enabledChanged(this._enabled);
                }
            }
        }


        protected override Task OnParametersSetAsync()
        {
            enabledChanged(_enabled);
            return base.OnParametersSetAsync();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        protected void enabledChanged(bool enabled)
        {
            if( enabled )
            {
                if (_timer != null)
                    _timer.Dispose();

                if (Period <= 0)
                    return;

                var period = Timeout.InfiniteTimeSpan;
                if (!FireOnce)
                    period = TimeSpan.FromSeconds(Period);
                
                _timer = new System.Threading.Timer(
                                callback: (_) => InvokeAsync(() => OnTick.InvokeAsync(null)),
                                state: null,
                                dueTime: TimeSpan.FromSeconds(Period),
                                period: period);
            }
            else
            {
                _timer?.Dispose();
                _timer = null;
            }
        }

    }
}