namespace Gol.Core.Data
{
    /// <summary>
    ///     Array extensions.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        ///     Convert multi-dimensional array to jagged.
        /// </summary>
        /// <typeparam name="T">Array type.</typeparam>
        /// <param name="twoDimensionalArray">Two-dimensional array.</param>
        /// <returns>Jagged array.</returns>
        public static T[][] ToJaggedArray<T>(this T[,] twoDimensionalArray)
        {
            var rowsFirstIndex = twoDimensionalArray.GetLowerBound(0);
            var rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
            var numberOfRows = rowsLastIndex + 1;

            var columnsFirstIndex = twoDimensionalArray.GetLowerBound(1);
            var columnsLastIndex = twoDimensionalArray.GetUpperBound(1);
            var numberOfColumns = columnsLastIndex + 1;

            var jaggedArray = new T[numberOfRows][];

            for (var i = rowsFirstIndex; i <= rowsLastIndex; i++)
            {
                jaggedArray[i] = new T[numberOfColumns];

                for (var j = columnsFirstIndex; j <= columnsLastIndex; j++)
                {
                    jaggedArray[i][j] = twoDimensionalArray[i, j];
                }
            }

            return jaggedArray;
        }
    }
}