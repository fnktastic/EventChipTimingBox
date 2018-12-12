namespace ECTL
{
    using System;

    public class Disposable : IDisposable
    {
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool bDisposing)
        {
        }

        ~Disposable()
        {
            this.Dispose(false);
        }
    }
}

