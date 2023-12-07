namespace Gol.Core.Controls.Models
{
    /// <summary>
    ///     Offset coordinates
    /// </summary>
    /// <param name="Dx">X offset</param>
    /// <param name="Dy">Y offset</param>
    internal readonly record struct Offset(int Dx, int Dy);
}