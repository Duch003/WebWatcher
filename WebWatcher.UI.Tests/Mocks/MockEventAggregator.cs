using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebWatcher.UI.Tests.Mocks
{
    public class MockEventAggregator : IEventAggregator
    {
        private Dictionary<Type, Action<object>> _callbackList;

        public MockEventAggregator()
        {
            _callbackList = new Dictionary<Type, Action<object>>();
        }
        public bool HandlerExistsFor(Type messageType) => throw new NotImplementedException();
        public void Publish(object message, Action<System.Action> marshal) => PublishOnUIThread(message);
        public void Subscribe(object subscriber) { }
        public void Unsubscribe(object subscriber) => throw new NotImplementedException();
        public void PublishOnUIThread(object message)
        {
            if (_callbackList.ContainsKey(message.GetType()))
            {
                _callbackList[message.GetType()].Invoke(message);
            }
        }

        public void AddCallback(Type key, Action<object> callback)
        {
            if (_callbackList.ContainsKey(key))
            {
                _callbackList[key] = callback;
            }
            else
            {
                _callbackList.Add(key, callback);
            }
        }
    }
}
