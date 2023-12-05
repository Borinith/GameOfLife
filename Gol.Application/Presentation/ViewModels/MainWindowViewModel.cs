using Gol.Application.Presentation.Views;
using Gol.Application.Utils;
using Gol.Core.Algorithm;
using Gol.Core.Controls.Models;
using Gol.Core.Prism;
using System;

namespace Gol.Application.Presentation.ViewModels
{
    /// <summary>
    ///     <see cref="MainWindow" /> ViewModel.
    /// </summary>
    public class MainWindowViewModel : NotificationObject
    {
        private const int DefaultFieldWidth = 126;

        private const int DefaultFieldHeight = 58;

        private DoubleStateLife _doubleStateLife;

        /// <summary>
        ///     Constructor for <see cref="MainWindowViewModel" />.
        /// </summary>
        public MainWindowViewModel()
        {
            var grid = new MonoLifeGrid<bool>(new bool[DefaultFieldWidth, DefaultFieldHeight], Guid.NewGuid());

            _doubleStateLife = new DoubleStateLife(grid);
            StartCommand = new DelegateCommand(Start);
            StopCommand = new DelegateCommand(Stop);
            RandomCommand = new DelegateCommand(Rand);
            SaveCommand = new DelegateCommand(Save);
            ExitCommand = new DelegateCommand(Exit);
            OpenCommand = new DelegateCommand(Open);
            NewCommand = new DelegateCommand(New);
            AboutCommand = new DelegateCommand(About);
            InfinityCommand = new DelegateCommand(Infinity);
            BoundCommand = new DelegateCommand(Bound);
        }

        /// <summary>
        ///     Mono grid controller.
        /// </summary>
        public DoubleStateLife DoubleStateLife
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
        ///     Open command.
        /// </summary>
        public DelegateCommand OpenCommand { get; }

        /// <summary>
        ///     Save command.
        /// </summary>
        public DelegateCommand SaveCommand { get; }

        /// <summary>
        ///     Stop command.
        /// </summary>
        public DelegateCommand StopCommand { get; }

        /// <summary>
        ///     Random command.
        /// </summary>
        public DelegateCommand RandomCommand { get; }

        /// <summary>
        ///     Infinity field.
        /// </summary>
        public DelegateCommand InfinityCommand { get; set; }

        /// <summary>
        ///     Bound field.
        /// </summary>
        public DelegateCommand BoundCommand { get; set; }

        private void About(object obj)
        {
            var aboutWindow = new AboutWindow
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            aboutWindow.ShowDialog();
        }

        private void Open(object obj)
        {
            if (FileUtils.TryGetOpenFile(out var fileStream))
            {
                using (fileStream)
                {
                    var grid = SerializationUtils.Read<MonoLifeGrid<bool>>(fileStream!);
                    DoubleStateLife = new DoubleStateLife(grid);
                }
            }
        }

        private void Save(object obj)
        {
            if (FileUtils.TryGetSaveFile(out var fileStream))
            {
                using (fileStream)
                {
                    SerializationUtils.Save(fileStream!, DoubleStateLife.Current);
                }
            }
        }

        private void Exit(object obj)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Start(object obj)
        {
            DoubleStateLife.Start();
        }

        private void Stop(object obj)
        {
            DoubleStateLife.Stop();
        }

        private void Rand(object obj)
        {
            DoubleStateLife.Random();
        }

        private void New(object obj)
        {
            var grid = new MonoLifeGrid<bool>(new bool[DefaultFieldWidth, DefaultFieldHeight], Guid.NewGuid());
            DoubleStateLife = new DoubleStateLife(grid);
        }

        private void Infinity(object obj)
        {
            DoubleStateLife.Field(DoubleStateLife.FieldType.Infinity);
        }

        private void Bound(object obj)
        {
            DoubleStateLife.Field(DoubleStateLife.FieldType.Bound);
        }
    }
}