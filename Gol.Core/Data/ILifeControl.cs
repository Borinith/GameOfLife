using Gol.Core.Controls.Models;

namespace Gol.Core.Data
{
    /// <summary>
    ///     Life control methods.
    /// </summary>
    public interface ILifeControl<TValue>
    {
        /// <summary>
        ///     Current generation.
        /// </summary>
        MonoLifeGrid<TValue>? Current { get; }

        /// <summary>
        ///     Field type.
        /// </summary>
        FieldType FieldType { get; set; }

        /// <summary>
        ///     Generation number.
        /// </summary>
        int GenerationNumber { get; set; }

        /// <summary>
        ///     Previous generation.
        /// </summary>
        MonoLifeGrid<TValue>? Previous { get; }

        /// <summary>
        ///     Type of field.
        /// </summary>
        /// <param name="fieldType"></param>
        void Field(FieldType fieldType);

        /// <summary>
        ///     Random life.
        /// </summary>
        void Random();

        /// <summary>
        ///     Set start generation.
        /// </summary>
        /// <param name="grid">Grid object reference.</param>
        void SetCurrent(MonoLifeGrid<TValue> grid);

        /// <summary>
        ///     Start\Resume life cycle.
        /// </summary>
        void Start();

        /// <summary>
        ///     Next step of life cycle.
        /// </summary>
        void Step();

        /// <summary>
        ///     Stop life.
        /// </summary>
        void Stop();
    }
}