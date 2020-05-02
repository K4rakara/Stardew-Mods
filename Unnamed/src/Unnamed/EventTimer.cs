

namespace Unnamed
{
	public class EventTimer
	{
		public delegate void OnTimeout();
		private int timeout;
		private OnTimeout onTimeout;

		public EventTimer(int timeout, OnTimeout onTimeout)
		{
			this.timeout = timeout;
			this.onTimeout = onTimeout;
		}

		public void Update()
		{
			this.timeout--;
			if (this.timeout == 0)
			{
				this.onTimeout();
			}
		}

		public bool ReadyToBeRemoved()
		{
			return (this.timeout == 0);
		}
	}
}