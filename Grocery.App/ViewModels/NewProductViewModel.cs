using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using Grocery.App.Views;
using Grocery.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grocery.App.ViewModels
{
    public partial class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        Client client;

        [ObservableProperty]
        string productName;
        public NewProductViewModel(IProductService productService, GlobalViewModel global)
        {
            _productService = productService;
            client = global.Client;
        }

        [RelayCommand]
        private async Task AddProduct()
        {
            if (client.Role == Role.Admin)
            {
                _productService.Add(new Product(0, productName, 0));
            }
            await Shell.Current.GoToAsync(nameof(ProductView), true);

        }
    }
}