using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
class Cat : Window {
    [StructLayout(LayoutKind.Sequential)] struct LASTINPUTINFO { public uint cbSize, dwTime; }
    [DllImport("user32.dll")] static extern bool GetLastInputInfo(ref LASTINPUTINFO info);
    [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
    Image pet=new Image();
    Dictionary<BitmapSource,double> feet=new Dictionary<BitmapSource,double>();
    Dictionary<BitmapSource,double> fits=new Dictionary<BitmapSource,double>();
    BitmapSource sleep,hiss,sit,blink;
    BitmapSource[] walk=new BitmapSource[8];
    BitmapSource[] stretch; bool wasSleeping; double stretchUntil; const double StretchSeconds=1.8;
    WalkCycle gait=new WalkCycle();
    bool wasWalking; double turnUntil, facing=-1; int previousDirection=-1;
    Behavior behavior=new Behavior();
    ScaleTransform scale=new ScaleTransform(1,1);
    TranslateTransform shift=new TranslateTransform();
    Stopwatch clock=Stopwatch.StartNew();
    double previous, localActivity, hissUntil;
    uint lastInput;
    IntPtr foreground;
    Point down,origin;
    bool dragging,moved,manualSleep;
    double size=160;
    [STAThread] static void Main() {
        try { new Application().Run(new Cat()); }
        catch(Exception e) { MessageBox.Show(e.ToString(),"Unable to start desktop pet"); }
    }
    BitmapSource[] Sheet(string file,int columns,int rows) {
        var b=new BitmapImage(); b.BeginInit(); b.CacheOption=BitmapCacheOption.OnLoad;
        b.UriSource=new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,file)); b.EndInit(); b.Freeze();
        int w=b.PixelWidth/columns, h=b.PixelHeight/rows, count=columns*rows; var frames=new BitmapSource[count];
        for(int i=0;i<count;i++){frames[i]=new CroppedBitmap(b,new Int32Rect((i%columns)*w,(i/columns)*h,w,h)); frames[i].Freeze();
            var rgba=new FormatConvertedBitmap(frames[i],PixelFormats.Bgra32,null,0);
            byte[] pixels=new byte[w*h*4]; rgba.CopyPixels(pixels,w*4,0);
            int bottom=h-1; bool found=false;
            for(int y=h-1;y>=0 && !found;y--) for(int x=0;x<w;x++) if(pixels[(y*w+x)*4+3]>200){bottom=y;found=true;break;}
                        feet[frames[i]]=(bottom+1)/(double)h;
            int top=0;found=false;
            for(int y=0;y<h && !found;y++) for(int x=0;x<w;x++) if(pixels[(y*w+x)*4+3]>200){top=y;found=true;break;}
            fits[frames[i]]=Math.Min(.94,.84/Math.Max(.01,(bottom-top+1)/(double)h));
        }
        return frames;
    }
    public Cat() {
        Title="Sandrone Cat v3.2"; Width=size; Height=size;
        WindowStyle=WindowStyle.None; ResizeMode=ResizeMode.NoResize;
        AllowsTransparency=true; Background=Brushes.Transparent; Topmost=true; ShowInTaskbar=false;
        // Do not take keyboard focus away from the app the user is using.
        ShowActivated=false; Focusable=false;
        var a=Sheet("sprites.png",4,3); sleep=a[0]; hiss=a[1];
        sit=a[2]; blink=a[3]; Array.Copy(a,4,walk,0,8);
                stretch=Sheet("stretch.png",3,2);
        // One stable scale for the entire walk cycle prevents frame-to-frame zoom.
        double walkFit=1,stretchFit=1;
        foreach(var frame in walk)walkFit=Math.Min(walkFit,fits[frame]);
        foreach(var frame in stretch)stretchFit=Math.Min(stretchFit,fits[frame]);
        foreach(var frame in walk)fits[frame]=walkFit;
        foreach(var frame in stretch)fits[frame]=stretchFit;
        pet.Source=sit; pet.Stretch=Stretch.Fill;
        pet.RenderTransformOrigin=new Point(.5,.93);
        var transforms=new TransformGroup(); transforms.Children.Add(scale); transforms.Children.Add(shift);
        pet.RenderTransform=transforms; Content=pet;
        var menu=new ContextMenu();
        Add(menu,"Sleep now (wake on activity)",delegate { ReadActivity(); manualSleep=true; hissUntil=0; });
        Add(menu,"Hiss",delegate { Hiss(); });
        Add(menu,"Mini - 120",delegate { SetSize(120); });
        Add(menu,"Default - 160",delegate { SetSize(160); });
        Add(menu,"Large - 200",delegate { SetSize(200); });
        Add(menu,"Move to bottom-right corner",delegate { Home(); });
        Add(menu,"Exit",delegate { Close(); }); pet.ContextMenu=menu;
        Loaded+=delegate { Home(); ReadActivity(); };
        MouseLeftButtonDown+=delegate(object s,MouseButtonEventArgs e){
            down=PointToScreen(e.GetPosition(this)); origin=new Point(Left,Top);
            dragging=true; moved=false; CaptureMouse();
        };
        MouseMove+=delegate(object s,MouseEventArgs e){
            if(!dragging)return;
            Point p=PointToScreen(e.GetPosition(this));
            var matrix=PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            Vector d=matrix.Transform(p-down);
            if(Math.Abs(d.X)+Math.Abs(d.Y)>5)moved=true;
            if(moved){Left=origin.X+d.X; Top=origin.Y+d.Y;}
        };
        MouseLeftButtonUp+=delegate {
            if(!dragging)return; dragging=false; ReleaseMouseCapture();
            if(!moved)Hiss(); else { Clamp(); behavior.X=Left; }
        };
        LostMouseCapture+=delegate { dragging=false; };
        var timer=new DispatcherTimer { Interval=TimeSpan.FromMilliseconds(16) };
        timer.Tick+=delegate { Tick(); }; timer.Start();
        SystemParameters.StaticPropertyChanged+=delegate { if(!dragging)Clamp(); };
    }
    void ReadActivity() {
        var info=new LASTINPUTINFO(); info.cbSize=(uint)Marshal.SizeOf(info);
        IntPtr window=GetForegroundWindow();
        bool valid=GetLastInputInfo(ref info);
        if((valid && info.dwTime!=lastInput) || window!=foreground){
            lastInput=info.dwTime; foreground=window;
            localActivity=clock.Elapsed.TotalSeconds; manualSleep=false;
        }
    }
    void Tick() {
        double now=clock.Elapsed.TotalSeconds,dt=Math.Min(.1,Math.Max(0,now-previous)); previous=now;
        ReadActivity();
        if(dragging || pet.ContextMenu.IsOpen)return;
        var r=SystemParameters.WorkArea;
        behavior.X=Left;
                double before=Left;
        behavior.Update(now-localActivity,now<turnUntil?0:dt,r.Left+4,Math.Max(r.Left+4,r.Right-Width-4));
        bool angry=now<hissUntil;
        Mood state=manualSleep?Mood.Sleeping:behavior.State;
                if(wasSleeping && state!=Mood.Sleeping) {
            stretchUntil=now+StretchSeconds;
            if(angry)hissUntil+=StretchSeconds;
        }
        bool stretching=now<stretchUntil;
        if(state==Mood.Sleeping && !wasSleeping) { stretchUntil=0; stretching=false; }
        bool walking=!angry&&!stretching&&(state==Mood.Walking||state==Mood.Returning);
        wasSleeping=state==Mood.Sleeping && !angry && !stretching;
        if(!manualSleep && (state==Mood.Walking||state==Mood.Returning||state==Mood.Sleeping)) {
            Left=behavior.X;
            // Both walking and returning are locked to the bottom of the work area.
            Top=r.Bottom-Height*.93;
        }
                if(walking) {
            if(!wasWalking) { gait.Reset(); facing=behavior.Direction; previousDirection=behavior.Direction; }
            if(behavior.Direction!=previousDirection) { turnUntil=now+.16; previousDirection=behavior.Direction; }
            gait.Advance(Left-before);
        }
        wasWalking=walking;
        bool blinking=(now%4.6)>4.43;
        pet.Source=stretching?stretch[Math.Min(5,(int)((now-(stretchUntil-StretchSeconds))/StretchSeconds*6))]:angry?hiss:walking?walk[gait.Frame(Width)]:state==Mood.Sleeping?sleep:blinking?blink:sit;
                double target=walking?behavior.Direction:1;
        if(walking) facing+=Math.Max(-dt*12,Math.Min(dt*12,target-facing)); else facing=1;
        double fit=fits[(BitmapSource)pet.Source];
        scale.ScaleX=facing*fit;
        scale.ScaleY=fit*(1+(angry?.012*Math.Sin(now*24):walking||stretching?0:.012*Math.Sin(now*2.1)));
        shift.Y=Height*(.93-feet[(BitmapSource)pet.Source])*fit;
    }
    void Hiss(){hissUntil=clock.Elapsed.TotalSeconds+2; manualSleep=false; localActivity=clock.Elapsed.TotalSeconds;}
    void SetSize(double value){double bottom=Top+Height*.93; Width=Height=size=value; Top=bottom-Height*.93; Clamp();}
    void Home(){var r=SystemParameters.WorkArea; Left=r.Right-Width-4; Top=r.Bottom-Height*.93; behavior.X=Left;}
    void Clamp(){var r=SystemParameters.WorkArea; Left=Math.Max(r.Left,Math.Min(Left,r.Right-Width)); Top=Math.Max(r.Top,Math.Min(Top,r.Bottom-Height*.93));}
    void Add(ContextMenu menu,string text,Action action){var item=new MenuItem{Header=text}; item.Click+=delegate{action();};menu.Items.Add(item);}
}
