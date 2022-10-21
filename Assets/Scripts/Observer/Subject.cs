using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject {
    List<Observer> observers = new List<Observer>();

    public void Notify() {
        foreach (Observer curObserver in observers)
            curObserver.OnNotify();
    }

    public void AddObserver(Observer observer) {
        observers.Add(observer);
    }
}
