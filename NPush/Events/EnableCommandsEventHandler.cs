using System;

namespace NPush.Events
{
    class EnableCommandsEventHandler : EventArgs
    {
        private readonly bool enabled;

        public EnableCommandsEventHandler(bool enabled)
        {
            this.enabled = enabled;
        }

        public bool Enabled
        {
            get { return this.enabled; }
        }
    }
}
