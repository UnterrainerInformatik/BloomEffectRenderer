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
using BloomEffectRenderer.Effects;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BloomEffectRenderer
{
    [PublicAPI]
    public class Renderer
    {
        public RenderTarget2D BloomRenderTarget1 { get; private set; }
        public RenderTarget2D BloomRenderTarget2 { get; private set; }

        private Effect CombineEffect { get; set; }
        private Effect ExtractEffect { get; set; }
        private Effect GaussianBlurEffect { get; set; }

        public void Initialize(GraphicsDevice gd, Point resolution)
        {
            // Create two render-targets for the bloom processing. These are half the
            // size of the back-Buffer, in order to minimize fill-rate costs. Reducing
            // the resolution in this way doesn't hurt quality, because we are going
            // to be blurring the bloom images anyway.
            BloomRenderTarget1 = new RenderTarget2D(gd,
                width: resolution.X / 2,
                height: resolution.Y / 2,
                mipMap: false,
                preferredFormat: SurfaceFormat.Color,
                preferredDepthFormat: DepthFormat.None);
            BloomRenderTarget2 = new RenderTarget2D(gd,
                width: resolution.X / 2,
                height: resolution.Y / 2,
                mipMap: false,
                preferredFormat: SurfaceFormat.Color,
                preferredDepthFormat: DepthFormat.None);
        }

        public delegate void DebugDelegate(RenderTarget2D currentTarget, RenderPhase phase);

        public void Render(GraphicsDevice gd, SpriteBatch sb, string name, Texture2D irt, RenderTarget2D ort,
            Settings s, DebugDelegate debugDelegate = null)
        {
            Clear(gd);
            // Pass 1: draw the scene into render-target 1, using a
            // shader that extracts only the brightest parts of the image.
            ExtractEffect.Parameters["BloomThreshold"].SetValue((float) s.BloomThreshold.Value);
            DrawFullscreenQuad(gd, sb, irt, BloomRenderTarget1, ExtractEffect);
            debugDelegate?.Invoke(BloomRenderTarget1, RenderPhase.EXTRACT);

            // Pass 2: draw from render-target 1 into render-target 2,
            // using a shader to apply a horizontal Gaussian blur filter.
            SetBlurEffectParameters(1.0f / BloomRenderTarget1.Width, 0, s);
            // The Sampler has to be on anisotropic lookup to smooth the pixels for the blur correctly.
            DrawFullscreenQuad(gd,
                sb,
                BloomRenderTarget1,
                BloomRenderTarget2,
                GaussianBlurEffect,
                null,
                SamplerState.AnisotropicClamp);
            debugDelegate?.Invoke(BloomRenderTarget2, RenderPhase.BLUR_HORIZONTAL);

            // Pass 3: draw from render-target 2 back into render-target 1,
            // using a shader to apply a vertical Gaussian blur filter.
            SetBlurEffectParameters(0, 1.0f / BloomRenderTarget2.Height, s);
            // The Sampler has to be on anisotropic lookup to smooth the pixels for the blur correctly.
            DrawFullscreenQuad(gd,
                sb,
                BloomRenderTarget2,
                BloomRenderTarget1,
                GaussianBlurEffect,
                null,
                SamplerState.AnisotropicClamp);
            debugDelegate?.Invoke(BloomRenderTarget1, RenderPhase.BLUR_VERTICAL);

            // Pass 4: draw both render-target 1 and the original scene
            // image back into the main back-Buffer, using a shader that
            // combines them to produce the final bloomed result.
            var parameters = CombineEffect.Parameters;
            parameters["BloomIntensity"].SetValue((float) s.BloomIntensity.Value);
            parameters["BaseIntensity"].SetValue((float) s.BaseIntensity.Value);
            parameters["BloomSaturation"].SetValue((float) s.BloomSaturation.Value);
            parameters["BaseSaturation"].SetValue((float) s.BaseSaturation.Value);
            parameters[5].SetValue(irt);

            DrawFullscreenQuad(gd, sb, BloomRenderTarget1, ort, CombineEffect);
            debugDelegate?.Invoke(ort, RenderPhase.COMBINE);
        }

        private void Clear(GraphicsDevice gd)
        {
            Clear(gd, BloomRenderTarget1);
            Clear(gd, BloomRenderTarget2);
        }

        public static void Clear(GraphicsDevice gd, RenderTarget2D rt)
        {
            gd.SetRenderTarget(rt);
            gd.Clear(Color.Black);
        }

        /// <summary>
        ///     Helper for drawing a texture into a renderTarget, using a custom shader to apply postProcessing effects.
        /// </summary>
        public static void DrawFullscreenQuad(GraphicsDevice gd, SpriteBatch sb, Texture2D t,
            RenderTarget2D renderTarget, Effect effect = null, BlendState blendState = null,
            SamplerState samplerState = null)
        {
            gd.SetRenderTarget(renderTarget);
            gd.Clear(Color.Black);

            sb.Begin(SpriteSortMode.Immediate,
                blendState ?? BlendState.Opaque,
                samplerState ?? SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                effect);

            int w, h;
            if (renderTarget == null)
            {
                w = gd.PresentationParameters.BackBufferWidth;
                h = gd.PresentationParameters.BackBufferHeight;
            }
            else
            {
                w = renderTarget.Width;
                h = renderTarget.Height;
            }

            if (t != null)
                sb.Draw(t,
                    new Rectangle(0, 0, w, h),
                    new Rectangle(0, 0, t.Width, t.Height),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    1f);
            sb.End();
        }

        /// <summary>
        ///     Computes sample weightings and texture coordinate offsets for one pass of a separable Gaussian blur filter.
        /// </summary>
        private void SetBlurEffectParameters(float dx, float dy, Settings settings)
        {
            // Look up the sample weight and offset effect parameters.
            var weightsParameter = GaussianBlurEffect.Parameters["SampleWeights"];
            var offsetsParameter = GaussianBlurEffect.Parameters["SampleOffsets"];

            // Look up how many samples our Gaussian blur effect supports.
            var sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter settings.
            var sampleWeights = new float[sampleCount];
            var sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0, settings);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            var totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (var i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                var weight = ComputeGaussian(i + 1, settings);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                var sampleOffset = i * 2 + 1.5f;

                var delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (var i = 0; i < sampleWeights.Length; i++)
                sampleWeights[i] /= totalWeights;

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }

        /// <summary>
        ///     Evaluates a single point on the Gaussian falloff curve. Used for setting up the blur filter weightings.
        /// </summary>
        private static float ComputeGaussian(float n, Settings settings)
        {
            var theta = settings.BlurAmount.Value;
            return (float) (1.0 / Math.Sqrt(2 * Math.PI * theta) * Math.Exp(-(n * n) / (2 * theta * theta)));
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            ExtractEffect = new Effect(graphicsDevice, EffectResource.ExtractEffect.Bytecode);
            GaussianBlurEffect = new Effect(graphicsDevice, EffectResource.BlurEffect.Bytecode);
            CombineEffect = new Effect(graphicsDevice, EffectResource.CombineEffect.Bytecode);
        }

        public void UnloadContent()
        {
            BloomRenderTarget1?.Dispose();
            BloomRenderTarget2?.Dispose();
        }
    }
}