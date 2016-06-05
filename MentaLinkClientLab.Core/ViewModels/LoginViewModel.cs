﻿using Prism.Commands;
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

		private string _info;

		public string Info
		{
			get { return _info; }
			set { _info = value; OnPropertyChanged(); }
		}

		Action _onConnect;
		IMqttClient _client;
		Guid _userId;
		public ICommand ConnectCommand { get; }

		public LoginViewModel(IMqttClient client, Action onConnect)
		{
			_onConnect = onConnect;
			_client = client;
			_userId = Guid.NewGuid(); ;
			client.Connect(_userId.ToString());
			Host = "_AA";

			ConnectCommand = new DelegateCommand(async () => await Connect());
		}

		private async Task Connect()
		{
			Info = "Connecting...";

			string gameControl = "GameControl" + Host;
			var myTopic = gameControl + User + _userId.ToString();

			var message = new
			{
				type = "connect",
				username = User,
				id = _userId.ToString()
			};

			_client.Send(gameControl, message.ToByteArray());

			_client.Subscribe(new string[] { myTopic },
				new byte[] { 2 });

			while (true)
			{
				var msg = await _client.ReceiveAsync();
				var response = msg.Payload.ToDynamic();
				if (response.type == "connect" && response.status == "ok")
				{
					Session.Host = Host;
					Session.GameControl = gameControl;
					Session.User = User;
					Session.Id = _userId;
					_onConnect.Invoke();
					break;
				}
			}
		}
	}
}