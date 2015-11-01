using System;

namespace NoelPush.Events
{
    class BalloonTipMessageEventHandler : EventArgs
    {
        private readonly string text;

        public BalloonTipMessageEventHandler(string text)
        {
            this.text = text;
        }

        public string Text
        {
            get { return this.text; }
        }
    }
}
