namespace Utils
{
    class InvokeOnce
    {
        public bool WasInvoked { get; set; } = false;

        public void Invoke(System.Action action)
        {
            if (!WasInvoked)
            {
                action?.Invoke();
                WasInvoked = true;
            }
        }

        public void Reset()
        {
            WasInvoked = false;
        }
    }
}
