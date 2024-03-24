public class ClockCalculator
{
    private const int SecondsInMinute = 60;

    public double CenterX { get; }
    public double CenterY { get; }
    public double SecondsHandLength { get; }

    public ClockCalculator(double centerX, double centerY, double secondsHandLength)
    {
        CenterX = centerX;
        CenterY = centerY;
        SecondsHandLength = secondsHandLength;
    }

    public double CalculateSecondHandX(TimeSpan elapsedTime)
    {
        return CenterX + SecondsHandLength * Math.Sin(2 * Math.PI * (double)elapsedTime.Seconds / SecondsInMinute);
    }

    public double CalculateSecondHandY(TimeSpan elapsedTime)
    {
        return CenterY - SecondsHandLength * Math.Cos(2 * Math.PI * (double)elapsedTime.Seconds / SecondsInMinute);
    }
}
