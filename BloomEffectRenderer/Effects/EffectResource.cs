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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BloomEffectRenderer.Effects
{
    /// <summary>
    ///     Reperesents the bytecode of an <see cref="Effect" /> that is encapsulated inside a compiled assembly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Files that are encapsulated inside a compiled assembly are commonly known as Manifiest or embedded resources.
    ///         Since embedded resources are added to the assembly at compiled time, they can not be accidentally deleted or
    ///         misplaced. However, if the file needs to be changed, the assembly will need to be re-compiled with the changed
    ///         file.
    ///     </para>
    ///     <para>
    ///         To add an embedded resource file to an assembly, first add it to the project and then change the Build Action
    ///         in the Properties of the file to <code>Embedded Resource</code>. The next time the project is compiled, the
    ///         compiler will add the file to the assembly as an embedded resource. The compiler adds namespace(s) to the
    ///         embedded resource so it matches with the path of where the file was added to the project.
    ///     </para>
    /// </remarks>
    public class EffectResource
    {
        private static EffectResource extractEffect;
        private static EffectResource blurEffect;
        private static EffectResource combineEffect;
        private static string shaderExtension;

        public static EffectResource ExtractEffect =>
            extractEffect ?? (extractEffect =
                new EffectResource($"BloomEffectRenderer.Effects.Resources.BloomExtract.{shaderExtension}.mgfxo"));

        public static EffectResource BlurEffect =>
            blurEffect ?? (blurEffect =
                new EffectResource($"BloomEffectRenderer.Effects.Resources.GaussianBlur.{shaderExtension}.mgfxo"));

        public static EffectResource CombineEffect =>
            combineEffect ?? (combineEffect =
                new EffectResource($"BloomEffectRenderer.Effects.Resources.BloomCombine.{shaderExtension}.mgfxo"));

        static EffectResource()
        {
            DetermineShaderExtension();
        }

        private static void DetermineShaderExtension()
        {
            // Use reflection to figure out if Shader.Profile is OpenGL (0) or DirectX (1).
            // May need to be changed / fixed for future shader profiles.

            var assembly = typeof(Game).GetTypeInfo().Assembly;
            Debug.Assert(assembly != null);

            var shaderType = assembly.GetType("Microsoft.Xna.Framework.Graphics.Shader");
            Debug.Assert(shaderType != null);
            var shaderTypeInfo = shaderType.GetTypeInfo();
            Debug.Assert(shaderTypeInfo != null);

            // https://github.com/MonoGame/MonoGame/blob/develop/MonoGame.Framework/Graphics/Shader/Shader.cs#L47
            var profileProperty = shaderTypeInfo.GetDeclaredProperty("Profile");
            var value = (int) profileProperty.GetValue(null);

            switch (value)
            {
                case 0:
                    // OpenGL
                    shaderExtension = "ogl";
                    break;
                case 1:
                    // DirectX
                    shaderExtension = "dx11";
                    break;
                default:
                    throw new InvalidOperationException("Unknown shader profile.");
            }
        }

        public readonly string ResourceName;
        private volatile byte[] bytecode;
        private readonly Assembly assembly;

        public byte[] Bytecode
        {
            get
            {
                if (bytecode != null)
                    return bytecode;

                lock (this)
                {
                    if (bytecode != null)
                        return bytecode;

                    var stream = assembly.GetManifestResourceStream(ResourceName);
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        bytecode = memoryStream.ToArray();
                    }
                }

                return bytecode;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EffectResource" /> class.
        /// </summary>
        /// <param name="resourceName">The name of the embedded resource. This must include the namespace(s).</param>
        /// <param name="assembly">The assembly which the embedded resource is apart of.</param>
        public EffectResource(string resourceName, Assembly assembly = null)
        {
            ResourceName = resourceName;
            this.assembly = assembly ?? typeof(EffectResource).GetTypeInfo().Assembly;
        }
    }
}