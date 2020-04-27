namespace WebWatcher.UI.Interfaces
{
    public interface IUCControlPanelViewModel
    {
        string State { get; set; }
        double Time { get; set; }
        string Url { get; set; }
        bool CanChangeAddress { get; }
        bool CanReset { get; }
        bool CanStart { get; }
        bool CanStop { get; }
        void Reset();
        void Start();
        void Stop();
    }
}