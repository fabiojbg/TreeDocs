using Apps.Blazor.Components.Extensions;
using Apps.Sdk.Helpers;
using Blazored.Toast.Services;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TreeDocs.Service.Contracts;

namespace TreeDocs.ClientApp.Services
{

    public interface INotifier
    {
        bool ShowIfHasError(ValidatableRequest request);
        bool ShowIfHasError(RequestResult result);
        bool ShowIfHasError(HttpResponseMessage httpResponse);
        void ShowSucess(string msg);
        void ShowWarning(string msg);
        void ShowError(string msg);
        void ShowError(Exception ex);
    }

    public class BlazoredToastNotifier : INotifier
    {
        IToastService _toastService;

        public BlazoredToastNotifier(IToastService toastService)
        {
            _toastService = toastService;
        }
        public bool ShowIfHasError(ValidatableRequest request)
        {

            if (!request.Validate() )
            {
                if (request.Notifications.Any())
                {
                    foreach (var notification in request.Notifications)
                        _toastService.ShowError($"{notification.Property}: {notification.Message}");
                }
                else
                {
                    _toastService.ShowError("Invalid request");
                }
                return true;
            }

            return false;
        }

        public bool ShowIfHasError(RequestResult result)
        {
            if (result == null)
                _toastService.ShowError($"Error not recognized");
            else
                if (result?._Result != RequestResultType.Success)
                {
                    if (result?._Notifications?.Any() == true)
                    {
                        foreach (var notification in result._Notifications)
                            _toastService.ShowError($"{notification.Property}: {notification.Message}");
                    }
                    else
                    {
                        _toastService.ShowError(result._Message);
                    }
                    return true;
                }

            return false;
        }

        public bool ShowIfHasError(HttpResponseMessage httpResponse)
        {
            if (httpResponse.IsSuccessStatusCode)
                return false;

            try
            {
                var requestResult = AsyncHelper.RunSync(() => httpResponse.Content.ReadJsonAsync<RequestResult>());
                if( requestResult != null)
                    return ShowIfHasError(requestResult);
                else
                    _toastService.ShowError($"Comunication error (HttpStatus={httpResponse.StatusCode})");
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Comunication error (HttpStatus={httpResponse.StatusCode}):{ex.Message}");
            }
            return true;
        }

        public void ShowSucess(string msg)
        {
            _toastService.ShowSuccess(msg);
        }
        public void ShowWarning(string msg)
        {
            _toastService.ShowWarning(msg);
        }
        public void ShowError(string msg)
        {
            _toastService.ShowError(msg);
        }
        public void ShowError(Exception ex)
        {
            _toastService.ShowError(ex.Message);
            Console.Write(ex.Message);
            Console.Write(ex.StackTrace);
        }
    }
}
