using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ticktock.ViewModels
{
	public class MainWindowViewModel : BaseViewModel
	{
		// Constants
		private const int SecondsInMinute = 60;
		private const double DefaultCenterX = 100;
		private const double DefaultCenterY = 100;
		private const double DefaultSecondsHandLength = 80;

		private DateTime _lastTickTime;

		private readonly IApiClient _client;
		private ClockCalculator _clockCalculator;
		private DispatcherTimer _timer;

		#region Properties
		// Need a boolean to keep track of whether the timer is running
		private bool _isRunning;
		public bool IsRunning
		{
			get { return _isRunning; }
			set
			{
				_isRunning = value;
				EvaluateButtons();
			}
		}

		// Properties for the elapsed time
		private TimeSpan _elapsedTime;
		public TimeSpan ElapsedTime
		{
			get { return _elapsedTime; }
			set
			{
				_elapsedTime = value;
				ElapsedTimeText = _elapsedTime.ToString(@"hh\:mm\:ss");
				OnPropertyChanged(nameof(ElapsedTime));
				// update second hand 
				OnPropertyChanged(nameof(SecondHandX));
				OnPropertyChanged(nameof(SecondHandY));
				// update progress bar
				OnPropertyChanged(nameof(ProgressBarValue));
			}
		}

		private string _elapsedTimeText;
		public string ElapsedTimeText
		{
			get { return _elapsedTimeText; }
			set
			{
				_elapsedTimeText = value;
				OnPropertyChanged(nameof(ElapsedTimeText));
			}
		}

		// Properties for the progress bar
		public int ProgressBarMaxValue = SecondsInMinute;
		public int ProgressBarValue
		{
			get
			{
				return (int)_elapsedTime.TotalSeconds % ProgressBarMaxValue;
			}
		}

		// Properties for the SecondHand
		public double SecondHandX
		{ get { return _clockCalculator.CalculateSecondHandX(ElapsedTime); } }
		public double SecondHandY
		{ get { return _clockCalculator.CalculateSecondHandY(ElapsedTime); } }
		public double CenterX
		{ get { return _clockCalculator.CenterX; } }
		public double CenterY
		{ get { return _clockCalculator.CenterY; } }

		// Properties for Time from API
		private string _currentTime;
		public string CurrentTime
		{
			get { return _currentTime; }
			set
			{
				_currentTime = value;
				OnPropertyChanged(nameof(CurrentTime));
			}
		}

		// Properties for the buttons
		private String _startStopButtonContent = "Start";
		public String StartStopButtonContent
		{
			get { return _startStopButtonContent; }
			private set
			{
				_startStopButtonContent = value;
				OnPropertyChanged(nameof(StartStopButtonContent));
			}
		}

		private bool _isResetButtonEnabled = true;
		public bool IsResetButtonEnabled
		{
			get { return _isResetButtonEnabled; }
			private set
			{
				_isResetButtonEnabled = value;
				OnPropertyChanged(nameof(IsResetButtonEnabled));
			}
		}
		#endregion Properties

		public void EvaluateButtons()
		{
			StartStopButtonContent = _isRunning ? "Stop" : "Start";
			IsResetButtonEnabled = !_isRunning;
		}

		// CONSTRUCTOR
		public MainWindowViewModel(IApiClient apiClient)
		{

			StartStopCommand = new RelayCommand(StartStopButtonClicked);
			ResetCommand = new RelayCommand(ResetButtonClicked);
			GetCurrentTimeCommand = new RelayCommand(async () => await GetCurrentTime());
			_client = apiClient;
			_clockCalculator = new ClockCalculator(DefaultCenterX, DefaultCenterY, DefaultSecondsHandLength);
			_currentTime = DateTime.Now.ToString("T");
			_timer = new DispatcherTimer();
			ElapsedTime = TimeSpan.Zero;
			InitializeTimer();
		}

		#region Commands
		// Button Commands: StartStopCommand, ResetCommand
		public ICommand StartStopCommand { get; }
		public ICommand ResetCommand { get; }
		public ICommand GetCurrentTimeCommand { get; }
		// Command Methods
		private void StartStopButtonClicked()
		{
			ToggleTimer();
		}

		private void ResetButtonClicked()
		{
			ClearTimer();
		}

		private async Task GetCurrentTime()
		{
			CurrentTime = await _client.GetTimeFromIp();
		}
		#endregion Commands

		#region Timer Functions
		/// <summary>
		/// Initializes the timer
		/// </summary>
		private void InitializeTimer()
		{
			_timer.Interval = TimeSpan.FromSeconds(1);
			_timer.Tick += Timer_Tick;
			_timer.IsEnabled = false;
		}

		/// <summary>
		/// Starts or stops the timer
		/// </summary>
		private void ToggleTimer()
		{
			if (_timer.IsEnabled)
			{
				_timer.Stop();
				IsRunning = false;
			}
			else
			{
				_lastTickTime = DateTime.Now;
				_timer.Start();
				IsRunning = true;
			}
		}

		/// <summary>
		/// Clears the timer if it is not running
		/// </summary>
		private void ClearTimer()
		{
			if (!IsRunning)
			{
				ElapsedTime = TimeSpan.Zero;
			}
		}

		/// <summary>
		/// Updates the ElapsedTime property every second
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Timer_Tick(object? sender, EventArgs e)
		{
			ElapsedTime += DateTime.Now - _lastTickTime;
			_lastTickTime = DateTime.Now;
		}
		#endregion Timer Functions
	}
}
