// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using System;
using JetBrains.Annotations;

namespace BloomEffectRenderer.Utils
{
    /// <summary>
    ///     This is a helper object that represents a mathematical interval.
    /// </summary>
    /// <typeparam name="T">The type of the interval.</typeparam>
    [PublicAPI]
    public struct Interval<T> where T : IComparable<T>
    {
        private T maximalValue;
        private T minimalValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Interval&lt;T&gt;" /> structure.
        /// </summary>
        /// <param name="min">The minimal value.</param>
        /// <param name="max">The maximal value.</param>
        /// <param name="isMinValueExclusive">
        ///     if set to <c>true</c> the lower bound is exclusive, otherwise its inclusive.
        /// </param>
        /// <param name="isMaxValueExclusive">
        ///     if set to <c>true</c> the upper bound is exclusive, otherwise its inclusive.
        /// </param>
        /// <exception cref="ArgumentException">Min has to be smaller than or equal to max.</exception>
        public Interval(T min, T max, bool isMinValueExclusive, bool isMaxValueExclusive)
        {
            this.IsMinValueExclusive = isMinValueExclusive;
            this.IsMaxValueExclusive = isMaxValueExclusive;
            // min > max OR max < min
            if (min.CompareTo(max) > 0 || max.CompareTo(min) < 0)
            {
                throw new ArgumentException("Min has to be smaller than or equal to max.");
            }
            minimalValue = min;
            maximalValue = max;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Interval&lt;T&gt;" /> structure.
        ///     The default way this structure treats the bounds is set to inclusive.
        /// </summary>
        /// <param name="min">The minimal value.</param>
        /// <param name="max">The maximal value.</param>
        /// <exception cref="ArgumentException">Min has to be smaller than or equal to max.</exception>
        public Interval(T min, T max)
        {
            IsMinValueExclusive = false;
            IsMaxValueExclusive = false;
            // min > max OR max < min
            if (min.CompareTo(max) > 0 || max.CompareTo(min) < 0)
            {
                throw new ArgumentException("Min has to be smaller than or equal to max.");
            }
            minimalValue = min;
            maximalValue = max;
        }

        /// <summary>
        ///     Gets or sets the minimal value. Adjusts the max-value as well if it
        ///     should currently be smaller than the given value.
        /// </summary>
        /// <value>The minimal value.</value>
        public T Min
        {
            get { return minimalValue; }
            set
            {
                if (value.CompareTo(maximalValue) > 0)
                {
                    maximalValue = value;
                }
                minimalValue = value;
            }
        }

        /// <summary>
        ///     Gets or sets the maximal value. Adjusts the min-value as well if it
        ///     should currently be greater than the given value.
        /// </summary>
        /// <value>The maximal value.</value>
        public T Max
        {
            get { return maximalValue; }
            set
            {
                if (value.CompareTo(minimalValue) < 0)
                {
                    minimalValue = value;
                }
                maximalValue = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the min value is inclusive
        ///     (within the interval) or not.
        /// </summary>
        /// <value>
        ///     if set to <c>true</c> the lower bound is exclusive, otherwise its inclusive.
        /// </value>
        public bool IsMinValueExclusive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the max value is inclusive
        ///     (within the interval) or not.
        /// </summary>
        /// <value>
        ///     if set to <c>true</c> the upper bound is exclusive, otherwise its inclusive.
        /// </value>
        public bool IsMaxValueExclusive { get; set; }

        /// <summary>
        ///     Determines whether a specified value is in between the intervals
        ///     boundaries.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <c>true</c> if the given value is in between the specified value; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool IsInBetween(T value)
        {
            return IsInBetween(value, IsMinValueExclusive, IsMaxValueExclusive);
        }

        /// <summary>
        ///     Determines whether a specified value is in between the intervals
        ///     boundaries.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minValueExclusive">
        ///     if set to <c>true</c> treats the lower boundary as exclusive.
        /// </param>
        /// <param name="maxValueExclusive">
        ///     if set to <c>true</c> treats the upper boundary as exclusive.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the given value is in between the specified value; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool IsInBetween(T value, bool minValueExclusive, bool maxValueExclusive)
        {
            bool isGreaterThanMin;
            bool isSmallerThanMax;
            if (minValueExclusive)
            {
                isGreaterThanMin = value.CompareTo(Min) > 0;
            }
            else
            {
                isGreaterThanMin = value.CompareTo(Min) >= 0;
            }

            if (!isGreaterThanMin)
            {
                return false;
            }

            if (maxValueExclusive)
            {
                isSmallerThanMax = value.CompareTo(Max) < 0;
            }
            else
            {
                isSmallerThanMax = value.CompareTo(Max) <= 0;
            }
            return isSmallerThanMax;
        }
    }
}