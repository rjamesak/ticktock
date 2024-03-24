using System.Configuration;
using System.Data;
using System.Windows;
using ticktock.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ticktock
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// Create a new service collection
			var services = new ServiceCollection();

			// Register types for dependency injection
			services.AddSingleton<IApiClient>(provider => new ApiClient("https://worldtimeapi.org"));
			services.AddSingleton<MainWindowViewModel>();

			// Build the service provider
			var serviceProvider = services.BuildServiceProvider();

			// Resolve the MainWindowViewModel and other dependencies
			var mainWindowViewModel = serviceProvider.GetService<MainWindowViewModel>();

			// Create the MainWindow
			var mainWindow = new MainWindow();

			// Show the MainWindow
			mainWindow.Show();

			// Set its ViewModel
			mainWindow.DataContext = mainWindowViewModel;
		}

	}

}
