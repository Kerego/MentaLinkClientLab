using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MentaLinkClientLab.Core.ViewModels
{
	public class LoginViewModel : BindableBase
	{
		private string _host;

		public string Host
		{
			get { return _host; }
			set { _host = value; OnPropertyChanged(); }
		}

		private string _user;

		public string User
		{
			get { return _user; }
			set { _user = value; OnPropertyChanged(); }
		}

		private bool _isConnecting;

		public bool IsConnecting
		{
			get { return _isConnecting; }
			set { _isConnecting = value; OnPropertyChanged(); OnPropertyChanged(() => CommandActionText); }
		}

		public string CommandActionText { get { return _isConnecting ? "Cancel" : "Connect"; } }


		Action _onConnect;
		IMqttClient _client;
		Guid _userId;
		public ICommand ConnectCommand { get; }

		public LoginViewModel(IMqttClient client, Action onConnect)
		{
			_onConnect = onConnect;
			_client = client;
			_userId = Guid.NewGuid(); ;
			Host = "_AA";
			ConnectCommand = new DelegateCommand(async () => await Connect());
			try
			{
				_client.Connect(_userId.ToString());
			}
			catch
			{

			}
		}

		private async Task Connect()
		{
			if(_isConnecting)
			{
				IsConnecting = false;
				return;
			}

			IsConnecting = true;

			try
			{
				string gameControl = "GameControl" + Host;
				string myTopic = gameControl + User + _userId.ToString();

				var message = new
				{
					type = "connect",
					username = User,
					id = _userId.ToString()
				};

				_client.Subscribe(new string[] { myTopic },
					new byte[] { 2 });
				_client.Send(gameControl, message.ToByteArray());

				while (_isConnecting)
				{
					var msg = await _client.ReceiveAsync();
					var response = msg.Payload.ToDynamic();
					if (response.type == "connect" && response.status == "ok" && _isConnecting)
					{
						Session.Host = Host;
						Session.GameControl = gameControl;
						Session.User = User;
						Session.Id = _userId;
						_onConnect.Invoke();
						break;
					}
				}
				if (!_isConnecting)
					await _client.UnSubscribeAsync(new string[] { myTopic });
			}
			catch
			{

			}
			
		}
	}
}
