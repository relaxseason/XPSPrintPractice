using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using XPSPrintPractice.Models.XPSPrint;

namespace XPSPrintPractice.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public ReactiveCommand StartPrintCommand { get; }
        public ReactiveProperty<string> ResultLabel { get; }

        public MainWindowViewModel()
        {
            ResultLabel = new ReactiveProperty<string>();
            StartPrintCommand = new ReactiveCommand()
                .AddTo(Disposables);
            StartPrintCommand.Subscribe(_ => StartPrintAction());
        }

        private async void StartPrintAction()
        {
            ResultLabel.Value = "処理開始";
            var print = new PrintProcess();
            var result = await print.Print();
            if (!result.Contains("Exception"))
            {
                ResultLabel.Value = "処理成功";
            }
            else
            {
                ResultLabel.Value = "処理失敗";
            }
        }
    }
}
