namespace Utils
{
    class InvokeOnce
    {
        /// <summary>
        /// State of InvokeOnce.
        /// </summary>
        public bool WasInvoked { get; set; } = false;

        /// <summary>
        /// Invokes only ones.
        /// </summary>
        public void Invoke(System.Action action)
        {
            if (!WasInvoked)
            {
                action?.Invoke();
                WasInvoked = true;
            }
        }

        /// <summary>
        /// Can invoke multiple times.
        /// </summary>
        public void SneakyInvoke(System.Action action)
        {
            if (!WasInvoked)
            {
                action?.Invoke();
            }
        }

        /// <summary>
        /// Make it inkvoke again.
        /// </summary>
        public void Reset()
        {
            WasInvoked = false;
        }
    }
}
