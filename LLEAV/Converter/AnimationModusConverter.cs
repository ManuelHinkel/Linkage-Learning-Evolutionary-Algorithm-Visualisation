using LLEAV.ViewModels;

namespace LLEAV.Converter
{
    /// <summary>
    /// Takes an animation modus enum and checks, if it its index is smaller or equal to another
    /// enum given by a string.
    /// </summary>
    public class AnimationModusConverter : EnumOrSmallerEqualsStringConverter<AnimationModus>
    {
    }
}
