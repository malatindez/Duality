namespace Utils
{
    class InvokeOnce
    {
        public bool IsInvoked { get; set; } = false;

        public void Invoke(System.Action action)
        {
            if (!IsInvoked)
            {
                action?.Invoke();
                IsInvoked = true;
            }
        }

        public void Reset()
        {
            IsInvoked = false;
        }
    }
}
