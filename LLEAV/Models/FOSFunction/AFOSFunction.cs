namespace LLEAV.Models.FOSFunction
{
    public abstract class AFOSFunction
    {
        public abstract string Depiction { get; }
        public abstract FOS CalculateFOS(Population population, int numberOfBits);
    }
}
