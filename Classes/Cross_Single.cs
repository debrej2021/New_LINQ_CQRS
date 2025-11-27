using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Quick_ref_all.Classes
{
    /// <summary>
    /// Represents a thread-safe, lazily initialized singleton instance of the <see cref="Cross_Single"/> class.
    /// </summary>
    /// <remarks>This class provides a singleton pattern implementation using the <see cref="Lazy{T}"/> class
    /// to ensure that the instance is created only when it is first accessed. The <see cref="Lazy{T}"/> guarantees
    /// thread safety during initialization, making this implementation suitable for concurrent environments.</remarks>
    public sealed class Cross_Single
    {
        /// <summary>
        /// Provides a lazily initialized, thread-safe singleton instance of the <see cref="Cross_Single"/> class.
        /// </summary>
        /// <remarks>The singleton instance is created only when it is accessed for the first time,
        /// ensuring efficient resource usage. This implementation leverages the <see cref="Lazy{T}"/> class to
        /// guarantee thread safety during initialization.</remarks>
        private static readonly Lazy<Cross_Single> _instance = new Lazy<Cross_Single>();

        /// <summary>
        /// Gets a lazily initialized instance of the <see cref="Cross_Single"/> class.
        /// </summary>
        /// <remarks>The <see cref="Lazy{T}"/> ensures that the <see cref="Cross_Single"/> instance is
        /// created only when it is first accessed,  providing a mechanism for deferred initialization and improved
        /// performance in scenarios where the instance may not always be needed.</remarks>
        public static Lazy<Cross_Single> Instance => new Lazy<Cross_Single>();
        /// <summary>
        /// Private constructor to prevent external instantiation.
        /// </summary>
        private Cross_Single()
        {
        }

    }
}
