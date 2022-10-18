using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Invokes the notificaton method
public class Subject {
    //A list with observers that are waiting for something to happen
    List<Observer> observers = new List<Observer>();
    Observer observerTmp;

    //Send notifications if something has happened
    public void Notify() {
        //Notify all observers even though some may not be interested in what has happened
        //Each observer should check if it is interested in this event
        foreach (Observer curObserver in observers)
            curObserver.OnNotify();
    }

    //Add observer to the list
    public void AddObserver(Observer observer) {
        observers.Add(observer);
        observerTmp = observer;
    }
}
