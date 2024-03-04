using Gol.Application.Presentation.Views;
using Gol.Application.Utils;
using Gol.Core.Algorithm;
using Gol.Core.Controls.Models;
using Gol.Core.Data;
using Gol.Core.Prism;
using System;

namespace Gol.Application.Presentation.ViewModels
{
    /// <summary>
    ///     <see cref="MainWindow" /> ViewModel.
    /// </summary>
    public class MainWindowViewModel : NotificationObject
    {
        private const int DEFAULT_FIELD_WIDTH = 130;
        private const int DEFAULT_FIELD_HEIGHT = 60;

        private ILifeControl<bool> _doubleStateLife;
        private bool _isBound;
        private bool _isInfinity;
        private bool _isStart;
        private string _startStopText = null!;

        /// <summary>
        ///     Constructor for <see cref="MainWindowViewModel" />.
        /// </summary>
        public MainWindowViewModel()
        {
            var grid = new MonoLifeGrid<bool>(new bool[DEFAULT_FIELD_WIDTH, DEFAULT_FIELD_HEIGHT], Guid.NewGuid());
            IsStart = true;

            _doubleStateLife = new DoubleStateLife(grid, true);
            Infinity(null);

            StartCommand = new DelegateCommand(Start);
            StopCommand = new DelegateCommand(Stop);
            StartStopCommand = new DelegateCommand(StartStop);
            StepCommand = new DelegateCommand(Step);
            RandomCommand = new DelegateCommand(Rand);
            SaveCommand = new DelegateCommand(Save);
            ExitCommand = new DelegateCommand(Exit);
            OpenCommand = new DelegateCommand(Open);
            NewCommand = new DelegateCommand(New);
            AboutCommand = new DelegateCommand(About);
            InfinityCommand = new DelegateCommand(Infinity);
            BoundCommand = new DelegateCommand(Bound);
        }

        private bool IsStart
        {
            get => _isStart;

            set
            {
                _isStart = value;
                StartStopText = _isStart ? "Start" : "Stop";
            }
        }

        public string StartStopText
        {
            get => _startStopText;

            set
            {
                _startStopText = value;
                RaisePropertyChanged(nameof(StartStopText));
            }
        }

        /// <summary>
        ///     Field type is infinity
        /// </summary>
        public bool IsInfinity
        {
            get => _isInfinity;

            set
            {
                _isInfinity = value;
                RaisePropertyChanged(nameof(IsInfinity));
            }
        }

        /// <summary>
        ///     Field type is bound
        /// </summary>
        public bool IsBound
        {
            get => _isBound;

            set
            {
                _isBound = value;
                RaisePropertyChanged(nameof(IsBound));
            }
        }

        /// <summary>
        ///     Mono grid controller.
        /// </summary>
        public ILifeControl<bool> DoubleStateLife
        {
            get => _doubleStateLife;

            private set
            {
                if (_doubleStateLife != value)
                {
                    _doubleStateLife = value;
                    RaisePropertyChanged(nameof(DoubleStateLife));
                }
            }
        }

        /// <summary>
        ///     About command.
        /// </summary>
        public DelegateCommand AboutCommand { get; }

        /// <summary>
        ///     Exit command.
        /// </summary>
        public DelegateCommand ExitCommand { get; }

        /// <summary>
        ///     New command.
        /// </summary>
        public DelegateCommand NewCommand { get; }

        /// <summary>
        ///     Start command.
        /// </summary>
        public DelegateCommand StartCommand { get; }

        /// <summary>
        ///     Stop command.
        /// </summary>
        public DelegateCommand StopCommand { get; }

        /// <summary>
        ///     Start or stop command.
        /// </summary>
        public DelegateCommand StartStopCommand { get; }

        /// <summary>
        ///     Step command.
        /// </summary>
        public DelegateCommand StepCommand { get; }

        /// <summary>
        ///     Open command.
        /// </summary>
        public DelegateCommand OpenCommand { get; }

        /// <summary>
        ///     Save command.
        /// </summary>
        public DelegateCommand SaveCommand { get; }

        /// <summary>
        ///     Random command.
        /// </summary>
        public DelegateCommand RandomCommand { get; }

        /// <summary>
        ///     Infinity field.
        /// </summary>
        public DelegateCommand InfinityCommand { get; }

        /// <summary>
        ///     Bound field.
        /// </summary>
        public DelegateCommand BoundCommand { get; }

        private static void About(object? obj)
        {
            var aboutWindow = new AboutWindow
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            aboutWindow.ShowDialog();
        }

        private void Open(object? obj)
        {
            if (FileUtils.TryGetOpenFile(out var fileStream))
            {
                using (fileStream)
                {
                    IsStart = true;
                    var grid = SerializationUtils.Read<MonoLifeGrid<bool>>(fileStream!);
                    DoubleStateLife = new DoubleStateLife(grid);
                }
            }
        }

        private void Save(object? obj)
        {
            if (DoubleStateLife.Current is not null && FileUtils.TryGetSaveFile(out var fileStream) && fileStream is not null)
            {
                using (fileStream)
                {
                    SerializationUtils.Save(fileStream, DoubleStateLife.Current);
                }
            }
        }

        private static void Exit(object? obj)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Start(object? obj)
        {
            if (IsStart)
            {
                IsStart = false;
                DoubleStateLife.Start();
            }
        }

        private void Stop(object? obj)
        {
            if (!IsStart)
            {
                IsStart = true;
                DoubleStateLife.Stop();
            }
        }

        private void StartStop(object? obj)
        {
            if (IsStart)
            {
                IsStart = false;
                DoubleStateLife.Start();
            }
            else
            {
                IsStart = true;
                DoubleStateLife.Stop();
            }
        }

        private void Step(object? obj)
        {
            IsStart = true;
            DoubleStateLife.Step();
        }

        private void Rand(object? obj)
        {
            DoubleStateLife.Random();
        }

        private void New(object? obj)
        {
            IsStart = true;
            var grid = new MonoLifeGrid<bool>(new bool[DEFAULT_FIELD_WIDTH, DEFAULT_FIELD_HEIGHT], Guid.NewGuid());
            DoubleStateLife = new DoubleStateLife(grid);
        }

        private void Infinity(object? obj)
        {
            SwitchFieldType(FieldType.Infinity);
            DoubleStateLife.Field(FieldType.Infinity);
        }

        private void Bound(object? obj)
        {
            SwitchFieldType(FieldType.Bound);
            DoubleStateLife.Field(FieldType.Bound);
        }

        private void SwitchFieldType(FieldType fieldType)
        {
            if (fieldType == FieldType.Infinity)
            {
                IsInfinity = true;
                IsBound = false;
            }
            else
            {
                IsInfinity = false;
                IsBound = true;
            }
        }
    }
}