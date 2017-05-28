using strange.extensions.dispatcher.eventdispatcher.api;

public interface IPawn // TODO: => IRival ???
{
    void Init(IPawnConfig config);
    IEventDispatcher dispatcher{ get; set; }


    // TODO: refactor актуализировать
    void HitHandler();
    PawnKind Kind { get; }
    //float Health { get; set; }
    bool Disabled { get; set; }
}