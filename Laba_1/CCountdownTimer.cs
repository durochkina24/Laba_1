using System;

namespace Laba_1
{
    public class CCountdownTimer
    {
        private double targetTime;
        private double currentTime;
        private bool isRunning;

        public CCountdownTimer(double targetTime)
        {
            this.targetTime = targetTime;
            this.currentTime = 0;
            this.isRunning = false;
        }

        public void Start()
        {
            currentTime = 0;
            isRunning = true;
        }

        public void Update(double delta)
        {
            if (!isRunning) return;
            currentTime += delta;
            if (currentTime >= targetTime) isRunning = false;
        }

        public void Reset()
        {
            currentTime = 0;
            isRunning = false; // Мгновенно готов к клику
        }

        public bool IsReady() => !isRunning;
        public void ChangeTargetTime(double newTime) => targetTime = newTime;
        public double GetTargetTime() => targetTime;
    }
}