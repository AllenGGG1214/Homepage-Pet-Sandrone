using System;
public class WalkCycle {
    double travel;
    public void Advance(double distance) { travel+=Math.Abs(distance); }
    public int Frame(double size) { return (int)Math.Floor(travel/Math.Max(1,size*.20)*8+1e-8)%8; }
    public void Reset() { travel=0; }
}


