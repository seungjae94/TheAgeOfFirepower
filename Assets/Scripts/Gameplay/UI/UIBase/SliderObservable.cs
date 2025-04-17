using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class SliderObservable
    {
        private readonly IObservable<float> observable;
        private bool isChanging = false;
        public readonly CompositeDisposable disposables = new();

        public SliderObservable(
            Slider slider, 
            Action<float> onValueChange,
            Action<float> onStartEdit,
            Action<float> onEndEdit,
            float timeout = 0.2f
        )
        {
            observable = slider
                .OnValueChangedAsObservable()
                .Publish()
                .RefCount();

            observable
                .Subscribe(onValueChange)
                .AddTo(disposables);
            
            observable
                .Where(v => !isChanging)
                .Do(v => isChanging = true)
                .Subscribe(v => onStartEdit?.Invoke(v))
                .AddTo(disposables);

            observable
                .Throttle(TimeSpan.FromSeconds(timeout)) // Debounce for the given timeout
                .Do(v => isChanging = false)
                .Subscribe(v => onEndEdit?.Invoke(v))
                .AddTo(disposables);
        }

        public IDisposable Subscribe(IObserver<float> observer)
        {
            return observable.Subscribe(observer);
        }
    }
}