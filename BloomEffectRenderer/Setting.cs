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

using Faders;

namespace BloomEffectRenderer
{
    /// <summary>
    ///     Class holds all the settings used to tweak the bloom effect.
    /// </summary>
    public class Setting
    {
        /// <summary>
        ///     Table of preset bloom settings, used by the sample program.
        /// </summary>
        public static readonly Setting[] PRESET_SETTING =
        {
            new Setting(name: "Default",
                bloomThreshold: 0.25f,
                blurAmount: 4,
                bloomIntensity: 1.25f,
                baseIntensity: 1,
                bloomSaturation: 1,
                baseSaturation: 1),
            new Setting(name: "Soft",
                bloomThreshold: 0,
                blurAmount: 3,
                bloomIntensity: 1,
                baseIntensity: 1,
                bloomSaturation: 1,
                baseSaturation: 1),
            new Setting(name: "Desaturated",
                bloomThreshold: 0.5f,
                blurAmount: 8,
                bloomIntensity: 2,
                baseIntensity: 1,
                bloomSaturation: 0,
                baseSaturation: 1),
            new Setting(name: "Saturated",
                bloomThreshold: 0.25f,
                blurAmount: 4,
                bloomIntensity: 2,
                baseIntensity: 1,
                bloomSaturation: 2,
                baseSaturation: 0),
            new Setting(name: "Blurry",
                bloomThreshold: 0,
                blurAmount: 2,
                bloomIntensity: 1,
                baseIntensity: 0.1f,
                bloomSaturation: 1,
                baseSaturation: 1),
            new Setting(name: "Subtle",
                bloomThreshold: 0.5f,
                blurAmount: 2,
                bloomIntensity: 1,
                baseIntensity: 1,
                bloomSaturation: 1,
                baseSaturation: 1),
            new Setting(name: "Subtle-",
                bloomThreshold: 0.5f,
                blurAmount: 1.6f,
                bloomIntensity: 0.8f,
                baseIntensity: 1,
                bloomSaturation: 1,
                baseSaturation: 1),
            new Setting(name: "Subtle+",
                bloomThreshold: 0.5f,
                blurAmount: 2,
                bloomIntensity: 1,
                baseIntensity: 1.3f,
                bloomSaturation: 1,
                baseSaturation: 1.5f),
            new Setting(name: "Desaturated++",
                bloomThreshold: 0.25f,
                blurAmount: 4,
                bloomIntensity: 2,
                baseIntensity: 1,
                bloomSaturation: 2,
                baseSaturation: 0)
        };

        /// <summary>
        ///     Constructs a new bloom settings descriptor.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="bloomThreshold">
        ///     The bloom threshold.<br />Controls how bright a pixel needs to be before it will bloom.
        ///     Zero makes everything bloom equally, while higher values select
        ///     only brighter colors. Somewhere between 0.25 and 0.5 is good.
        /// </param>
        /// <param name="blurAmount">
        ///     The blur amount.<br />Controls how much blurring is applied to the bloom image.
        ///     The typical range is from 1 up to 10 or so.
        /// </param>
        /// <param name="bloomIntensity">
        ///     The bloom intensity.<br />Controls the amount of the bloom and base images that
        ///     will be mixed into the final scene. Range 0 to 3.
        /// </param>
        /// <param name="baseIntensity">
        ///     The base intensity.<br />Controls the amount of the bloom and base images that
        ///     will be mixed into the final scene. Range 0 to 3.
        /// </param>
        /// <param name="bloomSaturation">
        ///     The bloom saturation.<br />Independently control the color saturation of the bloom and
        ///     base images. Zero is totally desaturated, 1.0 leaves saturation
        ///     unchanged, while higher values increase the saturation level.
        /// </param>
        /// <param name="baseSaturation">
        ///     The base saturation.<br />Independently control the color saturation of the bloom and
        ///     base images. Zero is totally desaturated, 1.0 leaves saturation
        ///     unchanged, while higher values increase the saturation level.
        /// </param>
        public Setting(string name, float bloomThreshold, float blurAmount, float bloomIntensity, float baseIntensity,
            float bloomSaturation, float baseSaturation)
        {
            BaseIntensity = new Fader(0f, 3f);
            BaseSaturation = new Fader(0f, 10f);
            BloomSaturation = new Fader(0f, 10f);
            BloomIntensity = new Fader(0f, 3f);
            BlurAmount = new Fader(0f, 10f);
            BloomThreshold = new Fader(0f, 1f);
            Name = name;
            BloomThreshold.Value = bloomThreshold;
            BlurAmount.Value = blurAmount;
            BloomIntensity.Value = bloomIntensity;
            BaseIntensity.Value = baseIntensity;
            BloomSaturation.Value = bloomSaturation;
            BaseSaturation.Value = baseSaturation;
        }

        /// <summary>
        ///     QueueName of a preset bloom setting, for display to the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets the bloom threshold.
        ///     Controls how bright a pixel needs to be before it will bloom.
        ///     Zero makes everything bloom equally, while higher values select
        ///     only brighter colors. Somewhere between 0.25 and 0.5 is good.
        /// </summary>
        /// <value>The bloom threshold.</value>
        public Fader BloomThreshold { get; }

