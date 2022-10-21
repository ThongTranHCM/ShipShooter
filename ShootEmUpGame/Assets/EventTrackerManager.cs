using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlurrySDK;

public class EventTrackerManager : MonoBehaviour
{
    abstract class EventTrackerDecorator{
        protected EventTrackerDecorator component;
        public EventTrackerDecorator(EventTrackerDecorator Component = null){
            component = Component;
        }
        public abstract void Init();
        public abstract void Log(string Event, Dictionary<string, string> Params);
    }
    class FirebaseDecorator: EventTrackerDecorator{
        public FirebaseDecorator(EventTrackerDecorator Component = null):base(Component){}
        public override void Init(){
            if(component != null){
                component.Init();
            }
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available) {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                } else {
                    UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
            return;
        }
        public override void Log(string Event, Dictionary<string, string> Params){
            if(component != null){
                component.Log(Event, Params);
            }
            Firebase.Analytics.FirebaseAnalytics.LogEvent(Event);
            Firebase.Analytics.Parameter[] p = new Firebase.Analytics.Parameter[Params.Count];
            int index = 0;
            foreach(KeyValuePair<string, string> element in Params) {
                p[index] = new Firebase.Analytics.Parameter(element.Key, element.Value);
                index += 1;
            }
            Firebase.Analytics.FirebaseAnalytics.LogEvent(Event, p);
            return;
        }
    }
    class FlurryDecorator: EventTrackerDecorator{
        string key;
        public FlurryDecorator(string Key, EventTrackerDecorator Component = null):base(Component){
            key = Key;
        }
        public override void Init(){
            if(component != null){
                component.Init();
            }
            new Flurry.Builder()
                  .WithCrashReporting(true)
                  .WithLogEnabled(true)
                  .WithLogLevel(Flurry.LogLevel.VERBOSE)
                  .WithMessaging(true)
                  .Build(key);
            return;
        }
        public override void Log(string Event, Dictionary<string, string> Params){
            if(component != null){
                component.Log(Event, Params);
            }
            Flurry.LogEvent(Event, Params);
            return;
        }
    }

    private static EventTrackerManager instance;
    public static EventTrackerManager Instance{
        get { return instance;}
    }
    private EventTrackerDecorator eventTracker;


    private EventTrackerManager(){
        eventTracker = new FlurryDecorator("123");
        eventTracker = new FirebaseDecorator(eventTracker);
    }
    void Start(){
        if(instance != null){
            instance = new EventTrackerManager();
        }
    }
    public void Log(string Event, Dictionary<string, string> Params){
        eventTracker.Log(Event, Params);
    }
}
