using System.Collections;

namespace LanternExtractor.Infrastructure
{
    /// <summary>
    /// Analyzes the bits of an integer
    /// </summary>
    public class BitAnalyzer
    {
        /// <summary>
        /// The bit array, created when class is created
        /// </summary>
        private readonly BitArray _bitArray;

        /// <summary>
        /// The constructor taking in an integer
        /// </summary>
        /// <param name="integer">The integer to be analyzed</param>
        public BitAnalyzer(int integer)
        {
            _bitArray = new BitArray(new[] {integer});
        }

        /// <summary>
        /// Returns whether or not the bit in a specific position is set
        /// </summary>
        /// <param name="position">The position of the bit to check</param>
        /// <returns></returns>
        public bool IsBitSet(int position)
        {
            return _bitArray.Get(position);
        }
    }
}