        /// <summary>
        ///     Gets the blur amount.
        ///     Controls how much blurring is applied to the bloom image.
        ///     The typical range is from 1 up to 10 or so.
        /// </summary>
        /// <value>The blur amount.</value>
        public Fader BlurAmount { get; }

        /// <summary>
        ///     Gets the bloom intensity.
        ///     Controls the amount of the bloom and base images that
        ///     will be mixed into the final scene. Range 0 to 3.
        /// </summary>
        /// <value>The bloom intensity.</value>
        public Fader BloomIntensity { get; }

        /// <summary>
        ///     Gets the base intensity.
        ///     Controls the amount of the bloom and base images that
        ///     will be mixed into the final scene. Range 0 to 3.
        /// </summary>
        /// <value>The base intensity.</value>
        public Fader BaseIntensity { get; }

        /// <summary>
        ///     Gets the bloom saturation.
        ///     Independently control the color saturation of the bloom and
        ///     base images. Zero is totally desaturated, 1.0 leaves saturation
        ///     unchanged, while higher values increase the saturation level.
        /// </summary>
        /// <value>The bloom saturation.</value>
        public Fader BloomSaturation { get; }

        /// <summary>
        ///     Gets the base saturation.
        ///     Independently control the color saturation of the bloom and
        ///     base images. Zero is totally desaturated, 1.0 leaves saturation
        ///     unchanged, while higher values increase the saturation level.
        /// </summary>
        /// <value>The base saturation.</value>
        public Fader BaseSaturation { get; }

        /// <summary>
        ///     Sets the values of all sliders to the values of the given setting.
        /// </summary>
        /// <param name="other">The other setting to copy the values from.</param>
        public void CopyFrom(Setting other)
        {
            Name = other.Name;
            BloomThreshold.Value = other.BloomThreshold.Value;
            BlurAmount.Value = other.BlurAmount.Value;
            BloomIntensity.Value = other.BloomIntensity.Value;
            BaseIntensity.Value = other.BaseIntensity.Value;
            BloomSaturation.Value = other.BloomSaturation.Value;
            BaseSaturation.Value = other.BaseSaturation.Value;
        }

        /// <summary>
        ///     Sets the min- and max-values of all the sliders within this setting.
        /// </summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        public void SetMinAndMax(Setting min, Setting max)
        {
            if (min.BloomThreshold.Value > max.BloomThreshold.Value)
            {
                Switch(min.BloomThreshold, max.BloomThreshold);
                BloomThreshold.IsInverted = true;
            }

            if (min.BlurAmount.Value > max.BlurAmount.Value)
            {
                Switch(min.BlurAmount, max.BlurAmount);
                BlurAmount.IsInverted = true;
            }

            if (min.BloomIntensity.Value > max.BloomIntensity.Value)
            {
                Switch(min.BloomIntensity, max.BloomIntensity);
                BloomIntensity.IsInverted = true;
            }

            if (min.BaseIntensity.Value > max.BaseIntensity.Value)
            {
                Switch(min.BaseIntensity, max.BaseIntensity);
                BaseIntensity.IsInverted = true;
            }

            if (min.BloomSaturation.Value > max.BloomSaturation.Value)
            {
                Switch(min.BloomSaturation, max.BloomSaturation);
                BloomSaturation.IsInverted = true;
            }

            if (min.BaseSaturation.Value > max.BaseSaturation.Value)
            {
                Switch(min.BaseSaturation, max.BaseSaturation);
                BaseSaturation.IsInverted = true;
            }

            BloomThreshold.MinValue = min.BloomThreshold.Value;
            BlurAmount.MinValue = min.BlurAmount.Value;
            BloomIntensity.MinValue = min.BloomIntensity.Value;
            BaseIntensity.MinValue = min.BaseIntensity.Value;
            BloomSaturation.MinValue = min.BloomSaturation.Value;
            BaseSaturation.MinValue = min.BaseSaturation.Value;

            BloomThreshold.MaxValue = max.BloomThreshold.Value;
            BlurAmount.MaxValue = max.BlurAmount.Value;
            BloomIntensity.MaxValue = max.BloomIntensity.Value;
            BaseIntensity.MaxValue = max.BaseIntensity.Value;
            BloomSaturation.MaxValue = max.BloomSaturation.Value;
            BaseSaturation.MaxValue = max.BaseSaturation.Value;
        }

        private static void Switch(Fader a, Fader b)
        {
            var t = a.Value;
            a.Value = b.Value;
            b.Value = t;
        }

        /// <summary>
        ///     Sets the percentage of all the sliders within this setting.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        public void SetPercentage(float percentage)
        {
            BloomThreshold.Percentage = percentage;
            BlurAmount.Percentage = percentage;
            BloomIntensity.Percentage = percentage;
            BaseIntensity.Percentage = percentage;
            BloomSaturation.Percentage = percentage;
            BaseSaturation.Percentage = percentage;
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Name} Th:{BloomThreshold.Value,2} Br:{BlurAmount.Value,2} BlI:{BloomIntensity.Value,2} " +
                   $"BaI:{BaseIntensity.Value,2} BlS:{BloomSaturation.Value,2} BaS:{BaseSaturation.Value,2}";
        }
    }
}