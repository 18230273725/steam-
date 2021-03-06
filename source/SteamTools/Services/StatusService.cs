using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace SteamTools.Services
{
	/// <summary>
	/// 提供对显示在主窗口底部的状态栏的访问。
	/// </summary>
	public class StatusService : NotificationObject
	{
		#region static members

		public static StatusService Current { get; } = new StatusService();

		#endregion

		private readonly Subject<string> notifier;
		private string persisitentMessage = "";
		private string notificationMessage;

		#region Message 変更通知

		/// <summary>
		/// 获取指示当前状态的字符串。
		/// </summary>
		public string Message
		{
			get { return this.notificationMessage ?? this.persisitentMessage; }
			set
			{
				if (this.persisitentMessage != value)
				{
					this.persisitentMessage = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		private StatusService()
		{
			this.notifier = new Subject<string>();
			this.notifier
				.Do(x =>
				{
					this.notificationMessage = x;
					this.RaiseMessagePropertyChanged();
				})
				.Throttle(TimeSpan.FromMilliseconds(5000))
				.Subscribe(_ =>
				{
					this.notificationMessage = null;
					this.RaiseMessagePropertyChanged();
				});
		}

		public void Set(string message)
		{
			this.Message = message;
		}

		public void Notify(string message)
		{
			this.notifier.OnNext(message);
		}

		private void RaiseMessagePropertyChanged()
		{
			this.RaisePropertyChanged(nameof(this.Message));
		}
	}
}
