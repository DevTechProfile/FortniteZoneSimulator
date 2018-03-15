using FortniteZoneSimulator;
using MediaLib;
using ObservableExtension;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Visualizer
{
    public class FortniteZoneSimulatorViewModel : BindableBase
    {
        private string _info;
        private string _calculationTime;
        private BitmapSource _matrixBitmapSource;
        private ZoneSimulator _zoneSimulator;
        private IDisposable _sequenceDispose;

        public string Info
        {
            get { return _info; }
            set { _info = value; RaisePropertyChanged(); }
        }

        public string CalculationTime
        {
            get { return _calculationTime; }
            set { _calculationTime = value; RaisePropertyChanged(); }
        }

        public BitmapSource MatrixBitmapSource
        {
            get { return _matrixBitmapSource; }
            set { _matrixBitmapSource = value; RaisePropertyChanged(); }
        }

        public ICommand AddZoneCommand { get; }

        public ICommand ContractZoneCommand { get; }

        public ICommand ResetZoneCommand { get; }

        public FortniteZoneSimulatorViewModel()
        {
            AddZoneCommand = new DelegateCommand(OnAddZone);
            ContractZoneCommand = new DelegateCommand(OnContractZone);
            ResetZoneCommand = new DelegateCommand(OnResetZone);

            _zoneSimulator = new ZoneSimulator();
            MatrixBitmapSource = BitmapHelper.BitmapToBitmapSource(_zoneSimulator.InitialZoneSet);
        }

        private void OnResetZone()
        {
            _zoneSimulator.Reset();
            MatrixBitmapSource = BitmapHelper.BitmapToBitmapSource(_zoneSimulator.InitialZoneSet);
        }

        private void OnAddZone()
        {
            MatrixBitmapSource = BitmapHelper.BitmapToBitmapSource(_zoneSimulator.GetNextZoneSet());
        }

        private void OnContractZone()
        {
            _sequenceDispose?.Dispose();

            int stepCount = 100;
            var context = SynchronizationContext.Current;

            _sequenceDispose = Sequence.LimitedTimeSequence(stepCount, TimeSpan.FromMilliseconds(40))
                    .ObserveOn(Scheduler.Default)
                    .Select(step =>
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        var bitmap = _zoneSimulator.DoContractSteps(stepCount, step);
                        var bitmapSource = BitmapHelper.BitmapToBitmapSource(bitmap);
                        bitmapSource.Freeze();
                        stopwatch.Stop();
                        return new Tuple<int, long, BitmapSource>(step, stopwatch.ElapsedMilliseconds, bitmapSource);
                    })
                    .ObserveOn(context)
                    .SubscribeOn(context)
                    .Subscribe(tuple =>
                    {
                        MatrixBitmapSource = tuple.Item3;
                        Info = (stepCount - tuple.Item1).ToString();
                        CalculationTime = tuple.Item2.ToString() + " ms";
                    }, () => Info = "Done");
        }
    }
}
