using Gol.Core.Controls.Models;
using Gol.Core.Data;
using Gol.Core.Prism;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Gol.Core.Algorithm
{
    /// <summary>
    ///     Black and White life cycle algorithm.
    /// </summary>
    public class DoubleStateLife : NotificationObject, ILifeControl<bool>
    {
        //    -1   0   1
        // -1 [ ] [ ] [ ]
        //  0 [ ] [X] [ ]
        //  1 [ ] [ ] [ ]
        private readonly Offset[] _offsets =
        {
            // Line 1
            new(-1, -1),
            new(0, -1),
            new(1, -1),

            // Line 2
            new(-1, 0),
            new(1, 0),

            // Line 3
            new(-1, 1),
            new(0, 1),
            new(1, 1)
        };

        private readonly TimeSpan _realtimeDelay = TimeSpan.FromMilliseconds(35);
        private FieldType _boundType;
        private MonoLifeGrid<bool>? _current;
        private int _generationNumber;
        private MonoLifeGrid<bool>? _previous;
        private bool _stopUpdateTimer;

        /// <summary>
        ///     Constructor for <see cref="DoubleStateLife" />.
        /// </summary>
        public DoubleStateLife(MonoLifeGrid<bool>? current) : this()
        {
            SetCurrent(current);
        }

        /// <summary>
        ///     Constructor for <see cref="DoubleStateLife" />.
        /// </summary>
        private DoubleStateLife()
        {
        }

        /// <summary>
        ///     Bound type.
        /// </summary>
        public FieldType BoundType
        {
            get => _boundType;

            set
            {
                _boundType = value;
                RaisePropertyChanged(nameof(BoundType));
            }
        }

        /// <summary>
        ///     Generation number.
        /// </summary>
        public int GenerationNumber
        {
            get => _generationNumber;

            set
            {
                if (_generationNumber != value)
                {
                    _generationNumber = value;
                    RaisePropertyChanged(nameof(GenerationNumber));
                }
            }
        }

        /// <inheritdoc />
        public MonoLifeGrid<bool>? Previous
        {
            get => _previous;

            private set
            {
                if (_previous != value)
                {
                    _previous = value;
                    RaisePropertyChanged(nameof(Previous));
                }
            }
        }

        /// <inheritdoc />
        public MonoLifeGrid<bool>? Current
        {
            get => _current;

            private set
            {
                if (_current != value)
                {
                    _current = value;
                    RaisePropertyChanged(nameof(Current));
                }
            }
        }

        /// <inheritdoc />
        public void SetCurrent(MonoLifeGrid<bool>? grid)
        {
            if (grid == null)
            {
                Stop();
            }
            else
            {
                GenerationNumber = 0;
                Current = grid;
            }
        }

        /// <inheritdoc />
        public async void Start()
        {
            if (Current == null || Current.Height == 0 || Current.Width == 0)
            {
                return;
            }

            _stopUpdateTimer = false;
            await TimerElapsed();
        }

        /// <inheritdoc />
        public void Stop()
        {
            _stopUpdateTimer = true;
        }

        /// <inheritdoc />
        public async void Random()
        {
            await Task.Run(() =>
            {
                _previous = _current;
                _current = _current!.Clone();

                var random = new Random();

                for (var i = 0; i < _current.Width; i++)
                {
                    for (var j = 0; j < _current.Height; j++)
                    {
                        // Will be true 25% of the time
                        _current[i, j] = random.Next(100) < 25;
                    }
                }

                RaisePropertyChanged(nameof(Current));
                RaisePropertyChanged(nameof(Previous));
            });
        }

        public void Field(FieldType fieldType)
        {
            BoundType = fieldType;
            NotificationObjectStatic.OnStaticPropertyChanged(nameof(BoundType));
        }

        private int IsBlack(int x, int y, MonoLifeGrid<bool> field)
        {
            //BoundType = BoundType;
            //NotificationObjectStatic.OnStaticPropertyChanged(nameof(BoundType));

            if (BoundType == FieldType.Infinity)
            {
                if (x < 0)
                {
                    x += field.Width;
                }
                else if (x == field.Width)
                {
                    x -= field.Width;
                }

                if (y < 0)
                {
                    y += field.Height;
                }
                else if (y == field.Height)
                {
                    y -= field.Height;
                }
            }

            if (0 <= x && x < field.Width && 0 <= y && y < field.Height)
            {
                return field[x, y] ? 1 : 0;
            }

            return 0;
        }

        private async Task TimerElapsed()
        {
            while (!_stopUpdateTimer)
            {
                await LifeStep();
                await Task.Delay(_realtimeDelay);
            }
        }

        private int NearCells(int x, int y, MonoLifeGrid<bool> field)
        {
            return _offsets.Sum(offset => IsBlack(x + offset.Dx, y + offset.Dy, field));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private Task LifeStep()
        {
            return Task.Run(() =>
            {
                /*
                https://ru.wikipedia.org/wiki/%D0%96%D0%B8%D0%B7%D0%BD%D1%8C_(%D0%B8%D0%B3%D1%80%D0%B0)
                - В пустой (мёртвой) клетке, рядом с которой ровно три живые клетки, зарождается жизнь;
                - Если у живой клетки есть две или три живые соседки, то эта клетка продолжает жить; в противном случае, если соседей 
                  меньше двух или больше трёх, клетка умирает («от одиночества» или «от перенаселённости»)
                */
                _previous = _current;
                _current = _current!.Clone();

                for (var i = 0; i < _current.Width; i++)
                {
                    for (var j = 0; j < _current.Height; j++)
                    {
                        var nearCells = NearCells(i, j, Previous!);
                        var isCreature = Previous![i, j];

                        if (isCreature)
                        {
                            // Если у живой клетки есть две или три живые соседки, то эта клетка продолжает жить;
                            _current[i, j] = nearCells == 2 || nearCells == 3;
                        }
                        else
                        {
                            // В пустой (мёртвой) клетке, рядом с которой ровно три живые клетки, зарождается жизнь;
                            _current[i, j] = nearCells == 3;
                        }
                    }
                }

                GenerationNumber++;

                RaisePropertyChanged(nameof(Current));
                RaisePropertyChanged(nameof(Previous));
            });
        }
    }
}