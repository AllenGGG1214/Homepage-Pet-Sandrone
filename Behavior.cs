using System;
public enum Mood { Sitting, Walking, Returning, Sleeping }
public class Behavior {
    public Mood State = Mood.Sitting;
    public double X;
    public int Direction = -1;
    public void Update(double idle, double dt, double min, double max) {
        if(idle < 30) { State=Mood.Sitting; return; }
        if(idle < 60) {
            State=Mood.Walking;
            X+=Direction*48*dt;
            if(X<=min){X=min; Direction=1;}
            if(X>=max){X=max; Direction=-1;}
            return;
        }
        double target=(X-min <= max-X)?min:max;
        if(Math.Abs(X-target)<=48*dt) { X=target; State=Mood.Sleeping; }
        else { State=Mood.Returning; Direction=target<X?-1:1; X+=Direction*48*dt; }
    }
}
