namespace net.niceygy.eddatacollector.handlers
{
    public class EDAM
    {
        const int ONE_HOUR = 1000 * 60 * 60;
        private List<string> uploaders;
        private object locker;
        private Thread thread;
        public EDAM()
        {
            this.locker = new object();
            lock (this.locker)
            {
                this.uploaders = new List<string>();
                this.thread = new Thread(Loop);
                thread.Start();
            }

        }

        private void Loop()
        {
            while (true)
            {
                Thread.Sleep(ONE_HOUR);

                lock (this.locker)
                {
                    int uniqueUploaders = this.uploaders.Count;
                    long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                    //save
                }
            }
        }

        public void AddUploader(string ID)
        {
            lock (this.locker)
            {
                if (!this.uploaders.Contains(ID))
                {
                    this.uploaders.Add(ID);
                }
            }
        }
    }
}