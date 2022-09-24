using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Domain.Extensions;
public static class RatingExtensions
{
    public static Rating ToRating(this string ratingString) 
        => ratingString switch
    {
        "ER" => Rating.ER,
        "L" => Rating.L,
        "10" => Rating.Rate10,
        "12" => Rating.Rate12,
        "14" => Rating.Rate14,
        "16" => Rating.Rate16,
        "18" => Rating.Rate18,
        _ => throw new ArgumentOutOfRangeException(nameof(ratingString))
    };
}
