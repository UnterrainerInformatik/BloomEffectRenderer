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
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace BloomEffectRenderer.Utils
{
    /// <summary>
    ///     This class represents a sliding potentiometer. It gets a min- and max-value.
    ///     Be careful when serializing since the order of value-assignments when deserializing is very important:
    ///     SetValue -> 300 BEFORE MaxValue -> 400 will clamp the value to 100 (the default-value of MaxValue).
    /// </summary>
    [PublicAPI]
    public class Fader
    {
        /// <summary>
        ///     Occurs when the value of the fader has changed. Happens on every
        ///     occasion the value is set.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<double>> ValueChanged;

        private const double A_THIRD = 1d / 3d;

        private Interval<double> interval = new Interval<double>(0f, 100f);
        private double value;
        private double percentage;
        private readonly bool check = true;

        public bool IsInverted { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Fader" /> class.
        ///     Don't use this constructor in everyday use since it turns off plausibility-checking.
        ///     It's mainly for deserialization-purposes.
        /// </summary>
        public Fader()
        {
            check = false;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Fader" /> class.
        /// </summary>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        public Fader(double minValue, double maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Fader" /> class.
        /// </summary>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <param name="value">The value.</param>
        /// <param name="valueIsPercentage">
        ///     if set to <c>true</c> [value is percentage]. If <see langword="true" />, the value you set via the parameter
        ///     'value'
        ///     is the percentage of the slider.
        /// </param>
        public Fader(double minValue, double maxValue, double value, bool valueIsPercentage = false)
        {
            // Avoid ArgumentException when the new min value is higher than the default max value.
            if (minValue > MaxValue)
            {
                MaxValue = maxValue;
                MinValue = minValue;
            }
            else
            {
                MinValue = minValue;
                MaxValue = maxValue;
            }

            if (valueIsPercentage)
            {
                Percentage = value;
            }
            else
            {
                Value = value;
            }
        }

        /// <summary>
        ///     Gets or sets the min value.
        ///     Default value is zero.
        /// </summary>
        /// <value>
        ///     The min value.
        /// </value>
        public double MinValue
        {
            get { return interval.Min; }
            set
            {
                interval.Min = value;

                if (check)
                {
                    // Ensure value is still in range..
                    if (Value < interval.Min)
                    {
                        Value = interval.Min;
                    }
                }
                percentage = GetPercentageAtValue(value);
            }
        }

        /// <summary>
        ///     Gets or sets the max value.
        ///     Default value is 100.
        /// </summary>
        /// <value>The max value.</value>
        public double MaxValue
        {
            get { return interval.Max; }
            set
            {
                interval.Max = value;

                if (check)
                {
                    // Ensure value is still in range..
                    if (Value > interval.Max)
                    {
                        Value = interval.Max;
                    }
                }
                percentage = GetPercentageAtValue(value);
            }
        }

        /// <summary>
        ///     Gets or sets the faders value. Automatically recalculates the
        ///     <see cref="Percentage" />.
        /// </summary>
        /// <value>The current value of the slider.</value>
        public double Value
        {
            get { return value; }
            set
            {
                double oldValue = this.value;

                if (check)
                {
                    // Make sure that the value does not stray outside the valid range...
                    if (value < interval.Min)
                    {
                        value = interval.Min;
                    }
                    else if (value > interval.Max)
                    {
                        value = interval.Max;
                    }
                }

                this.value = value;
                percentage = GetPercentageAtValue(value);
                ValueChanged?.Invoke(this, new ValueChangedEventArgs<double>(oldValue));
            }
        }

        /// <summary>
        ///     Gets or sets the quadratic value of the fader: y = x * x
        /// </summary>
        [XmlIgnore]
        public double QuadraticValue
        {
            get { return GetValueAtPercentage(Percentage * Percentage); }
            set { Percentage = Math.Sqrt(GetPercentageAtValue(value)); }
        }

        /// <summary>
        ///     Gets or sets the cubic value of the fader: y = x * x * x
        /// </summary>
        [XmlIgnore]
        public double CubicValue
        {
            get { return GetValueAtPercentage(Percentage * Percentage * Percentage); }
            set { Percentage = Math.Pow(GetPercentageAtValue(value), A_THIRD); }
        }

        /// <summary>
        ///     Gets or sets the exponential value of the fader: y = (20^x - 1) / 20
        /// </summary>
        [XmlIgnore]
        public double ExponentialValue
        {
            get { return GetValueAtPercentage((Math.Pow(20d, Percentage) - 1.0) / 20.0); }
            set { Percentage = Math.Log((20.0 * GetPercentageAtValue(value)) + 1.0) / Math.Log(20.0); }
        }

        /// <summary>
        ///     Gets the bidirectional slow start: y = (cos((x - 1)* PI) + 1) / 2
        ///     (No setter)
        /// </summary>
        [XmlIgnore]
        public double BidirectionalSlow
        {
            get { return GetValueAtPercentage((Math.Cos((Percentage - 1.0) * Math.PI) + 1.0) / 2.0); }
            set { Percentage = 1.0 - (Math.Acos(2.0 * GetPercentageAtValue(value) - 1.0) / Math.PI); }
        }

        /// <summary>
        ///     Gets the bidirectional quick start: y = ((2x - 1)^3+1)/2
        /// </summary>
        [XmlIgnore]
        public double BidirectionalQuick
        {
            get { return GetValueAtPercentage((Math.Pow((2.0 * Percentage - 1.0), 3.0) + 1.0) / 2.0); }
            set { Percentage = (Math.Pow(2.0 * GetPercentageAtValue(value) - 1.0, A_THIRD) + 1.0) / 2.0; }
        }

        /// <summary>
        ///     Gets or sets the percentage for the slider. A double value between 0.0
        ///     and 1.0.
        /// </summary>
        /// <value>The percentage done of the slider.</value>
        public double Percentage
        {
            get { return percentage; }
            set { Value = GetValueAtPercentage(value); }
        }

        /// <summary>
        ///     Getter for the value of a certain percentage.
        /// </summary>
        /// <param name="percent">The percentage.</param>
        /// <returns>What the value of the fader would be if it was at the given percentage.</returns>
        private double GetValueAtPercentage(double percent)
        {
            if (IsInverted)
            {
                return interval.Min + (1f - percent) * (interval.Max - interval.Min);
            }
            return interval.Min + percent * (interval.Max - interval.Min);
        }

        /// <summary>
        ///     Getter for the percentage of a certain value.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns>What the percentage of the fader would be if it was at the given value.</returns>
        private double GetPercentageAtValue(double val)
        {
            if (IsInverted)
            {
                return 1f - ((val - interval.Min) / (interval.Max - interval.Min));
            }
            return (val - interval.Min) / (interval.Max - interval.Min);
        }
    }
}