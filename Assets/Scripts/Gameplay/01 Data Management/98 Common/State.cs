using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace Mathlife.ProjectL.Gameplay
{
    public class State<DataType>
    {
        ReactiveProperty<DataType> _property = new(default);

        public DataType GetState() { return _property.Value; }
        public DataType SetState(DataType value) { return _property.Value = value; }

        public IDisposable SubscribeChangeEvent(Action<DataType> onChange)
        {
            return _property.Subscribe(onChange);
        }

        public IObservable<TProperty> ObserveEveryValueChanged<TProperty>(
            Func<ReactiveProperty<DataType>, TProperty> propertySelector
        )
        {
            return _property.ObserveEveryValueChanged(propertySelector);
        }
    }
}
