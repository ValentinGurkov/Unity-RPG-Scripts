namespace Util
{
    /// <summary>
    /// Container class that wraps a value and ensures initialisation is
    /// called just before first use.
    /// </summary>
    public class LazyValue<T>
    {
        private T _value;
        private bool _initialized;
        private readonly InitializerDelegate _initializer;

        public delegate T InitializerDelegate();

        /// <summary>
        /// Setup the container but don't initialise the value yet.
        /// </summary>
        /// <param name="initializer">
        /// The initializer delegate to call when first used.
        /// </param>
        public LazyValue(InitializerDelegate initializer)
        {
            _initializer = initializer;
        }

        /// <summary>
        /// Get or set the contents of this container.
        /// </summary>
        /// <remarks>
        /// Note that setting the value before initialisation will initialise
        /// the class.
        /// </remarks>
        public T Value
        {
            get
            {
                // Ensure we init before returning a value.
                ForceInit();
                return _value;
            }
            set
            {
                // Don't use default init anymore.
                _initialized = true;
                _value = value;
            }
        }

        /// <summary>
        /// Force the initialisation of the value via the delegate.
        /// </summary>
        public void ForceInit()
        {
            if (_initialized) return;
            _value = _initializer();
            _initialized = true;
        }
    }
}